using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.Component.UI.UIButton
{
    public class UIButtonEffect : MonoBehaviour
    {
        [SerializeField]
        private float delay = 0.1f;
        [SerializeField]
        private float duration = 0.5f;
        [SerializeField]
        private float startScale = 0f;
        [SerializeField]
        private float targetScale = 3f;

        //UIManger에 있을 때는 UIManager에서 CanvasGroup을 캐싱했지만,
        //이제 각 버튼 컴포넌트를 개별적으로 관리하므로 여기서 캐싱하도록 변경

        //투명도 제어를 위한 CanvasGroup 캐싱
        private CanvasGroup _canvasGroup;

        private CanvasGroup MyCanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                {
                    _canvasGroup = GetComponent<CanvasGroup>();
                    if (_canvasGroup == null)
                        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                return _canvasGroup;
            }
        }

        private void OnEnable()
        {
            PlayShowAnimation();
        }

        public void PlayShowAnimation()
        {
            transform.DOKill();
            MyCanvasGroup.DOKill();

            transform.localScale = Vector3.one * startScale;
            MyCanvasGroup.alpha = 0f;

            transform.DOScale(Vector3.one * targetScale, duration)
                .SetDelay(delay)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);

            MyCanvasGroup.DOFade(1f, 0.4f)
                .SetDelay(delay)
                .SetUpdate(true);
        }
    }
}
