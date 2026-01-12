using Game.Bulb.LightCube.Interface;
using UnityEngine;
using DG.Tweening;

namespace Game.Bulb.LightCube.CubeState
{
    public class IdleState : ICubeState
    {
        public void EnterState(LightCube cube)
        {
            cube.transform.DOKill();
            cube.Renderer.material.DOKill();

            cube.transform.localPosition = cube.OriginPosition;
            cube.IsDamageActive = false;

            cube.Renderer.material.DOColor(cube.OriginalColor, 0.2f);
            cube.Renderer.material.DOColor(Color.black, "_EmissionColor", 0.2f);
        }

        public void Execute(LightCube cube) { }
        public void ExitState(LightCube cube) { }
    }
}