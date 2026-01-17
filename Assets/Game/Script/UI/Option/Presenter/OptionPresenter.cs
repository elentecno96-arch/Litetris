using Game.Script.Manager;
using Game.Script.UI.Option.SoundView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Script.UI.Option.Presenter
{
    public class OptionPresenter : MonoBehaviour
    {
        [SerializeField]
        private SoundOptionView soundOptionView;

        private void Start()
        {
            float bgm = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
            float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

            //뷰에 초기값 전달
            soundOptionView.UpdateSliders(bgm, sfx);

            SoundManager.Instance.SetBGMVolume(bgm);
            SoundManager.Instance.SetSFXVolume(sfx);

            soundOptionView.OnBGMChanged = (val) => {
                SoundManager.Instance.SetBGMVolume(val);
                PlayerPrefs.SetFloat("BGMVolume", val);
            };

            soundOptionView.OnSFXChanged = (val) =>
            {
                SoundManager.Instance.SetSFXVolume(val);
                PlayerPrefs.SetFloat("SFXVolume", val);
            };
        }
    }
}
