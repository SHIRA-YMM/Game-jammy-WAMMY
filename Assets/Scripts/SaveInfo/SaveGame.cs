using UnityEngine;
using System.Collections;

public class SaveGame : MonoBehaviour
{
    public GameObject textNotif;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        int lastSavedDay = PlayerPrefs.GetInt("LastSavedDay", 0);
        int currentDay = GameProgressManager.Instance.GetCurrentDay();
        
        if (lastSavedDay != currentDay)
        {
            // Save current progress
            PlayerPrefs.SetInt("LastSavedDay", currentDay);
            StartCoroutine(ShowNotification());
            GetComponent<Collider2D>().enabled = false;
            
            // Save other game data here
            if (GlobalCoinManager.Instance != null)
                GlobalCoinManager.Instance.SaveCoins();
                
            PlayerPrefs.Save();
            Debug.Log($"Game Saved for Day {currentDay}!");
        }
    }
    
    IEnumerator ShowNotification()
    {
        textNotif.SetActive(true);
        yield return new WaitForSeconds(2f); // Tampilkan selama 2 detik
        textNotif.SetActive(false);
    }
}
