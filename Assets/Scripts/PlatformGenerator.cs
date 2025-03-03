using UnityEngine;
using System.Collections.Generic;

public class PlatformGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform groundObject; // �������
    [SerializeField] private GameObject basicPlatformPrefab; // ����ƽ̨Ԥ����
    [SerializeField] private GameObject coinPrefab; // ���Ԥ����

    [Header("Generation Settings")]
    [SerializeField] private int platformCount = 30; // ƽ̨����
    [SerializeField] private float minHeight = 1.5f; // ���ƽ̨�߶� (����)
    [SerializeField] private float maxHeight = 10f; // ���ƽ̨�߶� (����)
    [SerializeField] private float minPlatformSize = 3f; // ��Сƽ̨�ߴ�
    [SerializeField] private float maxPlatformSize = 8f; // ���ƽ̨�ߴ�
    [SerializeField] private float minPlatformThickness = 0.5f; // ��Сƽ̨���
    [SerializeField] private float maxPlatformThickness = 1.5f; // ���ƽ̨���
    [SerializeField] private float coveragePercentage = 0.6f; // ������ (0-1)

    [Header("Platform Types")]
    [SerializeField] private float movingPlatformChance = 0.3f; // �ƶ�ƽ̨����
    [SerializeField] private float rotatingPlatformChance = 0.2f; // ��תƽ̨����

    [Header("Platform Colors")]
    [SerializeField] private Color basicPlatformColor = Color.magenta;
    [SerializeField] private Color movingPlatformColor = Color.blue;
    [SerializeField] private Color rotatingPlatformColor = Color.green;
    [SerializeField] private Color highPlatformColor = Color.yellow;

    private Bounds groundBounds;
    private List<Bounds> platformBounds = new List<Bounds>();

    private void Start()
    {
        // ��ȡ����߽�
        if (groundObject != null)
        {
            Renderer groundRenderer = groundObject.GetComponent<Renderer>();
            if (groundRenderer != null)
            {
                groundBounds = groundRenderer.bounds;
                GeneratePlatforms();
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

    private void GeneratePlatforms()
    {
        // ����һ���ն�����Ϊ����ƽ̨�ĸ�����
        GameObject platformsParent = new GameObject("Platforms");

        float groundY = groundBounds.center.y + groundBounds.extents.y;
        float padding = 2f; // �߽��ڲ��ı߾�

        // �����������
        float groundArea = (groundBounds.size.x - padding * 2) * (groundBounds.size.z - padding * 2);
        float targetCoverage = groundArea * coveragePercentage;
        float currentCoverage = 0f;

        int attempts = 0;
        int maxAttempts = platformCount * 5; // ��ֹ����ѭ��

        // ������ʼƽ̨ - ���͵���ʼ�߶�
        float startX = groundBounds.center.x - groundBounds.extents.x * 0.5f;
        float startZ = groundBounds.center.z - groundBounds.extents.z * 0.5f;

        GameObject startPlatform = CreateBasicPlatform(
            platformsParent,
            new Vector3(startX, groundY + 1.0f, startZ), // ������ʼƽ̨�߶�
            new Vector3(5, 1, 5)
        );

        platformBounds.Add(GetPlatformBounds(startPlatform));
        currentCoverage += 5 * 5;

        int platformsCreated = 1;

        // ����һЩ�͸߶�ƽ̨��Ϊ��ʼ����
        for (int i = 0; i < 3; i++)
        {
            float x = startX + (i + 1) * 5;
            float z = startZ + Random.Range(-2f, 2f);
            float lowHeight = 1.0f + i * 0.5f; // �𽥵����ĵ͸߶�

            GameObject lowPlatform = CreateBasicPlatform(
                platformsParent,
                new Vector3(x, groundY + lowHeight, z),
                new Vector3(4, 0.8f, 4)
            );

            lowPlatform.tag = "Platform";
            platformBounds.Add(GetPlatformBounds(lowPlatform));
            platformsCreated++;
        }

        // ��������ƽ̨��ֱ���ﵽĿ�긲���ʻ�ƽ̨����
        while (platformsCreated < platformCount && currentCoverage < targetCoverage && attempts < maxAttempts)
        {
            attempts++;

            // ���λ��
            float xPos = Random.Range(groundBounds.min.x + padding, groundBounds.max.x - padding);
            float zPos = Random.Range(groundBounds.min.z + padding, groundBounds.max.z - padding);

            // ����������ƽ̨����ȷ���߶ȣ������彵��
            float progress = (float)platformsCreated / platformCount;
            float height = Mathf.Lerp(minHeight, maxHeight, progress * progress); // ƽ��ʹ�߶�����������

            // ���ƽ̨�ߴ� - ���ζ���������
            float width = Random.Range(minPlatformSize, maxPlatformSize);
            float depth = Random.Range(minPlatformSize, maxPlatformSize);
            float thickness = Random.Range(minPlatformThickness, maxPlatformThickness);

            Vector3 position = new Vector3(xPos, groundY + height, zPos);
            Vector3 scale = new Vector3(width, thickness, depth);

            // ��������߽�������ײ���
            Bounds newBounds = new Bounds(position, scale);

            // ����Ƿ�������ƽ̨�ص�
            bool overlaps = false;
            foreach (Bounds existingBounds in platformBounds)
            {
                if (newBounds.Intersects(existingBounds))
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                GameObject platform;

                // ����ƽ̨����
                float rand = Random.value;

                if (rand < movingPlatformChance)
                {
                    platform = CreateMovingPlatform(platformsParent, position, scale);
                }
                else if (rand < movingPlatformChance + rotatingPlatformChance)
                {
                    platform = CreateRotatingPlatform(platformsParent, position, scale);
                }
                else
                {
                    platform = CreateBasicPlatform(platformsParent, position, scale);
                }

                // ����ƽ̨��ǩΪ"Platform"�����ڶ��������
                platform.tag = "Platform";

                platformBounds.Add(GetPlatformBounds(platform));
                currentCoverage += width * depth;
                platformsCreated++;

                // �����ĳЩƽ̨����ӽ��
                if (Random.value < 0.4f && coinPrefab != null)
                {
                    Vector3 coinPosition = position + Vector3.up * 1.5f;
                    Instantiate(coinPrefab, coinPosition, Quaternion.identity);
                }
            }
        }

        Debug.Log($"Created {platformsCreated} platforms with {currentCoverage / groundArea * 100:F1}% ground coverage");
    }

    private GameObject CreateBasicPlatform(GameObject parent, Vector3 position, Vector3 scale)
    {
        GameObject platform = Instantiate(basicPlatformPrefab, position, Quaternion.identity, parent.transform);
        platform.transform.localScale = scale;

        // Ϊƽ̨��Ӳ���
        Renderer renderer = platform.GetComponent<Renderer>();
        if (renderer != null)
        {
            // ����һ���µĲ���ʵ��
            Material platformMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (platformMaterial == null)
            {
                // ���URP��ɫ�������ã����Ա�׼��ɫ��
                platformMaterial = new Material(Shader.Find("Standard"));
            }

            // ���ݸ߶�ѡ����ɫ
            if (position.y > groundBounds.center.y + groundBounds.extents.y + maxHeight * 0.7f)
            {
                platformMaterial.color = highPlatformColor;
            }
            else
            {
                platformMaterial.color = basicPlatformColor;
            }

            // ֱ��Ӧ���²��ʵ���Ⱦ��
            renderer.sharedMaterial = platformMaterial;
        }

        return platform;
    }

    private GameObject CreateMovingPlatform(GameObject parent, Vector3 position, Vector3 scale)
    {
        GameObject platform = Instantiate(basicPlatformPrefab, position, Quaternion.identity, parent.transform);
        platform.transform.localScale = scale;

        // Ϊƽ̨��Ӳ���
        Renderer renderer = platform.GetComponent<Renderer>();
        if (renderer != null)
        {
            // ����һ���µĲ���ʵ��
            Material platformMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (platformMaterial == null)
            {
                platformMaterial = new Material(Shader.Find("Standard"));
            }
            platformMaterial.color = movingPlatformColor;
            renderer.sharedMaterial = platformMaterial;
        }

        // ����ƶ�ƽ̨�ű�
        MovingPlatform movingScript = platform.AddComponent<MovingPlatform>();

        // ����ƶ�����
        Vector3[] possibleDirections = { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
        Vector3 moveDir = possibleDirections[Random.Range(0, possibleDirections.Length)];

        // ͨ���������ýű�����
        System.Type type = movingScript.GetType();
        System.Reflection.FieldInfo dirField = type.GetField("moveDirection");
        if (dirField != null) dirField.SetValue(movingScript, moveDir);

        System.Reflection.FieldInfo distField = type.GetField("moveDistance");
        if (distField != null) distField.SetValue(movingScript, Random.Range(3f, 8f));

        System.Reflection.FieldInfo speedField = type.GetField("moveSpeed");
        if (speedField != null) speedField.SetValue(movingScript, Random.Range(0.5f, 2f));

        return platform;
    }

    private GameObject CreateRotatingPlatform(GameObject parent, Vector3 position, Vector3 scale)
    {
        GameObject platform = Instantiate(basicPlatformPrefab, position, Quaternion.identity, parent.transform);
        platform.transform.localScale = scale;

        // Ϊƽ̨��Ӳ���
        Renderer renderer = platform.GetComponent<Renderer>();
        if (renderer != null)
        {
            // ����һ���µĲ���ʵ��
            Material platformMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (platformMaterial == null)
            {
                platformMaterial = new Material(Shader.Find("Standard"));
            }
            platformMaterial.color = rotatingPlatformColor;
            renderer.sharedMaterial = platformMaterial;
        }

        // �����תƽ̨�ű�
        RotatingPlatform rotatingScript = platform.AddComponent<RotatingPlatform>();

        // ͨ���������ýű�����
        System.Type type = rotatingScript.GetType();
        System.Reflection.FieldInfo axisField = type.GetField("rotationAxis");
        if (axisField != null) axisField.SetValue(rotatingScript, Vector3.up);

        System.Reflection.FieldInfo speedField = type.GetField("rotationSpeed");
        // ������������ʹ��rotatingScript������movingScript
        if (speedField != null) speedField.SetValue(rotatingScript, Random.Range(10f, 40f));

        return platform;
    }

    private Bounds GetPlatformBounds(GameObject platform)
    {
        Bounds bounds = new Bounds(platform.transform.position, platform.transform.localScale);

        // ��΢����߽���ȷ��ƽ̨���пռ�
        bounds.Expand(0.5f);

        return bounds;
    }
}