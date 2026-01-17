using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.Script.Component.UIPanel;

namespace Game.Script.Manager
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Main Menu Panels")]
        [SerializeField] private UIPanelEffect titlePanel;
        [SerializeField] private UIPanelEffect helpPanel;
        [SerializeField] private UIPanelEffect optionPanel;
        [SerializeField] private UIPanelEffect resultPanel;

        public UIPanelEffect TitlePanel => titlePanel;

        [Header("Animations")]
        [SerializeField] private RectTransform[] mainButtons;

        [Header("Result & Countdown")]
        [SerializeField] private TextMeshProUGUI finalTimeText;

        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            InitializeUI();
        }
        private void InitializeUI()
        {
            //기능 활용
            titlePanel.gameObject.SetActive(true);
            helpPanel.gameObject.SetActive(false);
            optionPanel.gameObject.SetActive(false);
            resultPanel.gameObject.SetActive(false);
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
        //public void OpenPanel(GameObject panel)
        //{
        //    if (panel == null) return;
        //    panel.SetActive(true);
        //    panel.transform.DOKill();
        //    panel.transform.localScale = Vector3.zero;
        //    panel.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
        //}

        //public void ClosePanel(GameObject panel)
        //{
        //    if (panel == null || !panel.activeSelf) return;
        //    panel.transform.DOKill();
        //    panel.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetUpdate(true)
        //        .OnComplete(() => panel.SetActive(false));
        //}
        public void CloseAllPopups()
        {
            //ClosePanel(helpPanel);
            //ClosePanel(optionPanel);
            //ClosePanel(resultPanel);
            helpPanel.Hide();
            optionPanel.Hide();
            resultPanel.Hide();
        }
        public void OnClickStart()
        {
            titlePanel.Hide();
            //ClosePanel(titlePanel);
            GameManager.Instance.OnClickStart();
        }

        public void OnClickHelp() => helpPanel.Show();//OpenPanel(helpPanel);
        public void CloseHelp() => helpPanel.Hide();//ClosePanel(helpPanel);
        public void OnClickOption() => optionPanel.Show();//OpenPanel(optionPanel);
        public void CloseOption() => optionPanel.Hide();//ClosePanel(optionPanel);
        public void HideResultPanel() => resultPanel.Hide();//ClosePanel(resultPanel);

        public void OnClickRestart() => GameManager.Instance.OnClickRestart();

        public void OnClickTitle() => GameManager.Instance.OnClickGoToTitle();

        public void OnClickExit() => GameManager.Instance.QuitGame();
        public void ShowResult(float survivalTime)
        {
            finalTimeText.text = $"SURVIVED\n{survivalTime:F1}s";
            //OpenPanel(resultPanel);
            resultPanel.Show();
        }

        
        public void ShowTitleUI()
        {
            CloseAllPopups();
            //titlePanel.SetActive(true);
            titlePanel.Show();
            //titlePanel.transform.DOKill();
            //titlePanel.transform.localScale = Vector3.one * 0.8f;
            //titlePanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
            PlayTitleButtonAnimation();
        }
    }
}