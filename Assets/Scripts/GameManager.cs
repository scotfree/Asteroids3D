using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    float startTime;
    bool isGameOver;

    Text timerText;
    GameObject gameOverPanel;
    Text survivedText;

    void Awake()
    {
        Instance = this;
        BuildUI();
    }

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (!isGameOver)
            timerText.text = FormatTime(Time.time - startTime);
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        survivedText.text = $"You survived {FormatTime(Time.time - startTime)}";
        gameOverPanel.SetActive(true);

        // Disable ship input; it will keep drifting from existing velocity
        FindFirstObjectByType<ShipController>().enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    string FormatTime(float t)
    {
        int m = (int)(t / 60);
        int s = (int)(t % 60);
        return $"{m:00}:{s:00}";
    }

    void BuildUI()
    {
        var canvasGO = new GameObject("UI");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // Survival timer — top center
        timerText = MakeText(canvasGO, "00:00", 48);
        SetRect(timerText.gameObject, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -20), new Vector2(300, 70));

        // Game over panel — centered dark box
        gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(canvasGO.transform, false);
        gameOverPanel.AddComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        SetRect(gameOverPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(500, 280));

        var title = MakeText(gameOverPanel, "GAME OVER", 60);
        title.color = new Color(1f, 0.3f, 0.3f);
        title.alignment = TextAnchor.MiddleCenter;
        SetRect(title.gameObject, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -30), new Vector2(460, 90));

        survivedText = MakeText(gameOverPanel, "", 32);
        survivedText.alignment = TextAnchor.MiddleCenter;
        SetRect(survivedText.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 20), new Vector2(460, 55));

        // Restart button
        var btnGO = new GameObject("RestartButton");
        btnGO.transform.SetParent(gameOverPanel.transform, false);
        var btnImg = btnGO.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.55f, 0.2f);
        var btn = btnGO.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        btn.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        SetRect(btnGO, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 25), new Vector2(200, 55));

        var btnText = MakeText(btnGO, "RESTART", 28);
        btnText.alignment = TextAnchor.MiddleCenter;
        SetRect(btnText.gameObject, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        gameOverPanel.SetActive(false);
    }

    Text MakeText(GameObject parent, string content, int fontSize)
    {
        var go = new GameObject("Text");
        go.transform.SetParent(parent.transform, false);
        var t = go.AddComponent<Text>();
        t.text = content;
        t.fontSize = fontSize;
        t.color = Color.white;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return t;
    }

    void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size)
    {
        var r = go.GetComponent<RectTransform>();
        r.anchorMin = anchorMin;
        r.anchorMax = anchorMax;
        r.pivot = pivot;
        r.anchoredPosition = pos;
        r.sizeDelta = size;
    }
}
