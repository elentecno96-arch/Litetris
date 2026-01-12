using Cysharp.Threading.Tasks;
using Game.SO;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Game.Sound;
using System.Linq;

namespace Game.Manager
{
    public class RhythmManager : MonoBehaviour
    {
        public static RhythmManager Instance { get; private set; }

        public event Action<int> OnBeat;
        public event Action<int> OnPhaseChanged;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private List<Game.Sound.SongData> playlist;

        [Header("Phase Data")]
        [SerializeField] private List<PatternPhaseSO> phaseList;

        [Header("Runtime Status (Read Only)")]
        [SerializeField] private float currentSurvivalTime = 0f;
        private int currentPhaseIndex = -1;
        private PatternPhaseSO currentPhaseData;
        private float phaseTimer = 0f;
        private float singleSpawnInterval;
        private float currentWarnTime;
        private float lastSingleSpawnTime = 0f;
        private float secPerBeat;
        private float songStartedTime;
        private int completedBeats = 0;
        private int currentSongIndex;

        private float lastOldPatternSpawnTime = 0f;
        private float lastComplexSpawnTime = 0f;
        private float nextOldPatternInterval = 1.0f;

        private List<int> availablePatternIDs;
        private bool isDirectorActive = false;
        private CancellationTokenSource _directorCTS;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            availablePatternIDs = new List<int>();
        }

        public void StartRhythmDirector()
        {
            if (playlist == null || playlist.Count == 0) return;

            StopDirector();
            _directorCTS = new CancellationTokenSource();

            currentSongIndex = UnityEngine.Random.Range(0, playlist.Count);
            currentSurvivalTime = 0f;
            lastSingleSpawnTime = 0f;
            lastOldPatternSpawnTime = 0f;
            completedBeats = 0;
            availablePatternIDs.Clear();
            currentPhaseIndex = -1;

            PlayCurrentSong();
            GoToNextPhase(_directorCTS.Token).Forget();
            isDirectorActive = true;
        }
        //음악 종료 시 비트계산, 실행중인 비동기 로직 중단
        public void StopDirector()
        {
            isDirectorActive = false;
            musicSource.Stop();
            _directorCTS?.Cancel(); //예약되어 있던 UniTask취소
            _directorCTS?.Dispose();//취소된 UniTask를 메모리에서 삭제(파괴)
            _directorCTS = null;    //빈손 명시 "작동 중인거 없음"
        }

        public void ResetDirector() => StopDirector();

        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;
            if (!isDirectorActive || currentPhaseData == null) return;

            if (!musicSource.isPlaying)
            {
                OnSongEnded();
                return;
            }

            currentSurvivalTime += Time.deltaTime;
            phaseTimer += Time.deltaTime;

            HandleDifficultyScaling();

