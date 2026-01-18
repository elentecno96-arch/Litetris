using Game.Bulb.LightCube;
using Game.Script.Manager;
using Game.Pattern.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pattern.PatternStrategy
{
    public class SquarePattern : IPatternStrategy
    {
        public List<LightCube> GetTargetCubes(int centerX, int centerY, int range)
        {
            //range가 0이면 자기 자신만 반환
            if (range == 0)
            {
                var center = BoardManager.Instance.GetCube(centerX, centerY);
                return center != null ? new List<LightCube> { center } : new List<LightCube>();
            }
            //보드 매니저에게 정사각형 테두리 공식(Chebyshev distance) 전달
            return BoardManager.Instance.GetCubesInFilter(centerX, centerY, range, (x, y) =>
            {
                int dx = Mathf.Abs(x - centerX);
                int dy = Mathf.Abs(y - centerY);

                //두 거리 중 최대값이 정확히 range와 같으면 '테두리'입니다.
                //만약 Mathf.Max(dx, dy) <= range 로 바꾸면 '꽉 찬 사각형'이 됩니다.
                return Mathf.Max(dx, dy) == range;
            });
            //List<LightCube> targets = new List<LightCube>();

            //for (int i = -range; i <= range; i++)
            //{
            //    // 위아래 가로줄, 좌우 세로줄 테두리만 추출
            //    AddTarget(centerX + i, centerY + range, targets); // 상
            //    AddTarget(centerX + i, centerY - range, targets); // 하
            //    AddTarget(centerX - range, centerY + i, targets); // 좌
            //    AddTarget(centerX + range, centerY + i, targets); // 우
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
