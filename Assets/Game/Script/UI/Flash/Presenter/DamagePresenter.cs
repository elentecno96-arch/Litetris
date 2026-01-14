using Game.Script.Manager;
using Game.Script.UI.Flash.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Script.UI.Flash.Presenter
{
    public class DamagePresenter : MonoBehaviour
    {
        [SerializeField]
        private DamageFlashView DamageFlashView;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerDamaged += OnDamageReceived;
            }
        }
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerDamaged -= OnDamageReceived;
            }
        }
        private void OnDamageReceived()
        {
            DamageFlashView.PlayDamageFlash();
        }
    }
}