            //AudioSettings.depTime : 오디오 카드의 하드웨어 시계기준
            //Time.time = 프레임 기반이라 알맞지 않음
            //1분(60초)를 bpm으로 나눠서 비트를 하나당 초secPerBeat를 구한 뒤
            //현재 음악 위치가 그 시간을 넘을 때 마다 OnBeat이벤트 
            float songPosition = (float)(AudioSettings.dspTime - songStartedTime);
            if (songPosition > (completedBeats + 1) * secPerBeat)
            {
                completedBeats++;
                OnBeatExecuted(completedBeats, songPosition);
            }
        }

        private void HandleDifficultyScaling()
        {
            
            float progress = Mathf.Clamp01(phaseTimer / currentPhaseData.duration);
            //Mathf.Lerp = 선형 보간 
            //페이즈 시작, 종료 사이에서 현재시간(progress)에 맞춰 값을 부드럽게 변화시킴
            float targetSpawnInterval = Mathf.Lerp(currentPhaseData.startSpawnInterval, currentPhaseData.endSpawnInterval, progress);
            float targetWarnTime = Mathf.Lerp(currentPhaseData.startWarnTime, currentPhaseData.endWarnTime, progress);

            if (currentPhaseIndex >= phaseList.Count - 1 && phaseTimer > currentPhaseData.duration)
            {
                float extraTime = phaseTimer - currentPhaseData.duration;

                targetSpawnInterval -= extraTime * 0.005f;
                targetSpawnInterval = Mathf.Max(targetSpawnInterval, 0.25f);

                targetWarnTime -= extraTime * 0.002f;
                targetWarnTime = Mathf.Max(targetWarnTime, 0.4f);
            }

            singleSpawnInterval = targetSpawnInterval;
            currentWarnTime = targetWarnTime;

            if (phaseTimer >= currentPhaseData.duration)
            {
                if (currentPhaseIndex < phaseList.Count - 1)
                {
                    GoToNextPhase(_directorCTS?.Token ?? default).Forget();
                }
            }
        }

        private async UniTaskVoid GoToNextPhase(CancellationToken token)
        {
            int nextIndex = currentPhaseIndex + 1;

            if (nextIndex >= phaseList.Count)
            {
                return;
            }

            currentPhaseIndex = nextIndex;
            isDirectorActive = false;
            currentPhaseData = phaseList[currentPhaseIndex];
            phaseTimer = 0f;

            OnPhaseChanged?.Invoke(currentPhaseIndex + 1);

            if (!availablePatternIDs.Contains(currentPhaseData.unlockPatternID))
            {
                availablePatternIDs.Add(currentPhaseData.unlockPatternID);
            }

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);
                isDirectorActive = true;
            }
            catch (OperationCanceledException) { }
        }

        private void OnBeatExecuted(int beatCount, float songPos)
        {
            OnBeat?.Invoke(beatCount);
            if (beatCount % 2 == 0)
            {
                CameraManager.Instance.BeatPulse(0.15f);
                CameraManager.Instance.FlashBackgroundColor(new Color(0.15f, 0.15f, 0f));
            }
            if (songPos >= lastSingleSpawnTime + singleSpawnInterval)
            {
                ExecuteSinglePattern();
                lastSingleSpawnTime = songPos;
            }

            if (songPos >= lastComplexSpawnTime + singleSpawnInterval)
            {
                HandlePatternSpawning(songPos);
                lastComplexSpawnTime = songPos;
            }
        }

        private void HandlePatternSpawning(float songPos)
        {
            if (availablePatternIDs.Count == 0) return;
            //최신 패턴 우선 실행
            //과거 패턴 무작위 실행
            int latestPID = availablePatternIDs[availablePatternIDs.Count - 1];
            ExecuteSpecificPattern(latestPID);

            if (availablePatternIDs.Count > 1 && songPos >= lastOldPatternSpawnTime + nextOldPatternInterval)
            {
                int spawnCount = UnityEngine.Random.Range(1, 3);

                for (int i = 0; i < spawnCount; i++)
                {
                    int oldPatternIndex = UnityEngine.Random.Range(0, availablePatternIDs.Count - 1);
                    int oldPID = availablePatternIDs[oldPatternIndex];

                    ExecuteSpecificPattern(oldPID);
                }

                lastOldPatternSpawnTime = songPos;
                nextOldPatternInterval = UnityEngine.Random.Range(0.5f, 1.5f);
            }
        }
        //계속 유지되는 패턴
        private void ExecuteSinglePattern()
        {
            var player = GameManager.Instance.PlayerController;
            if (player == null) return;

            int rx = UnityEngine.Random.Range(player.CurX - 1, player.CurX + 2);
            int ry = UnityEngine.Random.Range(player.CurY - 1, player.CurY + 2);

            PatternManager.Instance.ExecutePattern(3, rx, ry, 0, currentWarnTime, 0.2f).Forget();
        }
        //특수 패턴
        private void ExecuteSpecificPattern(int pID)
        {
            int rx = UnityEngine.Random.Range(0, BoardManager.Instance.BoardSize);
            int ry = UnityEngine.Random.Range(0, BoardManager.Instance.BoardSize);

            float dangerTime = Mathf.Max(0.15f, 0.3f - (currentPhaseIndex * 0.02f));

            PatternManager.Instance.ExecutePattern(pID, rx, ry, 3, currentWarnTime + 0.2f, dangerTime).Forget();
        }
        private void PlayCurrentSong()
        {
            var data = playlist[currentSongIndex];
            musicSource.clip = data.clip;
            secPerBeat = 60f / data.bpm;
            songStartedTime = (float)AudioSettings.dspTime;
            completedBeats = 0;

            lastSingleSpawnTime = 0f;
            lastComplexSpawnTime = 0f;
            lastOldPatternSpawnTime = 0f;

            nextOldPatternInterval = UnityEngine.Random.Range(0.5f, 1.5f);

            musicSource.Play();
        }
        private void OnSongEnded()
        {
            currentSongIndex = (currentSongIndex + 1) % playlist.Count;
            PlayCurrentSong();
        }
        private void OnDestroy()
        {
            _directorCTS?.Cancel();
            _directorCTS?.Dispose();
        }
    }
}