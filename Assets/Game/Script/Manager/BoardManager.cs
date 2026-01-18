using Cysharp.Threading.Tasks;
using Game.Bulb.LightCube;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

namespace Game.Script.Manager
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

        //각 행, 열, 전체 큐브 리스트를 미리 캐싱
        private List<LightCube>[] rows;
        private List<LightCube>[] cols;
        private List<LightCube> allCubesList;


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

            //캐싱
            allCubesList = new List<LightCube>(cubes.Length);
            rows = new List<LightCube>[boardSize];
            cols = new List<LightCube>[boardSize];
            for (int i = 0; i < boardSize; i++)
            {
                rows[i] = new List<LightCube>();
                cols[i] = new List<LightCube>();
            }

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

                        //생성할 때 미리 캐싱
                        allCubesList.Add(cube);
                        rows[y].Add(cube);
                        cols[x].Add(cube);
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
            if (x < 0 || x >= boardSize || y < 0 || y >= boardSize) return null;
            return cubes[y * boardSize + x];
        }
        #region [인트로 애니메이션]
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
        #endregion
        #region [전달 방식 프로퍼티]
        //가로/세로 전달
        public List<LightCube> GetRow(int y) => (y >= 0 && y < boardSize) ? rows[y] : new List<LightCube>();
        public List<LightCube> GetColumn(int x) => (x >= 0 && x < boardSize) ? cols[x] : new List<LightCube>();
        //전체 전달
        public List<LightCube> GetAllCubes() => allCubesList;
        //범위 전달
        public List<LightCube> GetCubesInFilter(int centerX, int centerY, int r, Func<int, int, bool> filter)
        {
            List<LightCube> results = new List<LightCube>();

            int minX = Mathf.Max(0, centerX - r);
            int maxX = Mathf.Min(boardSize - 1, centerX + r);
            int minY = Mathf.Max(0, centerY - r);
            int maxY = Mathf.Min(boardSize - 1, centerY + r);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (filter(x, y))
                    {
                        var cube = GetCube(x, y);
                        if (cube != null) results.Add(cube);
                    }
                }
            }
            return results;
        }
        #endregion
    }
}