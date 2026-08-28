using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;

    [Header("Spawn Settings")]
    public float minDelay = 0.5f;
    public float maxDelay = 1.5f;
    public float minAngle = -15f;
    public float maxAngle = 15f;
    public float minForce = 15f;
    public float maxForce = 22f;

    [Header("Bomb Settings")]
    [Range(0f, 1f)]
    public float bombChance = 0.1f; // 10% cơ hội sinh ra bom

    [Header("Difficulty Settings")]
    public float difficultyIncreaseRate = 0.05f; // Mỗi giây/điểm giảm delay bao nhiêu
    public float absoluteMinDelay = 0.2f; // Giới hạn delay nhỏ nhất
    public float absoluteMaxForce = 30f; // Giới hạn lực ném lớn nhất
    
    private float currentMinDelay;
    private float currentMaxDelay;
    private float currentMinForce;
    private float currentMaxForce;

    private Collider2D spawnArea;
    private bool isFirstSpawn = true;

    private void Awake()
    {
        spawnArea = GetComponent<Collider2D>();
        // Khởi tạo độ khó ban đầu
        currentMinDelay = minDelay;
        currentMaxDelay = maxDelay;
        currentMinForce = minForce;
        currentMaxForce = maxForce;
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine()
    {
        // Đợi một chút lúc đầu game
        yield return new WaitForSeconds(1f);

        while (true)
        {
            float delay = Random.Range(currentMinDelay, currentMaxDelay);
            yield return new WaitForSeconds(delay);
            Spawn();

            // TĂNG ĐỘ KHÓ: Giảm thời gian chờ giữa các lần phóng quả
            if (currentMinDelay > absoluteMinDelay)
            {
                currentMinDelay -= difficultyIncreaseRate;
                currentMaxDelay -= difficultyIncreaseRate;
            }

            // TĂNG ĐỘ KHÓ: Tăng lực ném để quả bay nhanh và cao hơn
            if (currentMaxForce < absoluteMaxForce)
            {
                currentMinForce += (difficultyIncreaseRate * 5);
                currentMaxForce += (difficultyIncreaseRate * 5);
            }

            // TĂNG ĐỘ KHÓ: Tăng tỷ lệ bom từ từ (tối đa 30%)
            if (bombChance < 0.3f)
            {
                bombChance += 0.005f; 
            }
        }
    }

    private void Spawn()
    {
        GameObject prefabToSpawn;
        
        // Luôn phóng quả cuối cùng (quả chuối) trong lần đầu tiên
        if (isFirstSpawn && fruitPrefabs.Length > 0)
        {
            prefabToSpawn = fruitPrefabs[fruitPrefabs.Length - 1]; 
            isFirstSpawn = false;
        }
        // Chọn ngẫu nhiên trái cây hoặc bom cho các lần sau
        else if (Random.value < bombChance && bombPrefab != null)
        {
            prefabToSpawn = bombPrefab;
        }
        else if (fruitPrefabs.Length > 0)
        {
            int index = Random.Range(0, fruitPrefabs.Length);
            prefabToSpawn = fruitPrefabs[index];
        }
        else
        {
            return;
        }

        // Tính toán vị trí sinh ngẫu nhiên trong spawnArea
        Vector3 spawnPos = new Vector3(
            Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            spawnArea.bounds.min.y,
            0f
        );

        // Góc ngẫu nhiên
        Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPos, rotation);

        // Áp dụng lực đẩy (Tùy thuộc vào Fruit.cs sẽ có Rigidbody)
        Fruit fruitScript = spawnedObject.GetComponent<Fruit>();
        if (fruitScript != null)
        {
            float force = Random.Range(currentMinForce, currentMaxForce);
            fruitScript.Launch(spawnedObject.transform.up, force);
        }
    }
}
