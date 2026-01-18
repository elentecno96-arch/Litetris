using Game.Bulb.LightCube;
using Game.Script.Manager;
using Game.Pattern.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pattern.PatternStrategy
{
    public class SquareRingPattern : IPatternStrategy
    {
        public List<LightCube> GetTargetCubes(int centerX, int centerY, int range)
        {
            //range가 0이면 자기 자신만 반환
            if (range == 0)
            {
                var center = BoardManager.Instance.GetCube(centerX, centerY);
                return center != null ? new List<LightCube> { center } : new List<LightCube>();
            }

            //BoardManager의 범위 필터를 사용하여 테두리만 추출
            return BoardManager.Instance.GetCubesInFilter(centerX, centerY, range, (x, y) =>
            {
                int dx = Mathf.Abs(x - centerX);
                int dy = Mathf.Abs(y - centerY);

                //두 거리 중 최대값이 range와 일치하면 정사각형의 '변'에 위치한 큐브입니다.
                return Mathf.Max(dx, dy) == range;
            });
            //List<LightCube> targets = new List<LightCube>();
            //for (int i = -range; i <= range; i++)
            //{
            //    targets.Add(BoardManager.Instance.GetCube(centerX + i, centerY + range));
            //    targets.Add(BoardManager.Instance.GetCube(centerX + i, centerY - range));
            //    if (i != -range && i != range) 
            //    {
            //        targets.Add(BoardManager.Instance.GetCube(centerX - range, centerY + i));
            //        targets.Add(BoardManager.Instance.GetCube(centerX + range, centerY + i));
            //    }
            //}
            //return targets;
        }
    }
}
