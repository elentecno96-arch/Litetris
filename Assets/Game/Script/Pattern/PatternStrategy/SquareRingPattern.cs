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
            List<LightCube> targets = new List<LightCube>();
            for (int i = -range; i <= range; i++)
            {
                targets.Add(BoardManager.Instance.GetCube(centerX + i, centerY + range));
                targets.Add(BoardManager.Instance.GetCube(centerX + i, centerY - range));
                if (i != -range && i != range) 
                {
                    targets.Add(BoardManager.Instance.GetCube(centerX - range, centerY + i));
                    targets.Add(BoardManager.Instance.GetCube(centerX + range, centerY + i));
                }
            }
            return targets;
        }
    }
}
