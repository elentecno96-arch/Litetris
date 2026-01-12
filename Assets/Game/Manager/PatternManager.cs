using Cysharp.Threading.Tasks;
using Game.Bulb.LightCube;
using Game.Pattern.Interface;
using Game.Pattern.PatternStrategy;
using Game.Pattern.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game.Manager
{
    public class PatternManager : MonoBehaviour
    {
        public static PatternManager Instance { get; private set; }
        //패턴들을 담아두는 곳, 찾기 쉽게 딕셔너리로 관리
        private Dictionary<int, IPatternStrategy> strategies;
        private CancellationTokenSource cts;

        private void Awake()
        {
            Instance = this;
            cts = new CancellationTokenSource();
            InitializeStrategies();
        }

        private void InitializeStrategies()
        {
            strategies = new Dictionary<int, IPatternStrategy>
            {
                { 0, new SquareRingPattern() },
                { 1, new DiamondPattern() },
                { 2, new LinePattern() },
                { 3, new SinglePattern() },
                { 4, new AllLinePattern(false) },
                { 5, new AllLinePattern(true) },
                { 6, new DiagonalPattern(false) },
                { 7, new DiagonalPattern(true) },
                { 8, new CirclePattern() },
                { 9, new SquarePattern() },
                { 10, new TrianglePattern() },
                {11, new SmallCross() }
            };
        }

        //중지 - 파괴 - 다시 할당
        public void StopAllPatterns()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = new CancellationTokenSource();
            Debug.Log("멈춰!");
        }

        public async UniTask ExecutePattern(int patternId, int x, int y, int range, float warn, float danger)
        {
            if (!strategies.TryGetValue(patternId, out IPatternStrategy strategy)) return;

            var targets = strategy.GetTargetCubes(x, y, range);
            if (targets == null) return;

            // 성능 최적화: sqrMagnitude 사용
            var sortedTargets = targets
                .Where(c => c != null)
                //보통 거리를 구할 때는 루트를 씌우는 Vector2.Distance를 사용하지만 연산 무거움
                //그래서 루트를 씌우기 전 값인 sqrMagnitude를 사용해 CPU부담 줄임
                //OrderBy = LINQ
                //OrderByDescending = 거리가 먼 것부터 정렬
                //나열된 데이터를 특정 기준에 따라 오름차순으로 정렬
                //Vector2.Distance = 내부적으로 (제곱근/루트)연산
                .OrderBy(c => (new Vector2(x, y) - new Vector2(c.X, c.Y)).sqrMagnitude);

            var token = cts.Token;
            try
            {
                foreach (var cube in sortedTargets)
                {
                    if (token.IsCancellationRequested) return;
                    RunSequenceWithDelay(cube, warn, danger, token).Forget();
                    await UniTask.Delay(TimeSpan.FromSeconds(0.04f), cancellationToken: token);
                }
            }
            catch (OperationCanceledException) { }
        }

        private async UniTaskVoid RunSequenceWithDelay(LightCube cube, float warn, float danger, CancellationToken token)
        {
            //Cube스스로가 실행하는 애니메이션(ActivateSequence)에 외부의 token을 강제로 연결
            //큐브의 애니메이션이 강제 종료가 된다면 token이 취소 되는 순간 큐브 내부의 로직도
            //즉시 멈추도록 강제함
            try { await cube.ActivateSequence(warn, danger).AttachExternalCancellation(token); }
            catch (OperationCanceledException) { }
        }
    }
}