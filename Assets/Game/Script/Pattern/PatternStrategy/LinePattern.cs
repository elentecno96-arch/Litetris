using Game.Bulb.LightCube;
using Game.Script.Manager;
using Game.Pattern.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pattern.PatternStrategy
{
    public class LinePattern : IPatternStrategy
    {
        public List<LightCube> GetTargetCubes(int centerX, int centerY, int range)
        {
            List<LightCube> targets = new List<LightCube>();
            bool isHorizontal = Random.value > 0.5f;
            for (int i = 0; i < 13; i++)
            {
                if (isHorizontal) targets.Add(BoardManager.Instance.GetCube(i, centerY));
                else targets.Add(BoardManager.Instance.GetCube(centerX, i));
            }
            return targets;
        }
    }
}
