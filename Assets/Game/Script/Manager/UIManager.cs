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

        [Header("Health UI")]
        [SerializeField] private GameObject healthGroup;
        [SerializeField] private Image[] heartImages;
        [SerializeField] private Color normalHeartColor = Color.white;
        [SerializeField] private Color loseHeartColor = Color.gray;
        //[SerializeField] private Image damageFlashImage;

        [Header("Animations")]
        [SerializeField] private RectTransform[] mainButtons;
        [SerializeField] private float buttonPunchAmount = 0.15f;

        [Header("Timer & Status UI")]
        [SerializeField] private TextMeshProUGUI survivalTimeText;
        [SerializeField] private TextMeshProUGUI phaseNoticeText;

        [Header("Result & Countdown")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TextMeshProUGUI finalTimeText;
        [SerializeField] private TextMeshProUGUI countdownText;

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

            if (healthGroup != null) healthGroup.SetActive(false);
            HideCountdown();
        }

        private void Start()
        {
            if (RhythmManager.Instance != null)
            {
                RhythmManager.Instance.OnBeat += HandleHeartBeat;
            }
            bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            bgmSlider.onValueChanged.AddListener(val => {
                SoundManager.Instance.SetBGMVolume(val);
            });

            sfxSlider.onValueChanged.AddListener(val => {
                SoundManager.Instance.SetSFXVolume(val);
            });
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제 (중요!)
            // 널래퍼런스 너무 싫다
            if (RhythmManager.Instance != null)
            {
                RhythmManager.Instance.OnBeat -= HandleHeartBeat;
            }
            transform.DOKill();
            foreach (var heart in heartImages)
            {
                if (heart != null) heart.transform.DOKill();
            }
        }

        // 비트에 맞춰 하트가 뛰는 연출
        private void HandleHeartBeat(int beat)
        {
            foreach (var heart in heartImages)
            {
                if (heart != null && heart.color != loseHeartColor)
                {
                    heart.transform.DOKill();
                    heart.transform.DOPunchScale(Vector3.one * buttonPunchAmount, 0.1f);
                }
            }
        }

        #region UI 애니메이션 (Panel 제어)

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
        #endregion

        #region 버튼 이벤트 (GameManager와 연결)

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

        #endregion

        #region 인게임 UI 업데이트

        public void UpdateHealth(int currentHealth)
        {
            for (int i = 0; i < heartImages.Length; i++)
            {
                if (heartImages[i] == null) continue;
                heartImages[i].transform.DOKill();

                if (i < currentHealth)
                {
                    heartImages[i].DOColor(normalHeartColor, 0.3f);
                    heartImages[i].transform.DOScale(1.0f, 0.3f);
                }
                else
                {
                    if (heartImages[i].color != loseHeartColor)
                    {
                        heartImages[i].DOColor(loseHeartColor, 0.3f);
                        heartImages[i].transform.DOScale(0.7f, 0.3f);
                        heartImages[i].transform.DOShakePosition(0.4f, 10f, 20);
                    }
                }
            }
        }
        public void ResetHealthUI(int maxHealth)
        {
            if (heartImages == null) return;

            healthGroup.SetActive(true);
            healthGroup.transform.DOKill();
            healthGroup.transform.localScale = Vector3.one;

            for (int i = 0; i < heartImages.Length; i++)
            {
                if (heartImages[i] == null) continue;

                heartImages[i].transform.DOKill();
                heartImages[i].DOKill();

                heartImages[i].transform.localScale = Vector3.one;
                heartImages[i].transform.localPosition = Vector3.zero;

                if (i < maxHealth)
                {
                    heartImages[i].color = normalHeartColor;
                    heartImages[i].gameObject.SetActive(true);
                }
                else
                {
                    heartImages[i].gameObject.SetActive(false); 
                }
            }
        }

        public void SetActiveHealthUI(bool active)
        {
            if (healthGroup == null) return;
            healthGroup.transform.DOKill();

            if (active)
            {
                healthGroup.SetActive(true);
                healthGroup.transform.localScale = Vector3.zero;
                healthGroup.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
            }
            else
            {
                healthGroup.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetUpdate(true)
                    .OnComplete(() => healthGroup.SetActive(false));
            }
        }

        //DamageFlashView로 이동
        //public void PlayDamageFlash()
        //{
        //    if (damageFlashImage == null) return;
        //    damageFlashImage.DOKill();
        //    damageFlashImage.color = new Color(1, 0, 0, 0.4f);
        //    damageFlashImage.DOFade(0f, 0.5f).SetEase(Ease.InQuad);
        //}

        public void UpdateSurvivalTime(float time)
        {
            TimeSpan t = TimeSpan.FromSeconds(time);
            survivalTimeText.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }

        public void ShowResult(float survivalTime)
        {
            finalTimeText.text = $"SURVIVED\n{survivalTime:F1}s";
            OpenPanel(resultPanel);
        }

        public void HideResultPanel() => ClosePanel(resultPanel);

        public async UniTask PlayCountdownSequence()
        {
            string[] counts = { "3", "2", "1", "GO!" };
            countdownText.gameObject.SetActive(true);

            _uiCts = new CancellationTokenSource();

            foreach (var text in counts)
            {
                countdownText.text = text;
                countdownText.alpha = 1;
                countdownText.transform.localScale = Vector3.one;

                //텍스트가 커지는 연출(DOScale)과 투명연출(DOFade)이 동시에 끝날 때 까지 대기
                await UniTask.WhenAll(
                    countdownText.transform.DOScale(2.0f, 0.5f).SetEase(Ease.OutExpo).ToUniTask(),
                    countdownText.DOFade(0, 0.5f).SetEase(Ease.InExpo).ToUniTask()
                );
            }
            HideCountdown();
        }

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
        public void ShowTitleUI()
        {
            CloseAllPopups();
            if (healthGroup != null)
            {
                healthGroup.SetActive(false);
                healthGroup.transform.DOKill();
            }
            titlePanel.SetActive(true);
            titlePanel.transform.DOKill();
            titlePanel.transform.localScale = Vector3.one * 0.8f;
            titlePanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
            PlayTitleButtonAnimation();
        }
        public void HideHealthUI()
        {
            if (healthGroup == null) return;

            // 모든 하트 트윈 중지
            healthGroup.transform.DOKill();
            foreach (var heart in heartImages)
            {
                if (heart != null) heart.transform.DOKill();
            }

            // 애니메이션 없이 즉시 끄기 (가장 확실함)
            healthGroup.SetActive(false);
        }
        public void HideCountdown() => countdownText.gameObject.SetActive(false);
        #endregion
    }
}