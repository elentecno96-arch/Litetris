using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.Script.Component.UI.UIPanel;
using Game.Utility;

namespace Game.Script.Manager
{
    public class UIManager : Singleton<UIManager>
    {
        [Header("Main Menu Panels")]
        [SerializeField] private UIPanelEffect titlePanel;
        [SerializeField] private UIPanelEffect helpPanel;
        [SerializeField] private UIPanelEffect optionPanel;
        [SerializeField] private UIPanelEffect resultPanel;
        [SerializeField] private UIPanelEffect mobileControlPanel;
        public UIPanelEffect TitlePanel => titlePanel;
        [Header("Result & Countdown")]
        [SerializeField] private TextMeshProUGUI finalTimeText;
        protected override void Awake()
        {
            base.Awake();
            InitializeUI();
        }
        private void InitializeUI()
        {
            //기능 활용
            titlePanel.gameObject.SetActive(true);
            helpPanel.gameObject.SetActive(false);
            optionPanel.gameObject.SetActive(false);
            resultPanel.gameObject.SetActive(false);
            mobileControlPanel.gameObject.SetActive(false);
        }
        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver += ShowResult;
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            // 구독 해제
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameOver -= ShowResult;
        }
        public void CloseAllPopups()
        {
            helpPanel.Hide();
            optionPanel.Hide();
            resultPanel.Hide();
        }
        public void OnClickStart()
        {
            titlePanel.Hide();
            GameManager.Instance.OnClickStart();
        }
        public void ShowResult(float survivalTime)
        {
            finalTimeText.text = $"SURVIVED\n{survivalTime:F1}s";
            resultPanel.Show();
        }
        public void ShowTitleUI()
        {
            CloseAllPopups();
            titlePanel.Show();
        }
        public void OnTouchMove(string direction)
        {
            if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;
            if (GameManager.Instance.PlayerController != null)
            {
                GameManager.Instance.PlayerController.MoveByButton(direction);
            }
        }
        public void OnClickHelp() => helpPanel.Show();
        public void CloseHelp() => helpPanel.Hide();
        public void OnClickOption() => optionPanel.Show();
        public void CloseOption() => optionPanel.Hide();
        public void HideResultPanel() => resultPanel.Hide();
        public void ShowMoblieControll() => mobileControlPanel.Show();
        public void HideMoblieControll() => mobileControlPanel.Hide();

        public void OnClickRestart() => GameManager.Instance.OnClickRestart();

        public void OnClickTitle() => GameManager.Instance.OnClickGoToTitle();

        public void OnClickExit() => GameManager.Instance.QuitGame();
    }
}