using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 moveDirection = Vector3.right;
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool moveOnStart = true;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + moveDirection.normalized * moveDistance;

        if (!moveOnStart)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        // 使用PingPong函数创建来回移动
        float pingPong = Mathf.PingPong(Time.time * moveSpeed, 1);
        transform.position = Vector3.Lerp(startPosition, endPosition, pingPong);
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;

        Vector3 start = transform.position;
        Vector3 end = start + moveDirection.normalized * moveDistance;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(start, 0.2f);
        Gizmos.DrawSphere(end, 0.2f);
    }
}