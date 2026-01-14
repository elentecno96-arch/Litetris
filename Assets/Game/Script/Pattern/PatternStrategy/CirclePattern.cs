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
            List<LightCube> targets = new List<LightCube>();
            int boardSize = BoardManager.Instance.BoardSize;
            int minX = Mathf.Max(0, centerX - range - 1);
            int maxX = Mathf.Min(boardSize - 1, centerX + range + 1);
            int minY = Mathf.Max(0, centerY - range - 1);
            int maxY = Mathf.Min(boardSize - 1, centerY + range + 1);
            float innerRadiusSqr = Mathf.Pow(range - 0.7f, 2);
            float outerRadiusSqr = Mathf.Pow(range + 0.7f, 2);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    int dx = x - centerX;
                    int dy = y - centerY;
                    float distSqr = (dx * dx) + (dy * dy);
                    if (distSqr >= innerRadiusSqr && distSqr <= outerRadiusSqr)
                    {
                        LightCube cube = BoardManager.Instance.GetCube(x, y);
                        if (cube != null) targets.Add(cube);
                    }
                }
            }
            return targets;
        }
    }
}