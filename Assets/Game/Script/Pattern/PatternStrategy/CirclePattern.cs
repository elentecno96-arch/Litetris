using Game.Bulb.LightCube;
using Game.Script.Manager;
using Game.Pattern.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pattern.PatternStrategy
{
    public class CirclePattern : IPatternStrategy
    {
        public List<LightCube> GetTargetCubes(int centerX, int centerY, int range)
        {
            //판정을 위한 기준값 계산
            float innerRadiusSqr = Mathf.Pow(range - 0.7f, 2);
            float outerRadiusSqr = Mathf.Pow(range + 0.7f, 2);

            //보드 매니저에게 '범위'와 '판정 공식(람다)'을 전달
            //range + 1을 탐색 범위로 잡아서 원의 끝부분이 잘리지 않게 합니다.
            return BoardManager.Instance.GetCubesInFilter(centerX, centerY, range + 1, (x, y) =>
            {
                int dx = x - centerX;
                int dy = y - centerY;
                float distSqr = (dx * dx) + (dy * dy);

                //이 수식에 맞는 좌표만 리스트에 담겨서 돌아옵니다.
                return distSqr >= innerRadiusSqr && distSqr <= outerRadiusSqr;
            });
            //List<LightCube> targets = new List<LightCube>();
            //int boardSize = BoardManager.Instance.BoardSize;
            //int minX = Mathf.Max(0, centerX - range - 1);
            //int maxX = Mathf.Min(boardSize - 1, centerX + range + 1);
            //int minY = Mathf.Max(0, centerY - range - 1);
            //int maxY = Mathf.Min(boardSize - 1, centerY + range + 1);
            //float innerRadiusSqr = Mathf.Pow(range - 0.7f, 2);
            //float outerRadiusSqr = Mathf.Pow(range + 0.7f, 2);

            //for (int y = minY; y <= maxY; y++)
            //{
            //    for (int x = minX; x <= maxX; x++)
            //    {
            //        int dx = x - centerX;
            //        int dy = y - centerY;
            //        float distSqr = (dx * dx) + (dy * dy);
            //        if (distSqr >= innerRadiusSqr && distSqr <= outerRadiusSqr)
            //        {
            //            LightCube cube = BoardManager.Instance.GetCube(x, y);
            //            if (cube != null) targets.Add(cube);
            //        }
            //    }
            //}
            //return targets;
        }
    }
}