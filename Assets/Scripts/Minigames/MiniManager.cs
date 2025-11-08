using UnityEngine;

public class MiniManager : MonoBehaviour
{
    public static MiniManager Instance { get; private set; }

    [Tooltip("Jumlah item yang harus ditempatkan untuk menang.")]
    public int totalToPlace = 3;

    [Tooltip("Waktu limit dalam detik.")]
    public float timeLimit = 30f;

    int placedCount = 0;
    float timeRemaining;
    bool finished = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        timeRemaining = timeLimit;
    }

    void Update()
    {
        if (finished) return;

        timeRemaining -= Time.deltaTime;
        // Optional: tampilkan timer di UI dengan Debug.Log tiap detik (bisa diubah)
        // Debug.Log($"Time left: {timeRemaining:F1}s");

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
        }
    }

    void DisableAllDraggables()
    {
        var drags = FindObjectsOfType<DraggableUI>();
        foreach (var d in drags) d.enabled = false;
    }
}
