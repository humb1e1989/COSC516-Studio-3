using UnityEngine;
using System.Collections.Generic;

public class PlatformGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform groundObject; // 地面对象
    [SerializeField] private GameObject basicPlatformPrefab; // 基本平台预制体
    [SerializeField] private GameObject coinPrefab; // 金币预制体

    [Header("Generation Settings")]
    [SerializeField] private int platformCount = 30; // 平台总数
    [SerializeField] private float minHeight = 1.5f; // 最低平台高度 (降低)
    [SerializeField] private float maxHeight = 10f; // 最高平台高度 (降低)
    [SerializeField] private float minPlatformSize = 3f; // 最小平台尺寸
    [SerializeField] private float maxPlatformSize = 8f; // 最大平台尺寸
    [SerializeField] private float minPlatformThickness = 0.5f; // 最小平台厚度
    [SerializeField] private float maxPlatformThickness = 1.5f; // 最大平台厚度
    [SerializeField] private float coveragePercentage = 0.6f; // 覆盖率 (0-1)

    [Header("Platform Types")]
    [SerializeField] private float movingPlatformChance = 0.3f; // 移动平台几率
    [SerializeField] private float rotatingPlatformChance = 0.2f; // 旋转平台几率

    [Header("Platform Colors")]
    [SerializeField] private Color basicPlatformColor = Color.magenta;
    [SerializeField] private Color movingPlatformColor = Color.blue;
    [SerializeField] private Color rotatingPlatformColor = Color.green;
    [SerializeField] private Color highPlatformColor = Color.yellow;

    private Bounds groundBounds;
    private List<Bounds> platformBounds = new List<Bounds>();

    private void Start()
    {
        // 获取地面边界
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
        // 创建一个空对象作为所有平台的父对象
        GameObject platformsParent = new GameObject("Platforms");

        float groundY = groundBounds.center.y + groundBounds.extents.y;
        float padding = 2f; // 边界内部的边距

        // 计算可用区域
        float groundArea = (groundBounds.size.x - padding * 2) * (groundBounds.size.z - padding * 2);
        float targetCoverage = groundArea * coveragePercentage;
        float currentCoverage = 0f;

        int attempts = 0;
        int maxAttempts = platformCount * 5; // 防止无限循环

        // 创建起始平台 - 更低的起始高度
        float startX = groundBounds.center.x - groundBounds.extents.x * 0.5f;
        float startZ = groundBounds.center.z - groundBounds.extents.z * 0.5f;

        GameObject startPlatform = CreateBasicPlatform(
            platformsParent,
            new Vector3(startX, groundY + 1.0f, startZ), // 降低起始平台高度
            new Vector3(5, 1, 5)
        );

        platformBounds.Add(GetPlatformBounds(startPlatform));
        currentCoverage += 5 * 5;

        int platformsCreated = 1;

        // 创建一些低高度平台作为起始区域
        for (int i = 0; i < 3; i++)
        {
            float x = startX + (i + 1) * 5;
            float z = startZ + Random.Range(-2f, 2f);
            float lowHeight = 1.0f + i * 0.5f; // 逐渐递增的低高度

            GameObject lowPlatform = CreateBasicPlatform(
                platformsParent,
                new Vector3(x, groundY + lowHeight, z),
                new Vector3(4, 0.8f, 4)
            );

            lowPlatform.tag = "Platform";
            platformBounds.Add(GetPlatformBounds(lowPlatform));
            platformsCreated++;
        }

        // 生成其余平台，直到达到目标覆盖率或平台数量
        while (platformsCreated < platformCount && currentCoverage < targetCoverage && attempts < maxAttempts)
        {
            attempts++;

            // 随机位置
            float xPos = Random.Range(groundBounds.min.x + padding, groundBounds.max.x - padding);
            float zPos = Random.Range(groundBounds.min.z + padding, groundBounds.max.z - padding);

            // 根据已生成平台数量确定高度，但整体降低
            float progress = (float)platformsCreated / platformCount;
            float height = Mathf.Lerp(minHeight, maxHeight, progress * progress); // 平方使高度增长更缓慢

            // 随机平台尺寸 - 矩形而非正方形
            float width = Random.Range(minPlatformSize, maxPlatformSize);
            float depth = Random.Range(minPlatformSize, maxPlatformSize);
            float thickness = Random.Range(minPlatformThickness, maxPlatformThickness);

            Vector3 position = new Vector3(xPos, groundY + height, zPos);
            Vector3 scale = new Vector3(width, thickness, depth);

            // 创建虚拟边界框进行碰撞检测
            Bounds newBounds = new Bounds(position, scale);

            // 检测是否与现有平台重叠
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

                // 决定平台类型
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

                // 设置平台标签为"Platform"，用于二段跳检测
                platform.tag = "Platform";

                platformBounds.Add(GetPlatformBounds(platform));
                currentCoverage += width * depth;
                platformsCreated++;

                // 随机在某些平台上添加金币
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

        // 为平台添加材质
        Renderer renderer = platform.GetComponent<Renderer>();
        if (renderer != null)
        {
            // 创建一个新的材质实例
            Material platformMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (platformMaterial == null)
            {
                // 如果URP着色器不可用，尝试标准着色器
                platformMaterial = new Material(Shader.Find("Standard"));
            }

            // 根据高度选择颜色
            if (position.y > groundBounds.center.y + groundBounds.extents.y + maxHeight * 0.7f)
            {
                platformMaterial.color = highPlatformColor;
            }
            else
            {
                platformMaterial.color = basicPlatformColor;
            }

            // 直接应用新材质到渲染器
            renderer.sharedMaterial = platformMaterial;
        }

        return platform;
    }

    private GameObject CreateMovingPlatform(GameObject parent, Vector3 position, Vector3 scale)
    {
        GameObject platform = Instantiate(basicPlatformPrefab, position, Quaternion.identity, parent.transform);
        platform.transform.localScale = scale;

        // 为平台添加材质
        Renderer renderer = platform.GetComponent<Renderer>();
        if (renderer != null)
        {
            // 创建一个新的材质实例
            Material platformMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (platformMaterial == null)
            {
                platformMaterial = new Material(Shader.Find("Standard"));
            }
            platformMaterial.color = movingPlatformColor;
            renderer.sharedMaterial = platformMaterial;
        }

        // 添加移动平台脚本
        MovingPlatform movingScript = platform.AddComponent<MovingPlatform>();

        // 随机移动方向
        Vector3[] possibleDirections = { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
        Vector3 moveDir = possibleDirections[Random.Range(0, possibleDirections.Length)];

        // 通过反射设置脚本属性
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

        // 为平台添加材质
        Renderer renderer = platform.GetComponent<Renderer>();
        if (renderer != null)
        {
            // 创建一个新的材质实例
            Material platformMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (platformMaterial == null)
            {
                platformMaterial = new Material(Shader.Find("Standard"));
            }
            platformMaterial.color = rotatingPlatformColor;
            renderer.sharedMaterial = platformMaterial;
        }

        // 添加旋转平台脚本
        RotatingPlatform rotatingScript = platform.AddComponent<RotatingPlatform>();

        // 通过反射设置脚本属性
        System.Type type = rotatingScript.GetType();
        System.Reflection.FieldInfo axisField = type.GetField("rotationAxis");
        if (axisField != null) axisField.SetValue(rotatingScript, Vector3.up);

        System.Reflection.FieldInfo speedField = type.GetField("rotationSpeed");
        // 这里修正错误：使用rotatingScript而不是movingScript
        if (speedField != null) speedField.SetValue(rotatingScript, Random.Range(10f, 40f));

        return platform;
    }

    private Bounds GetPlatformBounds(GameObject platform)
    {
        Bounds bounds = new Bounds(platform.transform.position, platform.transform.localScale);

        // 稍微扩大边界以确保平台间有空间
        bounds.Expand(0.5f);

        return bounds;
    }
}