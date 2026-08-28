using UnityEngine;
using UnityEditor;

public class SetupBanana
{
    [MenuItem("Tools/Setup Banana")]
    public static void Setup()
    {
        // 1. Load sprites
        Sprite wholeBananaSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/banana2.png");
        Sprite headSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/banana_head.png");
        Sprite tailSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/banana_tail.png");

        if (wholeBananaSprite == null || headSprite == null || tailSprite == null)
        {
            Debug.LogError("Không tìm thấy hình ảnh quả chuối trong thư mục Assets/UI/");
            return;
        }

        // Tạo thư mục Prefabs nếu chưa có
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // 2. Tạo đối tượng SlicedBanana (Chuối bị cắt đôi)
        GameObject slicedBanana = new GameObject("SlicedBanana");
        
        GameObject head = new GameObject("BananaHead");
        head.transform.SetParent(slicedBanana.transform);
        head.transform.localPosition = new Vector3(-0.5f, 0, 0); 
        SpriteRenderer headSr = head.AddComponent<SpriteRenderer>();
        headSr.sprite = headSprite;
        headSr.sortingOrder = 5; // Hiển thị đè lên nền
        head.AddComponent<Rigidbody2D>();
        head.AddComponent<PolygonCollider2D>();

        GameObject tail = new GameObject("BananaTail");
        tail.transform.SetParent(slicedBanana.transform);
        tail.transform.localPosition = new Vector3(0.5f, 0, 0); 
        SpriteRenderer tailSr = tail.AddComponent<SpriteRenderer>();
        tailSr.sprite = tailSprite;
        tailSr.sortingOrder = 5; // Hiển thị đè lên nền
        tail.AddComponent<Rigidbody2D>();
        tail.AddComponent<PolygonCollider2D>();

        // Lưu thành Prefab
        GameObject slicedPrefab = PrefabUtility.SaveAsPrefabAsset(slicedBanana, "Assets/Prefabs/SlicedBanana.prefab");
        GameObject.DestroyImmediate(slicedBanana);

        // 3. Tạo đối tượng Banana (Chuối nguyên vẹn)
        GameObject banana = new GameObject("Banana");
        SpriteRenderer bananaSr = banana.AddComponent<SpriteRenderer>();
        bananaSr.sprite = wholeBananaSprite;
        bananaSr.sortingOrder = 5; // Hiển thị đè lên nền
        banana.AddComponent<Rigidbody2D>();
        
        CircleCollider2D col = banana.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f; // Thu nhỏ vòng va chạm để không bị chết yểu ngay khi vừa sinh ra

        Fruit fruitScript = banana.AddComponent<Fruit>();
        fruitScript.slicedFruitPrefab = slicedPrefab;
        fruitScript.scoreAmount = 2; // Chém chuối được 2 điểm

        // Lưu thành Prefab
        GameObject bananaPrefab = PrefabUtility.SaveAsPrefabAsset(banana, "Assets/Prefabs/Banana.prefab");
        GameObject.DestroyImmediate(banana);

        // 4. Tìm Spawner trong Scene hiện tại và gán vào
        Spawner spawner = GameObject.FindObjectOfType<Spawner>();
        if (spawner != null)
        {
            bool alreadyExists = false;
            if (spawner.fruitPrefabs != null)
            {
                foreach (var f in spawner.fruitPrefabs)
                {
                    if (f == bananaPrefab) alreadyExists = true;
                }
            }

            if (!alreadyExists)
            {
                if (spawner.fruitPrefabs == null)
                {
                    spawner.fruitPrefabs = new GameObject[] { bananaPrefab };
                }
                else
                {
                    GameObject[] newArray = new GameObject[spawner.fruitPrefabs.Length + 1];
                    for (int i = 0; i < spawner.fruitPrefabs.Length; i++)
                    {
                        newArray[i] = spawner.fruitPrefabs[i];
                    }
                    newArray[newArray.Length - 1] = bananaPrefab;
                    spawner.fruitPrefabs = newArray;
                }
                
                EditorUtility.SetDirty(spawner);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(spawner.gameObject.scene);
                Debug.Log("Đã tạo và thêm quả chuối vào Spawner thành công!");
            }
            else
            {
                Debug.Log("Quả chuối đã có sẵn trong Spawner.");
            }
        }
        else
        {
            Debug.LogWarning("Đã tạo Prefab quả chuối nhưng không tìm thấy Spawner trong màn hình hiện tại.");
        }
    }
}
