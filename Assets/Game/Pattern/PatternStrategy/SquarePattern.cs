using Game.Bulb.LightCube;
using Game.Manager;
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
            List<LightCube> targets = new List<LightCube>();

            for (int i = -range; i <= range; i++)
            {
                // 위아래 가로줄, 좌우 세로줄 테두리만 추출
                AddTarget(centerX + i, centerY + range, targets); // 상
                AddTarget(centerX + i, centerY - range, targets); // 하
                AddTarget(centerX - range, centerY + i, targets); // 좌
                AddTarget(centerX + range, centerY + i, targets); // 우
            }
            return targets;
        }

        private void AddTarget(int x, int y, List<LightCube> list)
        {
            LightCube cube = BoardManager.Instance.GetCube(x, y);
            if (cube != null && !list.Contains(cube)) list.Add(cube);
        }
    }
}
