using Cysharp.Threading.Tasks;
using Game.Script.Manager;
using Game.Script.UI.Text.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Game.Script.Manager.GameManager;

namespace Game.Script.UI.Text.Presenter
{
    public class TextPresenter : MonoBehaviour
    {
        [SerializeField]
        PhaseTextView phaseTextView;
        [SerializeField]
        survivalTimeView survivalTimeView;
        [SerializeField]
        CountdownView countdownView;

        void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStarted += (max, cur) => PlayCountdown().Forget();
                GameManager.Instance.OnReturnToTitle += ResetAllViews;
            }

            if (RhythmManager.Instance != null)
            {
                RhythmManager.Instance.OnPhaseChanged += (phase) => phaseTextView.ShowPhasePopup(phase);
            }
        }
        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.currentState == GameState.Playing)
            {
                survivalTimeView.UpdateSurvivalTime(GameManager.Instance.playTime);
                if (survivalTimeView != null)
                {
                    survivalTimeView.gameObject.SetActive(true);
                }
            }
        }
        private async UniTaskVoid PlayCountdown()
        {
            await countdownView.PlayCountdownSequence();
        }

        private void ResetAllViews()
        {
            countdownView.HideCountdown();
            if (survivalTimeView != null)
            {
                survivalTimeView.gameObject.SetActive(false);
            }
        }
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnReturnToTitle -= ResetAllViews;
            }
        }
    }
}
