using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Bulb.LightCube;
using Game.Manager;
using UnityEngine;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveDuration = 0.15f;
        [SerializeField] private Ease moveEase = Ease.OutQuad;
        [SerializeField] private float jumpHeight = 1f;

        [Header("Status Settings")]
        [SerializeField] private float invincibilityDuration = 1.5f;

        private const string SND_MOVE = "MOVE";
        public int CurX { get; private set; }
        public int CurY { get; private set; }

        private bool isMoving = false;
        private bool isInvincible = false;
        private MeshRenderer meshRenderer;
        private Color originalColor;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            // 런타임에 생성된 머티리얼의 인스턴스를 저장하여 MissingReference 방지
            originalColor = meshRenderer.material.color;
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

            if (!isInvincible)
            {
                CheckTileHazard();
            }

            if (isMoving) return;
            HandleInput();
        }

        public void SetInitialPosition(int x, int y)
        {
            CurX = x;
            CurY = y;

            LightCube targetCube = BoardManager.Instance.GetCube(CurX, CurY);
            if (targetCube != null)
            {
                transform.position = new Vector3(targetCube.transform.position.x, 1f, targetCube.transform.position.z);
            }

            transform.localScale = Vector3.one * 2f;
            // DOKill()을 추가하여 중첩 트윈 방지
            transform.DOKill();
            transform.DOScale(Vector3.one * 2f, 0.5f).From(Vector3.zero).SetEase(Ease.OutBack);
        }

        private void HandleInput()
        {
            int horizontal = 0;
            int vertical = 0;

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) vertical = 1;
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) vertical = -1;
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) horizontal = -1;
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) horizontal = 1;

            if (horizontal != 0 || vertical != 0)
            {
                TryMove(horizontal, vertical);
            }
        }

        private void TryMove(int xDir, int yDir)
        {
            int nextX = CurX + xDir;
            int nextY = CurY + yDir;

            if (BoardManager.Instance.GetIndexFromCoord(nextX, nextY) != -1)
            {
                MoveToGrid(nextX, nextY).Forget();
            }
        }

        private async UniTaskVoid MoveToGrid(int nextX, int nextY)
        {
            isMoving = true;
            CurX = nextX;
            CurY = nextY;

            SoundManager.Instance.PlaySFX(SND_MOVE);

            LightCube targetCube = BoardManager.Instance.GetCube(CurX, CurY);
            Vector3 targetPos = new Vector3(targetCube.transform.position.x, 2f, targetCube.transform.position.z);

            // CancellationToken을 연결하여 파괴 시 즉시 중단
            await transform.DOJump(targetPos, jumpPower: jumpHeight, numJumps: 1, duration: moveDuration)
                .SetEase(moveEase)
                .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

            isMoving = false;
            CheckTileHazard();
        }

        public void CheckTileHazard()
        {
            if (isInvincible) return;

            LightCube currentCube = BoardManager.Instance.GetCube(CurX, CurY);
            if (currentCube != null && currentCube.IsDamageActive)
            {
                OnDamage().Forget();
            }
        }

        public async UniTaskVoid OnDamage()
        {
            if (isInvincible) return;

            GameManager.Instance.DecreaseHealth();
            UIManager.Instance.PlayDamageFlash();
            CameraManager.Instance.ShakeCamera();

            await StartInvincible(invincibilityDuration);
        }

        private async UniTask StartInvincible(float duration)
        {
            if (meshRenderer == null) return;

            isInvincible = true;

            // DOTween은 오브젝트 파괴 시 자동으로 Kill되지만, 
            // 안전하게 루프 트윈을 설정
            var blinkTween = meshRenderer.material.DOColor(Color.red, 0.1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear);

            try
            {
                // [핵심] 이 오브젝트가 파괴되면 이 Delay는 예외를 던지며 즉시 중단됩니다.
                await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: this.GetCancellationTokenOnDestroy());
            }
            catch (OperationCanceledException)
            {
                // 오브젝트가 파괴되어 취소된 경우, 아래 코드를 실행하지 않고 종료
                return;
            }

            // 파괴되지 않고 시간이 다 되었을 때만 실행
            if (blinkTween != null && blinkTween.IsActive()) blinkTween.Kill();
            if (meshRenderer != null) meshRenderer.material.color = originalColor;
            isInvincible = false;
        }

        private void OnDestroy()
        {
            // 혹시 남아있을지 모르는 모든 트윈 정리
            transform.DOKill();
            if (meshRenderer != null && meshRenderer.material != null)
            {
                meshRenderer.material.DOKill();
            }
        }
    }
}