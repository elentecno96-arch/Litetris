using Game.Bulb.LightCube;
using Game.Manager;
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
            List<LightCube> targets = new List<LightCube>();
            for (int i = 0; i <= range; i++)
            {
                AddTarget(centerX, centerY + i, targets);           
                AddTarget(centerX - i, centerY + range, targets);  
                AddTarget(centerX + i, centerY + range, targets);   
                                                                    
                AddTarget(centerX - i, centerY + i, targets);
                AddTarget(centerX + i, centerY + i, targets);
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
