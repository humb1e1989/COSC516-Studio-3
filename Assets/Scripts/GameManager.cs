using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Collectible Settings")]
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private int maxCollectibles = 10;
    [SerializeField] private float coinHeightAboveGround = 0.5f;

    [Header("Spawn Area")]
    [SerializeField] private Transform groundObject; // 你的Ground对象引用
    [SerializeField] private float spawnAreaPadding = 1.0f; // 边缘留白，防止金币生成在地面边缘
    [SerializeField] private int maxSpawnAttempts = 30; // 最大尝试次数，防止无限循环

    private int score = 0;
    private Renderer groundRenderer;
    private Bounds groundBounds;

    private void Start()
    {
        UpdateScoreDisplay();

        // 获取地面的边界信息
        if (groundObject != null)
        {
            groundRenderer = groundObject.GetComponent<Renderer>();
            if (groundRenderer != null)
            {
                groundBounds = groundRenderer.bounds;
                SpawnCollectibles();
            }
            else
            {
                Debug.LogError("Ground object does not have a Renderer component!");
            }
        }
        else
        {
            Debug.LogError("Ground object reference not set!");
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreDisplay();
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    private void SpawnCollectibles()
    {
        if (collectiblePrefab == null)
        {
            Debug.LogError("Collectible prefab is not assigned!");
            return;
        }

        Debug.Log($"Attempting to spawn {maxCollectibles} collectibles on ground");

        int spawnedCount = 0;

        for (int i = 0; i < maxCollectibles; i++)
        {
            if (TrySpawnCollectibleOnGround(out Vector3 spawnPosition))
            {
                Instantiate(collectiblePrefab, spawnPosition, Quaternion.identity);
                spawnedCount++;
                Debug.Log($"Spawned coin {spawnedCount} at {spawnPosition}");
            }
        }

        Debug.Log($"Successfully spawned {spawnedCount} collectibles");
    }

    private bool TrySpawnCollectibleOnGround(out Vector3 spawnPosition)
    {
        spawnPosition = Vector3.zero;

        // 尝试多次寻找有效的生成点
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            // 在地面边界内随机选择一个点（考虑边缘留白）
            float xPos = Random.Range(
                groundBounds.min.x + spawnAreaPadding,
                groundBounds.max.x - spawnAreaPadding
            );

            float zPos = Random.Range(
                groundBounds.min.z + spawnAreaPadding,
                groundBounds.max.z - spawnAreaPadding
            );

            // 设置射线起点在随机位置的上方
            Vector3 rayStart = new Vector3(xPos, groundBounds.max.y + 2f, zPos);

            // 向下发射射线检测地面
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit))
            {
                // 确认射线击中的是地面对象（可以通过标签或Layer检查）
                if (hit.collider.gameObject == groundObject.gameObject ||
                    hit.collider.gameObject.CompareTag("Ground"))
                {
                    // 在地面上方设置生成位置
                    spawnPosition = hit.point + Vector3.up * coinHeightAboveGround;
                    return true;
                }
            }
        }

        Debug.LogWarning("Failed to find valid spawn position after maximum attempts");
        return false;
    }

    // 在编辑器中可视化生成区域
    private void OnDrawGizmos()
    {
        if (groundObject == null) return;

        Renderer renderer = groundObject.GetComponent<Renderer>();
        if (renderer == null) return;

        Bounds bounds = renderer.bounds;

        // 显示生成区域（考虑边缘留白）
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Vector3 paddedSize = new Vector3(
            bounds.size.x - spawnAreaPadding * 2,
            bounds.size.y,
            bounds.size.z - spawnAreaPadding * 2
        );
        Gizmos.DrawCube(bounds.center, paddedSize);

        // 显示边界线
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bounds.center, paddedSize);
    }
}