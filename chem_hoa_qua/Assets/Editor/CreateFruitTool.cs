using UnityEngine;
using UnityEditor;
using System.Linq;

public class CreateFruitTool : EditorWindow
{
    [MenuItem("Tools/Fruit Setup/Tạo Prefab Quả Dứa (Pineapple)")]
    public static void CreatePineapplePrefab()
    {
        // Refresh AssetDatabase để chắc chắn nhận diện các ảnh
        AssetDatabase.Refresh();

        // 1. Tải ảnh từ thư mục
        Sprite fullSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/pinneapple.png");
        Sprite topSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/top_pineapple.png");
        Sprite bottomSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/bottom_pineapple.png");

        if (fullSprite == null || topSprite == null || bottomSprite == null)
        {
            Debug.LogError("Không tìm thấy một trong các Sprite quả dứa. Vui lòng kiểm tra lại thư mục Assets/UI và đảm bảo ảnh đã được set kiểu Sprite (2D and UI).");
            return;
        }

        // 2. Tạo Prefab cho quả dứa bị cắt đôi (SlicedPineapple)
        GameObject slicedObj = new GameObject("SlicedPineapple");
        
        GameObject topObj = new GameObject("TopHalf");
        topObj.transform.SetParent(slicedObj.transform);
        topObj.transform.localPosition = new Vector3(0, 0.5f, 0);
        SpriteRenderer topSr = topObj.AddComponent<SpriteRenderer>();
        topSr.sprite = topSprite;
        topSr.sortingOrder = 1;
        topObj.AddComponent<Rigidbody2D>().gravityScale = 2f;
        topObj.AddComponent<PolygonCollider2D>();

        GameObject bottomObj = new GameObject("BottomHalf");
        bottomObj.transform.SetParent(slicedObj.transform);
        bottomObj.transform.localPosition = new Vector3(0, -0.5f, 0);
        SpriteRenderer botSr = bottomObj.AddComponent<SpriteRenderer>();
        botSr.sprite = bottomSprite;
        botSr.sortingOrder = 1;
        bottomObj.AddComponent<Rigidbody2D>().gravityScale = 2f;
        bottomObj.AddComponent<PolygonCollider2D>();

        string slicedPrefabPath = "Assets/Prefabs/SlicedPineapple.prefab";
        GameObject savedSlicedPrefab = PrefabUtility.SaveAsPrefabAsset(slicedObj, slicedPrefabPath);
        DestroyImmediate(slicedObj);

        // 3. Tạo Prefab cho quả dứa nguyên vẹn (Pineapple)
        GameObject fullObj = new GameObject("Pineapple");
        fullObj.layer = LayerMask.NameToLayer("Default");
        
        SpriteRenderer sr = fullObj.AddComponent<SpriteRenderer>();
        sr.sprite = fullSprite;
        sr.sortingOrder = 1;
        
        Rigidbody2D rb = fullObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1.5f;
        
        CircleCollider2D col = fullObj.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.6f;

        Fruit fruitScript = fullObj.AddComponent<Fruit>();
        fruitScript.slicedFruitPrefab = savedSlicedPrefab; // Nối Prefab cắt đôi vào
        fruitScript.scoreAmount = 1;
        fruitScript.sliceForce = 5f;
        fruitScript.isBomb = false;

        string fullPrefabPath = "Assets/Prefabs/Pineapple.prefab";
        GameObject savedFullPrefab = PrefabUtility.SaveAsPrefabAsset(fullObj, fullPrefabPath);
        DestroyImmediate(fullObj);

        // 4. Tìm Spawner trong màn hình và thêm quả dứa vào mảng mồi (Spawning)
        Spawner spawner = Object.FindAnyObjectByType<Spawner>();
        if (spawner != null)
        {
            var list = spawner.fruitPrefabs.ToList();
            if (!list.Contains(savedFullPrefab))
            {
                list.Add(savedFullPrefab);
                spawner.fruitPrefabs = list.ToArray();
                EditorUtility.SetDirty(spawner); // Lưu lại thay đổi trên Spawner
            }
            Debug.Log("✅ Đã tạo thành công Prefab Pineapple & SlicedPineapple và tự động thêm vào Spawner!");
        }
        else
        {
            Debug.LogWarning("⚠️ Đã tạo Prefab thành công, nhưng không tìm thấy Spawner trong Scene hiện tại.");
        }
    }
}
