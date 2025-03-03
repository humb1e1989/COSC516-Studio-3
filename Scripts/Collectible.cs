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
        // ��¼��ʼλ���������¸�������
        startPosition = transform.position;
        // �����ʼbobʱ�䣬ʹ��ͬ��ҵ�bob������ͬ��
        bobTime = Random.Range(0f, 2f * Mathf.PI);
    }

    private void Update()
    {
        // ��ת���
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // ���¸�������
        bobTime += Time.deltaTime * bobFrequency;
        float yOffset = Mathf.Sin(bobTime) * bobAmplitude;
        transform.position = startPosition + new Vector3(0f, yOffset, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // �����ײ�������丸�����Ƿ���Player��ǩ
        if (other.CompareTag("Player") ||
            (other.transform.parent != null && other.transform.parent.CompareTag("Player")))
        {
            Debug.Log("Player or child of Player detected, collecting coin");
            Collect();
        }
    }

    private void Collect()
    {
        // �ҵ�GameManager�����ӷ���
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(pointValue);
        }

        // �����ռ���Ч������У�
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // ���ٽ��
        Destroy(gameObject);
    }

}