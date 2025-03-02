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
    [SerializeField] private Transform groundObject; // ���Ground��������
    [SerializeField] private float spawnAreaPadding = 1.0f; // ��Ե���ף���ֹ��������ڵ����Ե
    [SerializeField] private int maxSpawnAttempts = 30; // ����Դ�������ֹ����ѭ��

    private int score = 0;
    private Renderer groundRenderer;
    private Bounds groundBounds;

    private void Start()
    {
        UpdateScoreDisplay();

        // ��ȡ����ı߽���Ϣ
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

        // ���Զ��Ѱ����Ч�����ɵ�
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            // �ڵ���߽������ѡ��һ���㣨���Ǳ�Ե���ף�
            float xPos = Random.Range(
                groundBounds.min.x + spawnAreaPadding,
                groundBounds.max.x - spawnAreaPadding
            );

            float zPos = Random.Range(
                groundBounds.min.z + spawnAreaPadding,
                groundBounds.max.z - spawnAreaPadding
            );

            // ����������������λ�õ��Ϸ�
            Vector3 rayStart = new Vector3(xPos, groundBounds.max.y + 2f, zPos);

            // ���·������߼�����
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit))
            {
                // ȷ�����߻��е��ǵ�����󣨿���ͨ����ǩ��Layer��飩
                if (hit.collider.gameObject == groundObject.gameObject ||
                    hit.collider.gameObject.CompareTag("Ground"))
                {
                    // �ڵ����Ϸ���������λ��
                    spawnPosition = hit.point + Vector3.up * coinHeightAboveGround;
                    return true;
                }
            }
        }

        Debug.LogWarning("Failed to find valid spawn position after maximum attempts");
        return false;
    }

    // �ڱ༭���п��ӻ���������
    private void OnDrawGizmos()
    {
        if (groundObject == null) return;

        Renderer renderer = groundObject.GetComponent<Renderer>();
        if (renderer == null) return;

        Bounds bounds = renderer.bounds;

        // ��ʾ�������򣨿��Ǳ�Ե���ף�
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Vector3 paddedSize = new Vector3(
            bounds.size.x - spawnAreaPadding * 2,
            bounds.size.y,
            bounds.size.z - spawnAreaPadding * 2
        );
        Gizmos.DrawCube(bounds.center, paddedSize);

        // ��ʾ�߽���
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bounds.center, paddedSize);
    }
}