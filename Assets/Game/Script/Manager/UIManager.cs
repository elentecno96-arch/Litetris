using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Main Menu Panels")]
        [SerializeField] private GameObject titlePanel;
        [SerializeField] private GameObject helpPanel;
        [SerializeField] private GameObject optionPanel;
        public GameObject TitlePanel => titlePanel;

        [Header("Animations")]
        [SerializeField] private RectTransform[] mainButtons;

        [Header("Result & Countdown")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TextMeshProUGUI finalTimeText;

        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        private CancellationTokenSource _uiCts;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            InitializeUI();
        }
        private void InitializeUI()
        {
            titlePanel.SetActive(true);
            helpPanel.SetActive(false);
            optionPanel.SetActive(false);
            resultPanel.SetActive(false);
        }
        private void Start()
        {
            bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            bgmSlider.onValueChanged.AddListener(val => {
                SoundManager.Instance.SetBGMVolume(val);
            });

            sfxSlider.onValueChanged.AddListener(val => {
                SoundManager.Instance.SetSFXVolume(val);
            });
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver += ShowResult;
            }
        }
        private void OnDestroy()
        {
            // 구독 해제
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameOver -= ShowResult;
        }
        public void PlayTitleButtonAnimation()
        {
            if (mainButtons == null || mainButtons.Length == 0) return;

            for (int i = 0; i < mainButtons.Length; i++)
            {
                var button = mainButtons[i];
                if (button == null) continue;

                button.DOKill();

                CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
                }

                canvasGroup.DOKill();

                button.localScale = Vector3.zero;
                canvasGroup.alpha = 0;
                button.gameObject.SetActive(true);

                float delay = i * 0.15f;
                button.DOScale(Vector3.one * 3f, 0.5f)
                    .SetDelay(delay)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true);

                canvasGroup.DOFade(1f, 0.4f)
                    .SetDelay(delay)
                    .SetUpdate(true);
            }
        }
        public void OpenPanel(GameObject panel)
        {
            if (panel == null) return;
            panel.SetActive(true);
            panel.transform.DOKill();
            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
        }

        public void ClosePanel(GameObject panel)
        {
            if (panel == null || !panel.activeSelf) return;
            panel.transform.DOKill();
            panel.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetUpdate(true)
                .OnComplete(() => panel.SetActive(false));
        }
        public void CloseAllPopups()
        {
            ClosePanel(helpPanel);
            ClosePanel(optionPanel);
            ClosePanel(resultPanel);
        }
        public void OnClickStart()
        {
            ClosePanel(titlePanel);
            GameManager.Instance.OnClickStart();
        }

        public void OnClickHelp() => OpenPanel(helpPanel);
        public void CloseHelp() => ClosePanel(helpPanel);

        public void OnClickOption() => OpenPanel(optionPanel);
        public void CloseOption() => ClosePanel(optionPanel);

        public void OnClickRestart() => GameManager.Instance.OnClickRestart();

        public void OnClickTitle() => GameManager.Instance.OnClickGoToTitle();

        public void OnClickExit() => GameManager.Instance.QuitGame();
        public void ShowResult(float survivalTime)
        {
            finalTimeText.text = $"SURVIVED\n{survivalTime:F1}s";
            OpenPanel(resultPanel);
        }

        public void HideResultPanel() => ClosePanel(resultPanel);
        public void ShowTitleUI()
        {
            CloseAllPopups();
            titlePanel.SetActive(true);
            titlePanel.transform.DOKill();
            titlePanel.transform.localScale = Vector3.one * 0.8f;
            titlePanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
            PlayTitleButtonAnimation();
        }
    }
}