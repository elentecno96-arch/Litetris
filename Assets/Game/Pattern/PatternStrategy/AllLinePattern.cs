using Game.Bulb.LightCube;
using Game.Manager;
using Game.Pattern.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pattern.PatternStrategy
{
    public class AllLinePattern : IPatternStrategy
    {
        private bool isVertical;
        public AllLinePattern(bool isVertical)
        {
            this.isVertical = isVertical;
        }

        public List<LightCube> GetTargetCubes(int centerX, int centerY, int range)
        {
            List<LightCube> targets = new List<LightCube>();
            int boardSize = 9;

            for (int i = 0; i < boardSize; i++)
            {
                int x = isVertical ? centerX : i;
                int y = isVertical ? i : centerY;

                var cube = BoardManager.Instance.GetCube(x, y);
                if (cube != null) targets.Add(cube);
            }
            return targets;
        }
    }
}
