using DG.Tweening;
using Game.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Camera
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera Instance { get; private set; }

        [Header("Target Settings")]
        [SerializeField] private Transform target; // 플레이어 트랜스폼
        [SerializeField] private Vector3 offset = new Vector3(0, 15, -10); // 13x13 맵을 내려다보는 위치
        [SerializeField] private float smoothSpeed = 5f;

        [Header("Rhythm Effects")]
        [SerializeField] private float bounceStrength = 0.2f;
        [SerializeField] private float shakeDuration = 0.1f;
        [SerializeField] private float shakeStrength = 0.3f;

        private Vector3 currentVelocity;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // 리듬 매니저의 비트에 카메라 연출 연결
            if (RhythmManager.Instance != null)
            {
                RhythmManager.Instance.OnBeat += OnBeatBounce;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // 플레이어를 부드럽게 추적 (Lerp)
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

            // 항상 플레이어를 바라보게 설정 (원할 경우 고정 각도 사용 가능)
            transform.LookAt(target.position + Vector3.up * 0.5f);
        }

        // 매 비트마다 카메라가 살짝 들썩이는 효과
        private void OnBeatBounce(int beatCount)
        {
            // 4비트(정박)마다 조금 더 강하게 튐
            float strength = (beatCount % 4 == 0) ? bounceStrength * 1.5f : bounceStrength;

            transform.DOComplete(); // 이전 트윈 강제 종료
            transform.DOPunchPosition(Vector3.down * strength, 0.1f);
        }

        // 플레이어가 대미지를 입었을 때 호출할 쉐이크 효과
        public void ShakeCamera()
        {
            transform.DOShakePosition(shakeDuration, shakeStrength);
        }
    }
}
