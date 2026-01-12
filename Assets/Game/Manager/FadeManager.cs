using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Game.Manager
{
    public class FadeManager : MonoBehaviour
    {
        public static FadeManager Instance { get; private set; }

        [SerializeField] private Image fadeImage;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }
        public async UniTask FadeIn(float duration)
        {
            if (fadeImage == null) return; // 방어 코드
            fadeImage.gameObject.SetActive(true);
            await fadeImage.DOFade(0f, duration).SetUpdate(true).ToUniTask();
            fadeImage.gameObject.SetActive(false);
        }
        public async UniTask FadeOut(float duration)
        {
            fadeImage.gameObject.SetActive(true);
            await fadeImage.DOFade(1f, duration).SetUpdate(true).ToUniTask();
        }
    }
}