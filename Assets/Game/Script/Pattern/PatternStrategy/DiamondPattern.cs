using Game.Bulb.LightCube;
using Game.Script.Manager;
using Game.Pattern.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pattern.PatternStrategy
{
    public class DiamondPattern : IPatternStrategy
    {
        public List<LightCube> GetTargetCubes(int centerX, int centerY, int range)
        {
            if (range == 0)
            {
                var center = BoardManager.Instance.GetCube(centerX, centerY);
                return center != null ? new List<LightCube> { center } : new List<LightCube>();
            }

            //보드 매니저에게 다이아몬드 공식(맨해튼 거리)을 전달
            return BoardManager.Instance.GetCubesInFilter(centerX, centerY, range, (x, y) =>
            {
                int dx = Mathf.Abs(x - centerX);
                int dy = Mathf.Abs(y - centerY);

                // 테두리만 선택하려면 == range, 
                // 내부를 채우려면 <= range를 사용합니다.
                return (dx + dy) == range;
            });
            //List<LightCube> targets = new List<LightCube>();

            //for (int i = -range; i <= range; i++)
            //{
            //    int remain = range - Mathf.Abs(i);
            //    targets.Add(BoardManager.Instance.GetCube(centerX + i, centerY + remain));
            //    if (remain != 0)
            //        targets.Add(BoardManager.Instance.GetCube(centerX + i, centerY - remain));
            //}
            //return targets;
        }
    }
}