using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Bulb.LightCube.CubeState;
using Game.Bulb.LightCube.Interface;
using UnityEngine;

namespace Game.Bulb.LightCube
{
    public class LightCube : MonoBehaviour
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public MeshRenderer Renderer { get; private set; }
        public Color OriginalColor { get; private set; }
        public bool IsDamageActive { get; set; } = false;
        public Vector3 OriginPosition { get; private set; }

        private ICubeState currentState;
        private readonly ICubeState idleState = new IdleState();
        private readonly ICubeState warningState = new WarningState();
        private readonly ICubeState dangerState = new DangerState();

        private int _activePatternCount = 0; 

        public void Init(int x, int y)
        {
            X = x;
            Y = y;
            Renderer = GetComponent<MeshRenderer>();
            OriginalColor = Renderer.material.color;
            OriginPosition = transform.localPosition;
            ForceChangeState(idleState);
        }
        public void ChangeState(ICubeState newState)
        {
            int currentPriority = GetStatePriority(currentState);
            int newPriority = GetStatePriority(newState);

           
            if (currentPriority > newPriority) return;

            ExecuteStateChange(newState);
        }
        private void ForceChangeState(ICubeState newState)
        {
            ExecuteStateChange(newState);
        }

        private void ExecuteStateChange(ICubeState newState)
        {
            //현재 진행중인 애니메이션을 즉시 Complete지점으로 보낸 뒤 파괴
            //안그러면 색이 남음
            Renderer.material.DOKill(true);
            transform.DOKill(true);

            currentState?.ExitState(this);
            currentState = newState;
            currentState.EnterState(this);
        }

        private int GetStatePriority(ICubeState state)
        {
            //현재보다 낮은 단계 상태로는 절대 돌아갈 수 없게 함
            if (state is DangerState) return 2;
            if (state is WarningState) return 1;
            return 0; // IdleState
        }

        public void ResetCube()
        {
            _activePatternCount = 0;

            Renderer.material.DOKill();
            transform.DOKill();

            Renderer.material.color = OriginalColor;
            transform.position = OriginPosition;
            IsDamageActive = false;

            ForceChangeState(idleState);
        }

        //시퀀스(Sequence)
        //여러개의 트윈을 하나의 그룹으로 묶어 시간 순서대로 제어가능하게 하는 도구
        //애니메이션의 설계도가 있는 프레임 같은 느낌
        public async UniTask ActivateSequence(float warningTime, float dangerTime)
        {
            _activePatternCount++;

            ChangeState(warningState);
            await UniTask.Delay(System.TimeSpan.FromSeconds(warningTime));

            ChangeState(dangerState);
            await UniTask.Delay(System.TimeSpan.FromSeconds(dangerTime));

            _activePatternCount--;

            if (_activePatternCount <= 0)
            {
                _activePatternCount = 0;
                ForceChangeState(idleState);
            }
        }
        public void PlayTitleEffect(Color targetColor, float duration)
        {
            // 게임 중일 때는 실행 안 함
            if (_activePatternCount > 0) return;

            // 1. 색상 자체를 바꿈 (Emission이 아니더라도 보이게)
            Renderer.material.DOColor(targetColor, 0.8f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
        public void OnBeatBounce()
        {
            if (currentState is IdleState)
            {
                transform.DOPunchPosition(Vector3.up * 0.1f, 0.2f, 2, 0.5f);
            }
        }
    }
}