using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Heart.View
{
    public class HeartView : MonoBehaviour
    {
        [SerializeField] private GameObject healthGroup;
        [SerializeField] private Image[] heartImages;
        [SerializeField] private Color normalHeartColor = Color.white;
        [SerializeField] private Color loseHeartColor = Color.gray;
        [SerializeField] private float punchAmount = 0.15f;

        public void UpdateHealth(int currentHealth)
        {
            for (int i = 0; i < heartImages.Length; i++)
            {
                if (heartImages[i] == null) continue;
                heartImages[i].transform.DOKill();

                if (i < currentHealth)
                {
                    heartImages[i].DOColor(normalHeartColor, 0.3f);
                    heartImages[i].transform.DOScale(1.0f, 0.3f);
                }
                else
                {
                    if (heartImages[i].color != loseHeartColor)
                    {
                        heartImages[i].DOColor(loseHeartColor, 0.3f);
                        heartImages[i].transform.DOScale(0.7f, 0.3f);
                        heartImages[i].transform.DOShakePosition(0.4f, 10f, 20);
                    }
                }
            }
        }
        public void Show(int maxHealth, int currentHealth)
        {

            for (int i = 0; i < heartImages.Length; i++)
            {
                if (i < maxHealth)
                {
                    heartImages[i].gameObject.SetActive(true);
                    heartImages[i].DOKill();
                    heartImages[i].color = (i < currentHealth) ? normalHeartColor : loseHeartColor;
                    heartImages[i].transform.localScale = (i < currentHealth) ? Vector3.one : Vector3.one * 0.7f;
                }
                else
                {
                    heartImages[i].gameObject.SetActive(false);
                }
            }

            healthGroup.SetActive(true);
            healthGroup.transform.DOKill();
            healthGroup.transform.localScale = Vector3.zero;
            healthGroup.transform.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }
        public void HandleHeartBeat(int beat)
        {
            foreach (var heart in heartImages)
            {
                // 살아있는 하트만 뜁니다.
                if (heart != null && heart.gameObject.activeSelf && heart.color == normalHeartColor)
                {
                    heart.transform.DOKill(true); // 완성 상태로 즉시 종료 후 새로 시작
                    heart.transform.DOPunchScale(Vector3.one * punchAmount, 0.1f);
                }
            }
        }
        public void HideHealthUI()
        {
            if (healthGroup == null) return;

            // 모든 하트 트윈 중지
            healthGroup.transform.DOKill();
            foreach (var heart in heartImages)
            {
                if (heart != null) heart.transform.DOKill();
            }

            // 애니메이션 없이 즉시 끄기 (가장 확실함)
            healthGroup.SetActive(false);
        }
    }
}
