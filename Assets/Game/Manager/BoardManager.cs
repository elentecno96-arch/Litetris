using Cysharp.Threading.Tasks;
using Game.Bulb.LightCube;
using System;
using System.Threading;
using UnityEngine;

namespace Game.Manager
{
    public class BoardManager : MonoBehaviour
    {
        public static BoardManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private GameObject cubePrefab;
        [SerializeField] private int boardSize = 13;
        [SerializeField] private float spacing = 1.1f;

        //1차원 배열
        //1차원 배열이 메모리 연속성이 좋아 성능 이점이 있고,
        //전체 큐브를 순회할 때 코드가 깔끔해짐
        private LightCube[] cubes;
        private CancellationTokenSource _titleAnimationCTS;

        public int BoardSize => boardSize;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            GenerateBoard();
        }

        private void GenerateBoard()
        {
            //X * Spacing은 0,0으로 시작해 한쪽 방향으로만 치우셔 생성되기 때문에
            //전체 길이인 (boardSize - 1) * spacing의 절반을 뒤로 빼준다
            //그래야 좌우/상하 대칭이 가능
            cubes = new LightCube[boardSize * boardSize];
            float offset = (boardSize - 1) * spacing * 0.5f;

            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    Vector3 pos = new Vector3(x * spacing - offset, 0, y * spacing - offset);
                    GameObject obj = Instantiate(cubePrefab, pos, Quaternion.identity, transform);

                    if (obj.TryGetComponent<LightCube>(out var cube))
                    {
                        cube.Init(x, y);
                        cubes[GetIndexFromCoord(x, y)] = cube;
                    }
                }
            }
        }
        public void ResetAllCubes()
        {
            if (cubes == null) return;
            foreach (var cube in cubes)
            {
                cube?.ResetCube();
            }
        }
        public int GetIndexFromCoord(int x, int y)
        {
            if (x < 0 || x >= boardSize || y < 0 || y >= boardSize) return -1;
            return y * boardSize + x;
        }

        //외부에서 큐브를 요청 할 때 배달 역활
        public LightCube GetCube(int x, int y)
        {
            int index = GetIndexFromCoord(x, y);
            return index == -1 ? null : cubes[index];
        }
        public void StartTitleAnimation()
        {
            StopTitleAnimation();
            _titleAnimationCTS = new CancellationTokenSource();
            PlayTitleIdleLoop(_titleAnimationCTS.Token).Forget();
        }
        public void StopTitleAnimation()
        {
            _titleAnimationCTS?.Cancel();
            _titleAnimationCTS?.Dispose();
            _titleAnimationCTS = null;
            ResetAllCubes();
        }
        
        private async UniTaskVoid PlayTitleIdleLoop(CancellationToken token)
        {
            //예외처리
            try
            {
                //게임이 시작되거나 타이틀을 벗어날 때까지 실행중
                while (!token.IsCancellationRequested)
                {
                    if (cubes == null) break;
                    int randomIndex = UnityEngine.Random.Range(0, cubes.Length);
                    cubes[randomIndex]?.PlayTitleEffect(Color.HSVToRGB(UnityEngine.Random.value, 1, 1), 0.8f);
                    //게임 전체가 멈추지 않고 오직 1초 쉬었다가 실행
                    await UniTask.Delay(1000, cancellationToken: token);
                }
            }
            //해당 기능을 끄고 싶을 때 토큰 취소!
            catch (OperationCanceledException) { }
        }
    }
}