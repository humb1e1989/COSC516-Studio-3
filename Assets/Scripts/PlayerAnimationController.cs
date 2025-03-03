using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerModel;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InputManager inputManager;

    [Header("Animation Settings")]
    [SerializeField] private float idleScale = 1.0f;
    [SerializeField] private float walkScale = 0.95f;
    [SerializeField] private float runScale = 0.9f;
    [SerializeField] private float jumpScaleY = 1.2f;
    [SerializeField] private float jumpScaleXZ = 0.8f;
    [SerializeField] private float animationSmoothness = 5.0f;

    // 当前动画状态
    private Vector3 targetScale = Vector3.one;
    private bool isGrounded = true;
    private bool isRunning = false;
    private bool isMoving = false;

    private void Start()
    {
        if (playerModel == null)
        {
            // 查找第一个子对象作为playerModel
            if (transform.childCount > 0)
            {
                playerModel = transform.GetChild(0);
                Debug.Log("Player Model not set, using first child.");
            }
            else
            {
                Debug.LogError("No Player Model found!");
                enabled = false; // 禁用此脚本
                return;
            }
        }

        // 注册输入事件
        if (inputManager != null)
        {
            inputManager.OnMove.AddListener(HandleMoveInput);
            inputManager.OnJump.AddListener(HandleJumpInput);
            inputManager.OnRun.AddListener(HandleRunInput);
        }
        else
        {
            Debug.LogError("Input Manager not assigned to PlayerAnimationController!");
        }

        // 初始比例
        targetScale = new Vector3(1f, idleScale, 1f);
    }

    private void Update()
    {
        // 检查地面状态
        if (playerController != null)
        {
            // 注意：这需要在PlayerController中添加一个公共属性来获取地面状态
            // 如果无法直接访问，可以使用射线检测或其他方法来确定
            // isGrounded = playerController.IsGrounded;
        }

        // 更新目标比例
        UpdateTargetScale();

        // 平滑应用比例变化
        playerModel.localScale = Vector3.Lerp(playerModel.localScale, targetScale, Time.deltaTime * animationSmoothness);
    }

    private void HandleMoveInput(Vector2 input)
    {
        isMoving = input.magnitude > 0.1f;
    }

    private void HandleJumpInput()
    {
        if (isGrounded)
        {
            // 跳跃动画 - 拉伸胶囊体
            StartCoroutine(JumpSquashAndStretch());
        }
    }

    private void HandleRunInput(bool running)
    {
        isRunning = running;
    }

    private void UpdateTargetScale()
    {
        if (!isGrounded)
        {
            // 在空中时的比例
            targetScale = new Vector3(jumpScaleXZ, jumpScaleY, jumpScaleXZ);
        }
        else if (isMoving)
        {
            if (isRunning)
            {
                // 奔跑时的比例
                targetScale = new Vector3(1f, runScale, 1f);
            }
            else
            {
                // 行走时的比例
                targetScale = new Vector3(1f, walkScale, 1f);
            }
        }
        else
        {
            // 站立时的比例
            targetScale = new Vector3(1f, idleScale, 1f);
        }
    }

    private System.Collections.IEnumerator JumpSquashAndStretch()
    {
        // 跳跃前的蓄力下蹲
        Vector3 squashScale = new Vector3(1.2f, 0.8f, 1.2f);
        float duration = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            playerModel.localScale = Vector3.Lerp(playerModel.localScale, squashScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 跳跃时的拉伸
        // 注意：这只是视觉效果，实际跳跃物理由PlayerController处理
        // 实际跳跃动画会在Update中通过地面状态检测来处理
    }
}