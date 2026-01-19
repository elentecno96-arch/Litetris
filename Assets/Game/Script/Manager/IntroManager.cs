using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Script.Manager
{
    public class IntroManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI introText;
        [SerializeField]
        private CanvasGroup maincanvasGroup;

        private string mainSceneName = "Game";
        private float fadeDuration = 1.5f;
        private float duration = 2.0f;

        void Start()
        {
            introText.alpha = 0f;
            StartIntroSequence().Forget();
        }

        private async UniTaskVoid StartIntroSequence()
        {
            try
            {
                //페이드인
                await introText.DOFade(1f, fadeDuration)
                    .SetEase(Ease.InSine)
                    .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

                //백 그라운드에서 메인 씬 비동기 시작
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainSceneName);
                asyncLoad.allowSceneActivation = false;

                float timer = 0f;

                //90퍼까지 씬 로드 
                while (timer < duration || asyncLoad.progress < 0.9f)
                {
                    timer += Time.deltaTime;

                    float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

                    await UniTask.Yield();
                }

                await UniTask.Delay(System.TimeSpan.FromSeconds(1f));

                await maincanvasGroup.DOFade(0f, fadeDuration)
                    .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

                asyncLoad.allowSceneActivation = true;
            }
            catch (OperationCanceledException)
            {
                UnityEngine.Debug.Log("Intro sequence was canceled.");
            }
        }
    }
}
