using Game.Script.Manager;
using Game.Script.UI.Heart.View;
using UnityEngine;

namespace Game.Script.UI.Heart.Presenter
{
    public class HeartPresenter : MonoBehaviour
    {
        [SerializeField] private HeartView HeartView;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnHealthChanged += OnHealthChanged;
                GameManager.Instance.OnGameStarted += OnGameStarted;
                GameManager.Instance.OnReturnToTitle += OnReturnToTitle;
            }

            if (RhythmManager.Instance != null)
            {
                RhythmManager.Instance.OnBeat += OnBeatHit;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnHealthChanged -= OnHealthChanged;
                GameManager.Instance.OnGameStarted -= OnGameStarted;
                GameManager.Instance.OnReturnToTitle -= OnReturnToTitle;
            }

            if (RhythmManager.Instance != null)
            {
                RhythmManager.Instance.OnBeat -= OnBeatHit;
            }
        }

        //메서드로 분리 (가독성과 해제의 용이성)
        private void OnGameStarted(int max, int current) => HeartView.Show(max, current);
        private void OnReturnToTitle() => HeartView.HideHealthUI();
        private void OnHealthChanged(int currentHealth) => HeartView.UpdateHealth(currentHealth);
        private void OnBeatHit(int beatCount) => HeartView.HandleHeartBeat(beatCount);
    }
}