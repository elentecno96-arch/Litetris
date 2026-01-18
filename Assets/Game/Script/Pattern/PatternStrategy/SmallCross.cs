using Game.Bulb.LightCube;
using Game.Pattern.Interface;
using System.Collections.Generic;
using UnityEngine;
using Game.Script.Manager;

namespace Game.Pattern.Strategy
{
    public class SmallCross : IPatternStrategy
    {
        public List<LightCube> GetTargetCubes(int centerX, int centerY, int range)
        {
            // range와 상관없이 '작은 십자'는 반지름 1의 범위를 가집니다.
            return BoardManager.Instance.GetCubesInFilter(centerX, centerY, 1, (x, y) =>
            {
                int dx = Mathf.Abs(x - centerX);
                int dy = Mathf.Abs(y - centerY);

                //중심점(0,0)이거나, 거리가 정확히 1인 지점(십자) 판정
                //dx + dy <= 1 공식 하나로 자기자신과 상하좌우가 모두 포함됩니다.
                return (dx + dy) <= 1;
            });
            //List<LightCube> targets = new List<LightCube>();
            //var board = BoardManager.Instance;

            //AddIfValid(targets, board, centerX, centerY);
            //AddIfValid(targets, board, centerX + 1, centerY);
            //AddIfValid(targets, board, centerX - 1, centerY);
            //AddIfValid(targets, board, centerX, centerY + 1);
            //AddIfValid(targets, board, centerX, centerY - 1);

            //return targets;
        }
        //별도의 유효성 검사는 보드매니저에서 처리하고 있어 불필요
        //private void AddIfValid(List<LightCube> list, BoardManager board, int x, int y)
        //{
        //    if (x >= 0 && x < board.BoardSize && y >= 0 && y < board.BoardSize)
        //    {
        //        var cube = board.GetCube(x, y);
        //        if (cube != null) list.Add(cube);
        //    }
        //}
    }
}