using DG.Tweening;
using Game.Bulb.LightCube.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Bulb.LightCube.CubeState
{
    public class DangerState : ICubeState
    {
        private Tween emissionTween;
        public void EnterState(LightCube cube)
        {
            cube.IsDamageActive = true;

            //cube.Renderer.material.DOKill();

            float intensity = 2f;
            Color dangerRedHDR = Color.red * intensity;

            //EnableKeyword는 특정 머테리얼 개별 인스턴스의 기능을 키는 명령어
            //cube.Renderer.material.EnableKeyword("_EMISSION");
            //cube.Renderer.sharedMaterial.EnableKeyword("_EMISSION");

            //cube.Renderer.material.SetColor("_EmissionColor", Color.red * 8f);
            //cube.Renderer.material.DOColor(Color.red, "_BaseColor", 0.1f);
            //cube.transform.DOPunchPosition(Vector3.up * 0.3f, 0.2f);

            DOTween.To(() => cube.BaseColor, x => cube.BaseColor = x, Color.red, 0.1f)
                   .SetTarget(cube);
            DOTween.To(() => cube.EmissionColor, x => cube.EmissionColor = x, dangerRedHDR, 0.05f)
                   .SetTarget(cube);
            cube.transform.DOPunchPosition(Vector3.up * 0.3f, 0.2f);
        }
        public void Execute(LightCube cube) { }
        public void ExitState(LightCube cube)
        {
            cube.IsDamageActive = false; //피해 비활성화
        }
    }
}
