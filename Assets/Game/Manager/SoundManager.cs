using Game.Sound;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Manager
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer mainMixer;
        [SerializeField] private AudioMixerGroup sfxGroup;

        [Header("Sound Data")]
        [SerializeField] private List<SoundData> soundEffects;

        private Dictionary<string, AudioClip> sfxDictionary;
        private AudioSource sfxPlayer;

        // 상수 관리
        private const string BGM_VOL_PARAM = "BGMVolume";
        private const string SFX_VOL_PARAM = "SFXVolume";

        private void Awake()
        {
            sfxDictionary = new Dictionary<string, AudioClip>();
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            foreach (var data in soundEffects)
            {
                if (data.clip != null)
                    sfxDictionary[data.name] = data.clip;
            }

            sfxPlayer = gameObject.AddComponent<AudioSource>();
            sfxPlayer.outputAudioMixerGroup = sfxGroup;
            sfxPlayer.playOnAwake = false;
        }

        private void Start()
        {
            LoadVolumeSettings();
        }

        private void LoadVolumeSettings()
        {
            SetBGMVolume(PlayerPrefs.GetFloat(BGM_VOL_PARAM, 0.75f));
            SetSFXVolume(PlayerPrefs.GetFloat(SFX_VOL_PARAM, 0.75f));
        }

        // 볼륨 설정 로직 (UI에서 호출 가능하도록 public)
        public void SetBGMVolume(float volumeValue)
        {
            ApplyVolumeToMixer(BGM_VOL_PARAM, volumeValue);
            PlayerPrefs.SetFloat(BGM_VOL_PARAM, volumeValue);
        }

        public void SetSFXVolume(float volumeValue)
        {
            ApplyVolumeToMixer(SFX_VOL_PARAM, volumeValue);
            PlayerPrefs.SetFloat(SFX_VOL_PARAM, volumeValue);
        }

        private void ApplyVolumeToMixer(string parameterName, float value)
        {
            //인간의　귀는　소리의
            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            mainMixer.SetFloat(parameterName, dB);
            Debug.Log($"[Sound] 파라미터: {parameterName} | 값: {value} | dB: {dB} |");
        }
        
        public void PlaySFX(string name)
        {
            if (sfxDictionary.TryGetValue(name, out AudioClip clip))
            {
                sfxPlayer.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"[SoundManager] {name} 사운드를 찾을 수 없습니다.");
            }
        }
    }
}