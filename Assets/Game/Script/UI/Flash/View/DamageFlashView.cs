using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Flash.View
{
    public class DamageFlashView : MonoBehaviour
    {
        [SerializeField] private Image damageFlashImage;
        //매직 넘버 처리
        [SerializeField] private float flashDuration = 0.5f;
        [SerializeField] private float maxAlpha = 0.4f;
        [SerializeField] private Color flashColor = Color.red;

        public void PlayDamageFlash()
        {
            if (damageFlashImage == null) return;

            damageFlashImage.DOKill();
            damageFlashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, maxAlpha);
            damageFlashImage.DOFade(0f, flashDuration).SetEase(Ease.InQuad);
        }
    }
}
