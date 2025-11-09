using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProgressManager : MonoBehaviour 
{
    public static GameProgressManager Instance { get; private set; }
    
    [System.Serializable]
    public class DayData {
        public string daySceneName;
        public string nightSceneName;
        public string combatSceneName;
        public DialogueData dayDialogue;
        public DialogueData nightDialogue;
    }

    public DayData[] daysData = new DayData[5]; // Configure in inspector
    public string introSceneName = "Intro";
    public string epilogueSceneName = "Epilogue";

    [SerializeField] private int currentDay = 1;
    [SerializeField] private bool isDaytime = true;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("GameProgressManager starting...");
        StartGame();
    }

    public void StartGame()
    {
        Debug.Log("Loading intro scene...");
        currentDay = 1;
        isDaytime = true;
        SaveProgress();
        SceneManager.LoadScene(introSceneName);
    }

    public void GoToNextPhase()
    {
        if (isDaytime)
        {
            // Day -> Night transition
            isDaytime = false;
            SceneManager.LoadScene(daysData[currentDay - 1].nightSceneName);
        }
        else
        {
            // Night -> Combat
            SceneManager.LoadScene(daysData[currentDay - 1].combatSceneName);
        }
        SaveProgress();
    }

    public void OnCombatComplete()
    {
        currentDay++;
        isDaytime = true;
        
        if (currentDay > 5)
        {
            SceneManager.LoadScene(epilogueSceneName);
            return;
        }

        SceneManager.LoadScene(daysData[currentDay - 1].daySceneName);
        SaveProgress();
    }

    public int GetCurrentDay() => currentDay;
    public bool IsDaytime() => isDaytime;

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentDay", currentDay);
        PlayerPrefs.SetInt("IsDaytime", isDaytime ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        isDaytime = PlayerPrefs.GetInt("IsDaytime", 1) == 1;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        if (scene.name.StartsWith("Day"))
        {
            // cari DayManager pada root objects dari scene yang baru dimuat
            var roots = scene.GetRootGameObjects();
            DayManager dayManager = null;
            foreach (var root in roots)
            {
                // cek root langsung
                dayManager = root.GetComponent<DayManager>();
                if (dayManager != null) break;

                // cek children jika perlu
                dayManager = root.GetComponentInChildren<DayManager>(true);
                if (dayManager != null) break;
            }

            if (dayManager != null)
            {
                Debug.Log("Found DayManager in loaded scene root objects");
                // cek references secara defensif
                if (dayManager.backgroundObject != null &&
                    dayManager.blackBG != null &&
                    dayManager.dayanimText != null &&
                    dayManager.fadeinBG != null)
                {
                    dayManager.SetDay(currentDay);
                    dayManager.StartMorning();
                }
                else
                {
                    Debug.LogError("DayManager references not properly set in scene! (inspector values missing at runtime)");
                }
            }
            else
            {
                Debug.LogError("DayManager not found in loaded Day scene (check GameManager name/placement).");
            }
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}