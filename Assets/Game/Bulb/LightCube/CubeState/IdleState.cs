using Game.Bulb.LightCube.Interface;
using UnityEngine;
using DG.Tweening;

namespace Game.Bulb.LightCube.CubeState
{
    public class IdleState : ICubeState
    {
        //시퀸스를 안쓴 이유
        //DOColor(cube.OriginalColor, 0.2f);,(Color.black, "_EmissionColor", 0.2f);는
        //동시에 일어나야 하기 때문 Join
        public void EnterState(LightCube cube)
        {
            //새로운 연출을 위해 초기화하는 과정
            //이전 트윈을 없애야 함
            cube.transform.DOKill();
            cube.Renderer.material.DOKill();
            //움직임이 있었다면 다시 원위치로
            cube.transform.localPosition = cube.OriginPosition;
            cube.IsDamageActive = false; //안전

            cube.Renderer.material.DOColor(cube.OriginalColor, 0.2f);
            cube.Renderer.material.DOColor(Color.black, "_EmissionColor", 0.2f);
        }

        public void Execute(LightCube cube) { }
        public void ExitState(LightCube cube) { }
    }
}