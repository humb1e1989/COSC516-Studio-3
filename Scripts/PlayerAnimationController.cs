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

    // ��ǰ����״̬
    private Vector3 targetScale = Vector3.one;
    private bool isGrounded = true;
    private bool isRunning = false;
    private bool isMoving = false;

    private void Start()
    {
        if (playerModel == null)
        {
            // ���ҵ�һ���Ӷ�����ΪplayerModel
            if (transform.childCount > 0)
            {
                playerModel = transform.GetChild(0);
                Debug.Log("Player Model not set, using first child.");
            }
            else
            {
                Debug.LogError("No Player Model found!");
                enabled = false; // ���ô˽ű�
                return;
            }
        }

        // ע�������¼�
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

        // ��ʼ����
        targetScale = new Vector3(1f, idleScale, 1f);
    }

    private void Update()
    {
        // ������״̬
        if (playerController != null)
        {
            // ע�⣺����Ҫ��PlayerController�����һ��������������ȡ����״̬
            // ����޷�ֱ�ӷ��ʣ�����ʹ�����߼�������������ȷ��
            // isGrounded = playerController.IsGrounded;
        }

        // ����Ŀ�����
        UpdateTargetScale();

        // ƽ��Ӧ�ñ����仯
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
            // ��Ծ���� - ���콺����
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
            // �ڿ���ʱ�ı���
            targetScale = new Vector3(jumpScaleXZ, jumpScaleY, jumpScaleXZ);
        }
        else if (isMoving)
        {
            if (isRunning)
            {
                // ����ʱ�ı���
                targetScale = new Vector3(1f, runScale, 1f);
            }
            else
            {
                // ����ʱ�ı���
                targetScale = new Vector3(1f, walkScale, 1f);
            }
        }
        else
        {
            // վ��ʱ�ı���
            targetScale = new Vector3(1f, idleScale, 1f);
        }
    }

    private System.Collections.IEnumerator JumpSquashAndStretch()
    {
        // ��Ծǰ�������¶�
        Vector3 squashScale = new Vector3(1.2f, 0.8f, 1.2f);
        float duration = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            playerModel.localScale = Vector3.Lerp(playerModel.localScale, squashScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ��Ծʱ������
        // ע�⣺��ֻ���Ӿ�Ч����ʵ����Ծ������PlayerController����
        // ʵ����Ծ��������Update��ͨ������״̬���������
    }
}