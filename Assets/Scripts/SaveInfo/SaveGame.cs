using UnityEngine;
using System.Collections;

public class SaveGame : MonoBehaviour
{

    public GameObject textNotif;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Panggil fungsi penyimpanan game di sini
            StartCoroutine(ShowNotification());
            Debug.Log("Game Saved!");
            // Contoh: SaveSystem.SaveGame();
        }
    }
    
    IEnumerator ShowNotification()
    {
        textNotif.SetActive(true);
        yield return new WaitForSeconds(2f); // Tampilkan selama 2 detik
        textNotif.SetActive(false);
    }
}
