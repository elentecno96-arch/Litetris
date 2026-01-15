using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Game.Script.UI.Text.View
{
    public class CountdownView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countdownText;
        private CancellationTokenSource _uiCts;

        private void Awake()
        {
            HideCountdown();
        }
        
        public async UniTask PlayCountdownSequence()
        {
            if (countdownText == null) return;

            _uiCts?.Cancel();
            _uiCts?.Dispose();
            _uiCts = new CancellationTokenSource();

            string[] counts = { "3", "2", "1", "GO!" };
            countdownText.gameObject.SetActive(true);
            
            foreach (var text in counts)
            {
                countdownText.text = text;
                countdownText.alpha = 1;
                countdownText.transform.localScale = Vector3.one;

                //텍스트가 커지는 연출(DOScale)과 투명연출(DOFade)이 동시에 끝날 때 까지 대기
                await UniTask.WhenAll(
                    countdownText.transform.DOScale(2.0f, 0.5f).SetEase(Ease.OutExpo).ToUniTask(),
                    countdownText.DOFade(0, 0.5f).SetEase(Ease.InExpo).ToUniTask()
                );
            }
            HideCountdown();
        }
        public void HideCountdown() => countdownText.gameObject.SetActive(false);
    }
}
