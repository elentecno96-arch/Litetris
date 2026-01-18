using Game.Bulb.LightCube;
using Game.Script.Manager;
using Game.Pattern.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pattern.PatternStrategy
{
    public class DiagonalPattern : IPatternStrategy
    {
        private bool isMainDiagonal;

        public DiagonalPattern(bool isMainDiagonal) => this.isMainDiagonal = isMainDiagonal;

        public List<LightCube> GetTargetCubes(int centerX, int centerY, int range)
        {
            int boardSize = BoardManager.Instance.BoardSize;

            //보드 전체를 범위로 잡고, 대각선 공식에 맞는 큐브만 필터링
            //(보드 중앙을 기준으로 보드 사이즈 절반만큼 탐색 범위를 잡으면 전체 커버 가능)
            return BoardManager.Instance.GetCubesInFilter(boardSize / 2, boardSize / 2, boardSize, (x, y) =>
            {
                if (isMainDiagonal)
                {
                    //우하향 대각선 (\): x - y 값이 일정함
                    return (x - y) == (centerX - centerY);
                }
                else
                {
                    //우상향 대각선 (/): x + y 값이 일정함
                    return (x + y) == (centerX + centerY);
                }
            });
            //List<LightCube> targets = new List<LightCube>();
            //int boardSize = 9;

            //for (int x = 0; x < boardSize; x++)
            //{
            //    for (int y = 0; y < boardSize; y++)
            //    {
            //        if (isMainDiagonal)
            //        {
            //            if (x - y == centerX - centerY)
            //                targets.Add(BoardManager.Instance.GetCube(x, y));
            //        }
            //        else
            //        {
            //            if (x + y == centerX + centerY)
            //                targets.Add(BoardManager.Instance.GetCube(x, y));
            //        }
            //    }
            //}
            //return targets;
        }
    }
}
