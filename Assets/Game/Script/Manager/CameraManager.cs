using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game.Script.Manager
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        [Header("Camera Settings")]
        [SerializeField] private UnityEngine.Camera mainCamera;
        [SerializeField] private float transitionDuration = 2.0f;
        [SerializeField] private Ease transitionEase = Ease.InOutQuart;

        private Vector3 initialPosition;
        private Quaternion initialRotation;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            if (mainCamera == null) mainCamera = UnityEngine.Camera.main;

            // 시작 시의 탑뷰 위치 및 회전 저장
            initialPosition = mainCamera.transform.position;
            initialRotation = mainCamera.transform.rotation;
        }

        public async UniTask TransitionToPlayView(Transform target)
        {
            if (target == null) return;

            await UniTask.WhenAll(
                mainCamera.transform.DOMove(target.position, transitionDuration)
                    .SetEase(transitionEase).ToUniTask(),
                mainCamera.transform.DORotateQuaternion(target.rotation, transitionDuration)
                    .SetEase(transitionEase).ToUniTask()
            );
        }

        public async UniTask TransitionToMainView()
        {
            //DOMove와 DORotate를 기다림
            await UniTask.WhenAll(
                mainCamera.transform.DOMove(initialPosition, transitionDuration)
                    .SetEase(transitionEase).ToUniTask(),
                mainCamera.transform.DORotateQuaternion(initialRotation, transitionDuration)
                    .SetEase(transitionEase).ToUniTask()
            );
        }

        public void BeatPulse(float strength = 0.2f, float duration = 0.1f)
        {
            //카메라의 화각을 순식간에 좁혔다가 Yoyo 되돌린다
            mainCamera.DOFieldOfView(mainCamera.fieldOfView - (strength * 10f), duration)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);
            //DOBlendableMoveBy 기존 위치에서 a만큼 더함 (중첩)
            mainCamera.transform.DOBlendableMoveBy(mainCamera.transform.forward * strength, duration)
                .SetLoops(2, LoopType.Yoyo);
        }
        //배경색을 순간적으로 바꿨다가 되돌림
        public void FlashBackgroundColor(Color flashColor, float duration = 0.2f)
        {
            Color originalColor = mainCamera.backgroundColor;
            mainCamera.DOColor(flashColor, duration)
                .OnComplete(() => mainCamera.DOColor(originalColor, duration));
        }
        public void ShakeCamera(float duration = 0.5f, float strength = 0.5f)
        {
            //DOShakePosition 무작위로 흔듬
            mainCamera.transform.DOShakePosition(duration, strength);
        }

        public void ResetToInitialView()
        {
            mainCamera.transform.position = initialPosition;
            mainCamera.transform.rotation = initialRotation;
        }
    }
}