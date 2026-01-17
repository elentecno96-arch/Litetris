using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Script.Component.UI.UIPanel
{
    public class UIPanelEffect : MonoBehaviour
    {
        private float openDuration = 0.4f;
        private float closeDuration = 0.3f;
        //Ease는 보통 Easing을 의미하며 값이 시간에 따라 어떻게 변화되는지 정의하는 함수
        //얼마나 빠르게 시작하고, 중간에 어떻게 변화하며, 끝날 때 어떻게 끝나는지 결정
        private Ease openEase = Ease.OutBack;
        private Ease closeEase = Ease.InBack;

        public void Show()
        {
            gameObject.SetActive(true);
            transform.DOKill(); //트윈 중지
            transform.localScale = Vector3.zero; //초기 설정
            transform.DOScale(Vector3.one, openDuration)
                     .SetEase(openEase)
                     .SetUpdate(true); //시간과 무관하게 재생
        }
        public void Hide()
        {
            transform.DOKill();

            transform.DOScale(Vector3.zero, closeDuration)
                     .SetEase(closeEase)
                     .SetUpdate(true)
                     .OnComplete(() => gameObject.SetActive(false));
        }
    }
}
