using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Script.Player;
using System;
using UnityEngine;

namespace Game.Script.Manager
{
    public class GameManager : MonoBehaviour
    {
        //씬이 하나이기 때문에 씬에 대한 상태를 저장하기 위해서 만들어야했음
        public enum GameState { Title, Intro, Playing, GameOver }
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private Transform playCameraTransform;

        [Header("Settings")]
        public int maxHealth = 3;
        private int currentHealth;
        private float playTime;
        private GameState currentState;
        private PlayerController _playerController;

        public GameState CurrentState => currentState;
        public PlayerController PlayerController => _playerController;

        //MVP 분리 작업을 위한 이벤트
        //public event Action<int> OnHealthChanged;
        public event Action OnPlayerDamaged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
            //중간에 할당치를 초과했다는 오류 때문에 급하게 찾아서 채운 코드
            //트윈의 기본 할당이 500이하였던거 같음
            DOTween.SetTweensCapacity(1000, 100);
        }

        private void Start()
        {
            currentState = GameState.Title;
            //Forget : async UniTaskVoid는 비동기적으로 실행되는 명령어 인데
            //Start처럼 일반 함수라서 비동기 함수가 끝날 때 까지 기다리는게 안됨
            //그래서 Forget을 이용해 강제적으로 실행시킴 무서운 친구임
            InitializeTitleSequence().Forget();
            
            
        }

        private async UniTaskVoid InitializeTitleSequence()
        {
            await UniTask.WaitUntil(() => BoardManager.Instance != null);
            BoardManager.Instance.StartTitleAnimation();
            UIManager.Instance.PlayTitleButtonAnimation();
        }

        private void CleanupGameSystem()
        {
            PatternManager.Instance.StopAllPatterns();
            RhythmManager.Instance.ResetDirector();
            BoardManager.Instance.ResetAllCubes();
            UIManager.Instance.HideResultPanel();
            UIManager.Instance.CloseAllPopups();
        }

        public void OnClickStart()
        {
            if (currentState == GameState.Title) StartGameSequence().Forget();
        }
        //이 친구가 핵심임 ㄹㅇ
        private async UniTaskVoid StartGameSequence()
        {
            BoardManager.Instance.StopTitleAnimation(); //타이틀 연출
            currentState = GameState.Intro;             //맞는 상태로 이동

            //카메라 연출이 끝날 때까지 기다림
            await CameraManager.Instance.TransitionToPlayView(playCameraTransform);
            //이전에 있던 정보 청소
            CleanupGameSystem();

            if (RhythmManager.Instance != null)
                RhythmManager.Instance.OnPhaseChanged += (phase) => UIManager.Instance.ShowPhasePopup(phase);

            UIManager.Instance.SetActiveHealthUI(true);
            //우리의 네모친구 소환!
            SpawnPlayer();

            UIManager.Instance.ResetHealthUI(maxHealth);
            UIManager.Instance.SetActiveHealthUI(true);

            //시작 카운트 이것도 다 지나갈 때 까지 기다려야함
            await UIManager.Instance.PlayCountdownSequence();

            currentHealth = maxHealth;
            playTime = 0;
            currentState = GameState.Playing;

            UIManager.Instance.UpdateHealth(currentHealth);
            //음악 재생 후 시작
            RhythmManager.Instance.StartRhythmDirector();
        }

        private void Update()
        {
            if (currentState == GameState.Playing)
            {
                playTime += Time.deltaTime;
                UIManager.Instance.UpdateSurvivalTime(playTime);
            }
        }

        private void SpawnPlayer()
        {
            if (_playerController != null) Destroy(_playerController.gameObject);
            int center = BoardManager.Instance.BoardSize / 2;
            _playerController = Instantiate(playerPrefab);
            _playerController.SetInitialPosition(center, center);
        }

        public void DecreaseHealth()
        {
            if (currentState != GameState.Playing) return;
            currentHealth--;

            //알람만 보냄
            //OnHealthChanged?.Invoke(currentHealth);
            OnPlayerDamaged?.Invoke();
            UIManager.Instance.UpdateHealth(currentHealth);
            if (currentHealth <= 0) GameOver();
        }

        private void GameOver()
        {
            currentState = GameState.GameOver;
            RhythmManager.Instance.StopDirector();

            // 데이터 저장 로직은 여기에 유지
            //float bestTime = PlayerPrefs.GetFloat("BestSurvivalTime", 0f);
            //if (playTime > bestTime) PlayerPrefs.SetFloat("BestSurvivalTime", playTime);

            PatternManager.Instance.StopAllPatterns();
            UIManager.Instance.ShowResult(playTime);
        }

        public void OnClickRestart()
        {
            if (currentState == GameState.GameOver) RestartSequence().Forget();
        }

        private async UniTaskVoid RestartSequence()
        {
            currentState = GameState.Intro;
            CleanupGameSystem();
            SpawnPlayer();
            await UIManager.Instance.PlayCountdownSequence();

            playTime = 0;
            currentHealth = maxHealth;
            currentState = GameState.Playing;

            UIManager.Instance.UpdateHealth(currentHealth);
            RhythmManager.Instance.StartRhythmDirector();
        }

        public void OnClickGoToTitle()
        {
            if (currentState == GameState.GameOver) GoToTitleSequence().Forget();
        }

        private async UniTaskVoid GoToTitleSequence()
        {
            CleanupGameSystem();
            currentState = GameState.Title;

            if (_playerController != null) Destroy(_playerController.gameObject);

            UIManager.Instance.HideHealthUI();

            // 카메라가 메인 뷰로 돌아가는 동안 대기
            await CameraManager.Instance.TransitionToMainView();
            UIManager.Instance.ShowTitleUI();

            // 보드 애니메이션 재시작
            BoardManager.Instance.StartTitleAnimation();
        }
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
    // 실제 빌드된 게임에서 실행 중일 때
    Application.Quit();
#endif
        }
    }
}