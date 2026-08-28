using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Fruit Settings")]
    public GameObject slicedFruitPrefab;
    public GameObject explosionEffect; // Thêm biến chứa hiệu ứng nổ
    public int scoreAmount = 1;
    public bool isBomb = false;

    [Header("Physics Settings")]
    public float sliceForce = 5f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float force)
    {
        if (rb != null)
        {
            rb.AddForce(direction * force, ForceMode2D.Impulse);
            
            // Thêm chút mô-men xoắn (torque) để trái cây xoay lúc bay lên
            rb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem có va chạm với Lưỡi dao (Blade) không
        if (collision.CompareTag("Blade"))
        {
            Slice(collision.transform.position, (transform.position - collision.transform.position).normalized);
        }
        else if (collision.CompareTag("BottomBoundary"))
        {
            // Nếu rơi xuống qua cạnh dưới màn hình
            if (!isBomb)
            {
                GameManager.Instance.LoseLife();
            }
            Destroy(gameObject);
        }
    }

    private void Slice(Vector3 slicePos, Vector3 sliceDirection)
    {
        if (isBomb)
        {
            if (explosionEffect != null)
            {
                GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(explosion, 2f); // Xóa hiệu ứng sau 2 giây
            }
            GameManager.Instance.LoseLife();
        }
        else
        {
            GameManager.Instance.AddScore(scoreAmount);

            // Sinh ra trái cây bị cắt đôi
            if (slicedFruitPrefab != null)
            {
                GameObject sliced = Instantiate(slicedFruitPrefab, transform.position, transform.rotation);
                
                // Lấy tất cả Rigidbody2D của các nửa trái cây
                Rigidbody2D[] rbs = sliced.GetComponentsInChildren<Rigidbody2D>();

                foreach (Rigidbody2D slicedRb in rbs)
                {
                    // Lấy lại vận tốc của trái cây gốc
                    slicedRb.linearVelocity = rb.linearVelocity;
                    
                    // Thêm lực cắt để văng ra 2 bên
                    float randomForce = Random.Range(sliceForce - 1f, sliceForce + 1f);
                    // Dùng hướng cắt hoặc hướng ngẫu nhiên để 2 nửa văng ra
                    Vector2 pushDirection = (slicedRb.transform.position - transform.position).normalized;
                    slicedRb.AddForce(pushDirection * randomForce, ForceMode2D.Impulse);
                }

                // Xóa object bị cắt đôi sau 3 giây để tối ưu bộ nhớ
                Destroy(sliced, 3f);
            }
        }

        // Xóa trái cây nguyên vẹn
        Destroy(gameObject);
    }
}
