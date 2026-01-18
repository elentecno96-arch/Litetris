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
            //DOTween.To(() => cube.BaseColor, x => cube.BaseColor = x, Color.yellow, 0.15f)
            //                 .SetLoops(-1, LoopType.Yoyo)
            //                 .SetTarget(cube);

            Color targetBlue = new Color(0f, 0.5f, 1f);
            cube.EmissionColor = Color.black;

            DOTween.To(() => cube.EmissionColor, x => cube.EmissionColor = x, targetBlue, 0.4f)
                .SetEase(Ease.InOutQuad)
                .SetLoops(-1, LoopType.Yoyo)
                .SetTarget(cube);

            //// 노란색으로 빠르게 깜빡이며 위험 신호 알림
            //cube.Renderer.material.DOColor(Color.yellow, 0.15f)
            //    //루프 (-1은 무한 반복)
            //    .SetLoops(-1, LoopType.Yoyo)
            //    //각 큐브에 고유 번호를 붙혀 DOTween.Kill(ID)를 호출해
            //    //해당 큐브의 애니메이션만 종료 가능
            //    .SetId(cube.gameObject.GetInstanceID());
        }
        public void Execute(LightCube cube) { }
        public void ExitState(LightCube cube)
        {
            //SetLoops(-1, LoopType.Yoyo) 파괴
            //cube.Renderer.material.DOKill();
            DOTween.Kill(cube);
        }
    }
}
