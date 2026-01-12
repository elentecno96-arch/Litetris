using DG.Tweening;
using Game.Bulb.LightCube.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Bulb.LightCube.CubeState
{
    public class WarningState : ICubeState
    {
        public void EnterState(LightCube cube)
        {
            // 노란색으로 빠르게 깜빡이며 위험 신호 알림
            cube.Renderer.material.DOColor(Color.yellow, 0.15f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetId(cube.gameObject.GetInstanceID());
        }
        public void Execute(LightCube cube) { }
        public void ExitState(LightCube cube)
        {
            cube.Renderer.material.DOKill();
        }
    }
}
