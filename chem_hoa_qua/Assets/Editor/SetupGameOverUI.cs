using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SetupGameOverUI
{
    [MenuItem("Tools/Setup Game Over UI")]
    public static void Setup()
    {
        // 1. Tìm Canvas
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Không tìm thấy Canvas trong Scene. Bạn phải có ít nhất 1 UI Canvas!");
            return;
        }

        // 2. Tải ảnh Game Over (hỗ trợ cả ảnh dạng Single và Multiple Sprite)
        Sprite gameOverSprite = null;
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath("Assets/UI/over_game.png");
        foreach (Object asset in assets)
        {
            if (asset is Sprite)
            {
                gameOverSprite = asset as Sprite;
                break; // Lấy sprite đầu tiên tìm thấy
            }
        }

        if (gameOverSprite == null)
        {
            Debug.LogError("Không tìm thấy ảnh tại Assets/UI/over_game.png. Hãy kiểm tra xem file đã được import dạng Sprite chưa.");
            return;
        }

        // 3. Tạo Panel nền (đen mờ che toàn màn hình)
        GameObject panelObj = new GameObject("GameOverPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImg = panelObj.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.7f); // Đen mờ 70%

        // 4. Tạo ảnh khung Game Over
        GameObject bgObj = new GameObject("GameOverImage");
        bgObj.transform.SetParent(panelObj.transform, false);
        
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(gameOverSprite.rect.width, gameOverSprite.rect.height);
        
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.sprite = gameOverSprite;

        // 5. Tạo Chữ hiển thị điểm số cuối cùng
        GameObject textObj = new GameObject("FinalScoreText");
        textObj.transform.SetParent(bgObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        // Tùy chỉnh vị trí theo hình ảnh thực tế (Ví dụ hạ xuống 20 pixel từ tâm)
        textRect.anchoredPosition = new Vector2(0, -20); 
        textRect.sizeDelta = new Vector2(300, 150);

        Text scoreText = textObj.AddComponent<Text>();
        scoreText.text = "0";
        scoreText.fontSize = 80;
        scoreText.alignment = TextAnchor.MiddleCenter;
        // Nếu ảnh là bảng gỗ sáng màu thì để màu trắng hoặc cam cho giống game thật
        scoreText.color = new Color(1f, 0.6f, 0f, 1f); // Màu cam vàng
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Thêm viền cho chữ (Outline) cho đẹp giống game gốc
        Outline outline = textObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        // 6. Tạo Nút Chơi Lại (Play Button)
        GameObject btnObj = new GameObject("PlayButton");
        btnObj.transform.SetParent(bgObj.transform, false);
        
        RectTransform btnRect = btnObj.AddComponent<RectTransform>();
        // Đặt xa xuống phía dưới cùng của cái khung
        btnRect.anchoredPosition = new Vector2(0, -180); 
        btnRect.sizeDelta = new Vector2(220, 70);

        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Nền xanh lá
        
        Button playBtn = btnObj.AddComponent<Button>();

        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        RectTransform btnTextRect = btnTextObj.AddComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.sizeDelta = Vector2.zero;

        Text btnText = btnTextObj.AddComponent<Text>();
        btnText.text = "CHƠI LẠI";
        btnText.fontSize = 35;
        btnText.fontStyle = FontStyle.Bold;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.white;
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 7. Gắn vào GameManager
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.gameOverPanel = panelObj;
            gm.finalScoreText = scoreText;

            // Tự động gán sự kiện OnClick gọi hàm RestartGame
            UnityEditor.Events.UnityEventTools.AddPersistentListener(playBtn.onClick, new UnityEngine.Events.UnityAction(gm.RestartGame));
            
            EditorUtility.SetDirty(gm);
        }

        // Ẩn panel mặc định khi mới bắt đầu (đã comment để hiển thị trong Editor)
        // panelObj.SetActive(false);
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
        Debug.Log("Tạo bảng Game Over thành công! Vui lòng lưu Scene.");
    }
}
