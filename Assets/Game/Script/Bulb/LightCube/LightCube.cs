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
        //MaterialPropertyBlock
        //똑같은 머티리얼을 쓰는 수많은 오브젝트의 색상이나 속성만 각각 다르게 사용할 때
        //renderer.material.color = Color.red;이런 식으로 사용 시 색이 달라 새로운 객체가 생성됨
        //메모리 낭비, Batch처리 불가 등 성능 저하 유발
        //유니티 내부적으로는 Block에 담긴 값들을 GPU의 상수에 직접 매핑하기 때문에
        //성능 저하 없이 다양한 속성값을 오브젝트마다 다르게 줄 수 있음
        private MaterialPropertyBlock MaterialPropertyBlock;
        //Shader.PropertyToID는 문자열("_BaseColor")을 컴퓨터가
        //이해하기 쉬운 정수 번호로 바꾸는 작업
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private Color _currentBaseColor;
        private Color _currentEmissionColor;

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
            MaterialPropertyBlock = new MaterialPropertyBlock();
            //인스펙터 창에서 에미션을 체크 해도 최적화 과정에서 에미션 값이 0일 경우
            //GPU한테 전달할 때 연산 단계에서 생략해버리는 경우가 있다
            //MPB는 데이터를 전달 할 뿐 머테리얼의 기능을 켜거나 끄는 역할을 하지 않기 때문에
            //또는 런타일에 사용하지 않는 쉐이더 속성을 제거하려 한다
            //그러므로 강제적으로 머테리얼의 기능을 켜줘야 한다
            Renderer.sharedMaterial.EnableKeyword("_EMISSION");

            //material 대신 .sharedMaterial을 사용하여 원본 색상을 가져옵니다.
            //material을 쓰는 순간 배칭이 깨지기 시작함
            OriginalColor = Renderer.sharedMaterial.GetColor(BaseColorId);
            _currentBaseColor = OriginalColor;

            OriginPosition = transform.localPosition;
            ForceChangeState(idleState);
        }
        private void ApplyMPB()
        {
            //현재 렌더러의 블록 정보를 가져온다
            Renderer.GetPropertyBlock(MaterialPropertyBlock);
            //우리가 정한 색상 값을 블록에 쓴다
            MaterialPropertyBlock.SetColor(BaseColorId, _currentBaseColor);
            MaterialPropertyBlock.SetColor(EmissionColorId, _currentEmissionColor);
            //수정된 블록을 다시 렌더러에게 전달한다
            Renderer.SetPropertyBlock(MaterialPropertyBlock);
        }

        //외부(State 등)에서 트윈할 때 접근할 '입구'
        public Color BaseColor
        {
            get => _currentBaseColor;
            set { _currentBaseColor = value; ApplyMPB(); }
        }

        public Color EmissionColor
        {
            get => _currentEmissionColor;
            set { _currentEmissionColor = value; ApplyMPB(); }
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
            DOTween.Kill(this); //이 객체(LightCube)를 타겟으로 하는 모든 트윈 종료
            //Renderer.material.DOKill(true);
            transform.DOKill(this); //이 객체(LightCube)를 타겟으로 하는 트윈 종료

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

            //Renderer.material.DOKill();
            DOTween.Kill(this);
            //물리적 변화(transform) 트윈 종료 this => true
            transform.DOKill(true);

            //Renderer.material.color = OriginalColor;
            BaseColor = OriginalColor;
            EmissionColor = Color.black;

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
            //게임 중일 때는 실행 안 함
            if (_activePatternCount > 0) return;

            //색상 자체를 바꿈 (Emission이 아니더라도 보이게)
            //Renderer.material.DOColor(targetColor, 0.8f)
            //    .SetLoops(2, LoopType.Yoyo)
            //    .SetEase(Ease.InOutSine);

            //DOColor대신 DOTween.To 사용 
            DOTween.To(() => BaseColor, x => BaseColor = x, targetColor, duration * 0.5f)
                            .SetLoops(2, LoopType.Yoyo)
                            .SetEase(Ease.InOutSine)
                            .SetTarget(this);
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