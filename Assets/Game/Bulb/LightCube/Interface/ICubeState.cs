using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Bulb.LightCube.Interface
{
    public interface ICubeState
    {
        void EnterState(LightCube cube);
        void Execute(LightCube cube);
        void ExitState(LightCube cube);
    }
}
