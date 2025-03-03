using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int pointValue = 1;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float bobAmplitude = 0.2f;
    [SerializeField] private float bobFrequency = 1f;

    [Header("Effects")]
    [SerializeField] private AudioClip collectSound;

    private Vector3 startPosition;
    private float bobTime;

    private void Start()
    {
        // 记录初始位置用于上下浮动动画
        startPosition = transform.position;
        // 随机初始bob时间，使不同金币的bob动画不同步
        bobTime = Random.Range(0f, 2f * Mathf.PI);
    }

    private void Update()
    {
        // 旋转金币
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // 上下浮动动画
        bobTime += Time.deltaTime * bobFrequency;
        float yOffset = Mathf.Sin(bobTime) * bobAmplitude;
        transform.position = startPosition + new Vector3(0f, yOffset, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞体对象或其父对象是否有Player标签
        if (other.CompareTag("Player") ||
            (other.transform.parent != null && other.transform.parent.CompareTag("Player")))
        {
            Debug.Log("Player or child of Player detected, collecting coin");
            Collect();
        }
    }

    private void Collect()
    {
        // 找到GameManager并增加分数
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(pointValue);
        }

        // 播放收集音效（如果有）
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // 销毁金币
        Destroy(gameObject);
    }

}