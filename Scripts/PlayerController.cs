using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Settings")]
    [SerializeField] private int maxJumpCount = 2;

    [Header("References")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Transform cameraTransform;

    // 组件引用
    private Rigidbody rb;
    private CapsuleCollider col;

    // 状态变量
    private Vector2 currentMoveInput;
    private bool isRunning = false;
    private bool isGrounded = true;
    private int jumpCount = 0;
    private Vector3 moveDirection = Vector3.zero;
    private bool canDoubleJump = false; // 新增：追踪是否可以二段跳

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        // 确保刚体设置正确
        rb.constraints = RigidbodyConstraints.FreezeRotation; // 冻结旋转以防角色倒下
    }

    private void Start()
    {
        // 确保有CapsuleCollider组件
        if (col == null)
        {
            col = gameObject.AddComponent<CapsuleCollider>();
            col.height = 2f;
            col.radius = 0.5f;
            Debug.LogWarning("No CapsuleCollider found, automatically added one");
        }

        // 如果未设置地面层，默认为"Default"层
        if (groundLayer == 0)
        {
            groundLayer = 1; // Default layer
            Debug.LogWarning("Ground Layer not set, using Default layer");
        }

        // 注册输入事件监听器
        if (inputManager == null)
        {
            Debug.LogError("Input Manager reference not set in PlayerController!");
            return;
        }

        inputManager.OnMove.AddListener(HandleMoveInput);
        inputManager.OnJump.AddListener(HandleJumpInput);
        inputManager.OnRun.AddListener(HandleRunInput);

        // 如果未设置相机引用，尝试使用主相机
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            Debug.Log("Camera Transform not set, using Main Camera");
        }
    }

    private void Update()
    {
        CheckGrounded();
        HandleRotation();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void HandleMoveInput(Vector2 input)
    {
        currentMoveInput = input;
    }

    private void HandleJumpInput()
    {
        // 第一次跳跃：如果在地面上
        if (isGrounded)
        {
            PerformJump();
            jumpCount = 1; // 设置为1表示已经进行了一次跳跃
            canDoubleJump = true; // 允许二段跳
            Debug.Log("First jump performed");
        }
        // 二段跳：如果已经跳过一次且可以二段跳
        else if (canDoubleJump && jumpCount < maxJumpCount)
        {
            PerformJump();
            jumpCount = maxJumpCount; // 防止进一步跳跃
            canDoubleJump = false; // 使用了二段跳能力
            Debug.Log("Second jump performed");
        }
    }

    private void PerformJump()
    {
        // 清除现有的垂直速度
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        // 应用跳跃力
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void HandleRunInput(bool running)
    {
        isRunning = running;
    }

    private void HandleRotation()
    {
        if (currentMoveInput.magnitude > 0.1f)
        {
            // 基于相机朝向转换移动方向
            float targetAngle = Mathf.Atan2(currentMoveInput.x, currentMoveInput.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            // 平滑旋转到目标方向
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void ApplyMovement()
    {
        if (currentMoveInput.magnitude > 0.1f)
        {
            // 基于相机朝向转换移动方向
            float targetAngle = Mathf.Atan2(currentMoveInput.x, currentMoveInput.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            // 确定当前速度
            float currentSpeed = isRunning ? runSpeed : walkSpeed;

            // 空中移动控制减弱
            float controlMultiplier = isGrounded ? 1f : 0.7f;

            // 应用移动
            Vector3 targetVelocity = moveDirection * currentSpeed * controlMultiplier;
            targetVelocity.y = rb.linearVelocity.y; // 保留垂直速度

            // 平滑过渡到目标速度
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, 0.25f);
        }
        else
        {
            // 停止水平移动，保留垂直速度
            Vector3 targetVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, 0.25f);
        }
    }

    private void CheckGrounded()
    {
        if (col == null)
        {
            col = GetComponent<CapsuleCollider>();
            if (col == null) return;
        }

        // 获取胶囊体底部的位置
        float capsuleHalfHeight = col.height * 0.5f;
        Vector3 checkPosition = transform.position - new Vector3(0, capsuleHalfHeight - 0.1f, 0);

        // 使用射线检测地面
        float rayDistance = groundCheckDistance;

        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(checkPosition, Vector3.down, rayDistance, groundLayer);

        // 如果刚刚着陆
        if (!wasGrounded && isGrounded)
        {
            jumpCount = 0; // 重置跳跃计数
            canDoubleJump = false; // 重置二段跳状态
            Debug.Log("Just landed - reset jump variables");
        }
    }

    // 提供一个公共属性以便其他脚本可以访问地面状态
    public bool IsGrounded
    {
        get { return isGrounded; }
    }

    // 可视化地面检测
    private void OnDrawGizmosSelected()
    {
        if (col == null) col = GetComponent<CapsuleCollider>();
        if (col == null) return;

        float capsuleHalfHeight = col.height * 0.5f;
        Vector3 checkPosition = transform.position - new Vector3(0, capsuleHalfHeight - 0.1f, 0);

        // 绘制地面检测射线
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(checkPosition, checkPosition + Vector3.down * groundCheckDistance);
    }
}