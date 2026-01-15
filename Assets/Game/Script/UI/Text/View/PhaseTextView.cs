using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Game.Script.UI.Text.View
{
    public class PhaseTextView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI phaseNoticeText;

        public void ShowPhasePopup(int phaseLevel)
        {
            if (phaseNoticeText == null) return;

            phaseNoticeText.DOKill(); //필수
            // private Sequence _phaseSeq; ( 시퀀스 변수를 만들어 관리 )
            // _phaseSeq?.Kill(); 실행 중인 시퀀스가 있다면 통째로 제거
            // 변수를 담아서 Kill하면 메모리 관리에도 안전
            phaseNoticeText.gameObject.SetActive(true);

            phaseNoticeText.rectTransform.anchoredPosition = new Vector2(0, 0f);
            phaseNoticeText.alpha = 0;
            phaseNoticeText.transform.localScale = Vector3.one * 0.8f;

            Sequence seq = DOTween.Sequence().SetUpdate(true);

            //Append 뒤에 덧붙이기 : 앞에 실행되고 있는 기능이 완전히 끝나고 난 뒤
            //Join 같이 시작 : Append된 기능과 동시에 실행함
            //Insert(시간, 애니메이션) 시퀸스가 시작되고 특정 시간이 지났을 때 애니메이션을 끼워 넣어라
            seq.Append(phaseNoticeText.rectTransform.DOAnchorPosY(100f, 0.5f).SetEase(Ease.OutBack));
            seq.Join(phaseNoticeText.DOFade(1, 0.4f));
            seq.Join(phaseNoticeText.transform.DOScale(1.2f, 0.5f));

            seq.AppendInterval(1f); //앞에 다 끝날 때 까지 대기

            seq.Append(phaseNoticeText.DOFade(0, 0.4f));
            seq.Join(phaseNoticeText.rectTransform.DOAnchorPosY(200f, 0.4f).SetEase(Ease.InBack));

            seq.OnComplete(() => phaseNoticeText.gameObject.SetActive(false));
        }
    }

}
