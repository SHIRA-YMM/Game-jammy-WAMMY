using UnityEngine;
using System.Collections;

public class SaveGame : MonoBehaviour
{
    public GameObject textNotif;
    public bool hasSaved = false;  // Track if save point has been used

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasSaved)
        {
            // Panggil fungsi penyimpanan game di sini
            StartCoroutine(ShowNotification());
            hasSaved = true;  // Mark as used
            
            // Optional: Disable the collider after saving
            GetComponent<Collider2D>().enabled = false;
            
            Debug.Log("Game Saved!");
        }
    }
    
    IEnumerator ShowNotification()
    {
        textNotif.SetActive(true);
        yield return new WaitForSeconds(2f); // Tampilkan selama 2 detik
        textNotif.SetActive(false);
    }

    // Optional: Reset save point functionality
    public void ResetSavePoint()
    {
        hasSaved = false;
        GetComponent<Collider2D>().enabled = true;
    }
}
