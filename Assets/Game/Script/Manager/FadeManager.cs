using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Game.Script.Manager
{
    public class FadeManager : MonoBehaviour
    {
        public static FadeManager Instance { get; private set; }

        [SerializeField] private Image fadeImage;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }
        public async UniTask FadeIn(float duration)
        {
            if (fadeImage == null) return; // 방어 코드
            fadeImage.gameObject.SetActive(true);
            //SetUpdate(true) 게임 시간과 관계없이 독립적으로 작동
            //이게 없으면 일시정지 상태(Time.timeScale = 0)일 때 멈춰버림
            await fadeImage.DOFade(0f, duration).SetUpdate(true).ToUniTask();
            fadeImage.gameObject.SetActive(false);
        }
        public async UniTask FadeOut(float duration)
        {
            fadeImage.gameObject.SetActive(true);
            //To.UniTask : DOTween의 트윈을 UniTask로 변환
            //DOTween의 트윈 객체는 본래 await를 할 수 있는 타입이 아님
            //하지만 ToUniTask를 붙히면 트윈이 완료 되었는지 매 프레임 체크하는 UniTask를 만듬
            //트윈이 Complete되거나 Kill되면 UniTask가 await를 풀고 다음 줄로 넘겨줌
            await fadeImage.DOFade(1f, duration).SetUpdate(true).ToUniTask();
            //TweenCancelBehaviour : 트윈이 중간에 Kill이 되었을 때 어떻게 행동할지 정함 (끝낼건지, 에러낼건지 등)
            //CancellationToken도 연결 가능!
        }
    }
}