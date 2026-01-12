using DG.Tweening;
using Game.Bulb.LightCube.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Bulb.LightCube.CubeState
{
    public class DangerState : ICubeState
    {
        public void EnterState(LightCube cube)
        {
            cube.IsDamageActive = true;
            cube.Renderer.material.SetColor("_EmissionColor", Color.red * 3f); // ºû³ª´Â »¡°£»ö
            cube.Renderer.material.DOColor(Color.red, 0.1f);
            cube.transform.DOPunchPosition(Vector3.up * 0.3f, 0.2f);
        }
        public void Execute(LightCube cube) { }
        public void ExitState(LightCube cube)
        {
            cube.IsDamageActive = false;
        }
    }
}
