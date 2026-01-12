using Game.Bulb.LightCube;
using Game.Manager;
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
            List<LightCube> targets = new List<LightCube>();
            int boardSize = 9;

            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    if (isMainDiagonal)
                    {
                        if (x - y == centerX - centerY)
                            targets.Add(BoardManager.Instance.GetCube(x, y));
                    }
                    else
                    {
                        if (x + y == centerX + centerY)
                            targets.Add(BoardManager.Instance.GetCube(x, y));
                    }
                }
            }
            return targets;
        }
    }
}
