using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MiniManager : MonoBehaviour
{
    public static MiniManager Instance { get; private set; }

    [Tooltip("Jumlah item yang harus ditempatkan untuk menang.")]
    public int totalToPlace = 3;

    [Tooltip("Waktu limit dalam detik.")]
    public int timeLimit = 60;
    [SerializeField] public TextMeshProUGUI timerText;
    public GameObject winPanel;

    int placedCount = 0;
    float timeRemaining;
    bool finished = false;
    private bool isGameStarted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Initially disable all draggables
        DisableAllDraggables();
        
        // Set initial timer text with integer format
        if (timerText != null)
            timerText.text = $"{timeLimit}";
    }

    public void StartGame()
    {
        timeRemaining = timeLimit;
        isGameStarted = true;
        
        // Enable all draggables when game starts
        var draggables = FindObjectsOfType<DraggableUI>();
        foreach (var d in draggables) d.enabled = true;

        Debug.Log("Game Started!");
    }

    void Update()
    {
        if (!isGameStarted || finished) return;

        timeRemaining -= Time.deltaTime;
        // Convert to int to show only whole seconds
        timerText.text = $"{Mathf.CeilToInt(timeRemaining)}";

        if (timeRemaining <= 0f)
        {
            finished = true;
            Debug.Log("Waktu habis! Game Over.");
            DisableAllDraggables();
        }
    }

    public void RegisterSuccess()
    {
        if (finished) return;

        placedCount++;
        Debug.Log($"Placed {placedCount}/{totalToPlace}");

        if (placedCount >= totalToPlace)
        {
            finished = true;
            Debug.Log("Semua item terpasang! YOU WIN (debug).");
            if (winPanel != null)
                winPanel.SetActive(true);

            // ✅ Tambahkan koin ke global
            if (GlobalCoinManager.Instance != null)
            {
                GlobalCoinManager.Instance.AddCoins(20);
                Debug.Log("Player mendapatkan 20 koin!");
            }
        }
    }


    public void RestartGame()
    {
        // Reset timer and counters
        timeRemaining = timeLimit;
        placedCount = 0;
        finished = false;
        isGameStarted = false;

        // Reset all draggable items
        var draggables = FindObjectsOfType<DraggableUI>();
        foreach (var draggable in draggables)
        {
            draggable.enabled = false; // Start disabled
            draggable.gameObject.SetActive(true);
            draggable.ReturnToOriginal();
        }

        // Reset all drop zones
        var dropZones = FindObjectsOfType<DropZone>();
        foreach (var dropZone in dropZones)
        {
            if (dropZone.targetImage != null)
            {
                dropZone.targetImage.sprite = null;
            }
        }

        // Reset timer text
        if (timerText != null)
            timerText.text = $"{timeLimit}";

        Debug.Log("Game Restarted!");
    }

    void DisableAllDraggables()
    {
        var drags = FindObjectsOfType<DraggableUI>();
        foreach (var d in drags) d.enabled = false;
    }

    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
