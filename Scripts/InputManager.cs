using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    // 移动输入事件 (x, y) - x为水平移动，y为垂直移动
    public UnityEvent<Vector2> OnMove = new UnityEvent<Vector2>();

    // 跳跃输入事件
    public UnityEvent OnJump = new UnityEvent();

    // 奔跑输入事件（按住Shift键）
    public UnityEvent<bool> OnRun = new UnityEvent<bool>();

    // 相机旋转输入
    public UnityEvent<Vector2> OnLook = new UnityEvent<Vector2>();

    // 输入敏感度设置
    [SerializeField] private float mouseSensitivity = 1.0f;

    private void Update()
    {
        //Debug.Log("Input Manager is Running");
        // 处理移动输入 (WASD/方向键)
        Vector2 moveInput = Vector2.zero;
        moveInput.x = Input.GetAxis("Horizontal"); // A/D 或 左/右方向键
        moveInput.y = Input.GetAxis("Vertical");   // W/S 或 上/下方向键

        // 只有当有实际输入时才触发事件
        if (moveInput.magnitude > 0.1f)
        {
            OnMove?.Invoke(moveInput);
        }
        else
        {
            OnMove?.Invoke(Vector2.zero);
        }

        // 处理跳跃输入 (空格键)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Debug");
            OnJump?.Invoke();
        }

        // 处理奔跑输入 (Shift键)
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            OnRun?.Invoke(true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            OnRun?.Invoke(false);
        }

        // 处理鼠标/相机输入
        Vector2 lookInput = new Vector2(
            Input.GetAxis("Mouse X") * mouseSensitivity,
            Input.GetAxis("Mouse Y") * mouseSensitivity
        );

        if (lookInput.magnitude > 0.1f)
        {
            OnLook?.Invoke(lookInput);
        }
    }

    private void Start()
    {
        // 锁定并隐藏鼠标光标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        // 当脚本禁用时，解锁光标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}