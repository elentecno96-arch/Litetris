using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Option.SoundView
{
    public class SoundOptionView : MonoBehaviour
    {
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        
        //Presenter한테 알려줄 이벤트
        public Action<float> OnBGMChanged;
        public Action<float> OnSFXChanged;

        private void Awake()
        {
            bgmSlider.onValueChanged.AddListener(val => OnBGMChanged?.Invoke(val));
            sfxSlider.onValueChanged.AddListener(val => OnSFXChanged?.Invoke(val));
        }
        public void UpdateSliders(float bgm, float sfx)
        {
            bgmSlider.value = bgm;
            sfxSlider.value = sfx;
        }
    }
}
