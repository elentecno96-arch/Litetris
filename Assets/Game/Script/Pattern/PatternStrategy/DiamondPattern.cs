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
            List<LightCube> targets = new List<LightCube>();

            for (int i = -range; i <= range; i++)
            {
                int remain = range - Mathf.Abs(i);
                targets.Add(BoardManager.Instance.GetCube(centerX + i, centerY + remain));
                if (remain != 0)
                    targets.Add(BoardManager.Instance.GetCube(centerX + i, centerY - remain));
            }
            return targets;
        }
    }
}