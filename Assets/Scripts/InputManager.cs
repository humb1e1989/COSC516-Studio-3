using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    // �ƶ������¼� (x, y) - xΪˮƽ�ƶ���yΪ��ֱ�ƶ�
    public UnityEvent<Vector2> OnMove = new UnityEvent<Vector2>();

    // ��Ծ�����¼�
    public UnityEvent OnJump = new UnityEvent();

    // ���������¼�����סShift����
    public UnityEvent<bool> OnRun = new UnityEvent<bool>();

    // �����ת����
    public UnityEvent<Vector2> OnLook = new UnityEvent<Vector2>();

    // �������ж�����
    [SerializeField] private float mouseSensitivity = 1.0f;

    private void Update()
    {
        //Debug.Log("Input Manager is Running");
        // �����ƶ����� (WASD/�����)
        Vector2 moveInput = Vector2.zero;
        moveInput.x = Input.GetAxis("Horizontal"); // A/D �� ��/�ҷ����
        moveInput.y = Input.GetAxis("Vertical");   // W/S �� ��/�·����

        // ֻ�е���ʵ������ʱ�Ŵ����¼�
        if (moveInput.magnitude > 0.1f)
        {
            OnMove?.Invoke(moveInput);
        }
        else
        {
            OnMove?.Invoke(Vector2.zero);
        }

        // ������Ծ���� (�ո��)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Debug");
            OnJump?.Invoke();
        }

        // ���������� (Shift��)
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            OnRun?.Invoke(true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            OnRun?.Invoke(false);
        }

        // �������/�������
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
        // ���������������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        // ���ű�����ʱ���������
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}