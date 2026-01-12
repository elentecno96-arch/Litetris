using Game.Bulb.LightCube;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Pattern.Interface
{
    public interface IPatternStrategy
    {
        List<LightCube> GetTargetCubes(int centerX, int centerY, int range);
    }
}
