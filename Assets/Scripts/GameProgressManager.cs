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

    public void StartGame()
    {
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
}