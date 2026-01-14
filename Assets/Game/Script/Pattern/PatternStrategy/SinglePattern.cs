using Game.Bulb.LightCube;
using Game.Script.Manager;
using Game.Pattern.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pattern.PatternStrategy
{
    public class SinglePattern : IPatternStrategy
    {
        public List<LightCube> GetTargetCubes(int centerX, int centerY, int range)
        {
            List<LightCube> targets = new List<LightCube>();

            var cube = BoardManager.Instance.GetCube(centerX, centerY);
            if (cube != null) targets.Add(cube);

            return targets;
        }
    }
}
