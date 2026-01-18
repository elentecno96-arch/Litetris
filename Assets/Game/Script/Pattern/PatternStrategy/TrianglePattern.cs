using Game.Bulb.LightCube;
using Game.Script.Manager;
using Game.Pattern.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pattern.PatternStrategy
{
    public class TrianglePattern : IPatternStrategy
    {
        public List<LightCube> GetTargetCubes(int centerX, int centerY, int range)
        {
            //range가 0이면 자기 자신만 반환
            if (range == 0)
            {
                var center = BoardManager.Instance.GetCube(centerX, centerY);
                return center != null ? new List<LightCube> { center } : new List<LightCube>();
            }

            //삼각형 테두리 판정 로직
            return BoardManager.Instance.GetCubesInFilter(centerX, centerY + (range / 2), range, (x, y) =>
            {
                int dy = y - centerY;
                int dx = Mathf.Abs(x - centerX);

                //밑변 판정 (y가 정확히 정점에서 range만큼 떨어진 위치)
                bool isBottom = (dy == range);

                //빗변 판정 (y가 내려갈수록 x의 허용 범위가 1씩 늘어남: 즉 |dx| == dy)
                //dy가 0보다 크고(정점 아래), dy가 dx와 같으면 빗변입니다.
                bool isSide = (dy >= 0 && dx == dy);

                return isBottom || isSide;
            });
            //List<LightCube> targets = new List<LightCube>();
            //for (int i = 0; i <= range; i++)
            //{
            //    AddTarget(centerX, centerY + i, targets);           
            //    AddTarget(centerX - i, centerY + range, targets);  
            //    AddTarget(centerX + i, centerY + range, targets);   

            //    AddTarget(centerX - i, centerY + i, targets);
            //    AddTarget(centerX + i, centerY + i, targets);
            //}
            //return targets;
        }

        //private void AddTarget(int x, int y, List<LightCube> list)
        //{
        //    LightCube cube = BoardManager.Instance.GetCube(x, y);
        //    if (cube != null && !list.Contains(cube)) list.Add(cube);
        //}
    }
}
