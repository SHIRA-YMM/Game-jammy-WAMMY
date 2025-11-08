using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI turnText; // "Enemy Turn" or "Bert's Turn"
    public TextMeshProUGUI infoText; // damage notif etc
    public GameObject skillNotFoundPopup; // optional small popup
    public Button attackButton, defendButton, skillButton;

    void Start()
    {
        if (turnText != null) turnText.gameObject.SetActive(false);
        if (infoText != null) infoText.gameObject.SetActive(false);
    }

    public void SetTurnText(string txt)
    {
        if (turnText == null) return;
        turnText.gameObject.SetActive(true);
        turnText.text = txt;
    }

    public void ShowInfo(string txt)
    {
        if (infoText == null) return;
        infoText.gameObject.SetActive(true);
        infoText.text = txt;
        StopAllCoroutines();
        StartCoroutine(InfoFade());
    }

    IEnumerator InfoFade()
    {
        yield return new WaitForSeconds(2f);
        if (infoText != null) infoText.gameObject.SetActive(false);
    }

    public void ShowSkillMissing()
    {
        if (skillNotFoundPopup != null)
        {
            skillNotFoundPopup.SetActive(true);
            StartCoroutine(HidePopup());
        }
    }

    IEnumerator HidePopup()
    {
        yield return new WaitForSeconds(1.2f);
        skillNotFoundPopup.SetActive(false);
    }

    // disable/enable the player action buttons
    public void SetActionButtonsEnabled(bool enabled, bool skillAvailable)
    {
        if (attackButton != null) attackButton.interactable = enabled;
        if (defendButton != null) defendButton.interactable = enabled;
        if (skillButton != null) skillButton.interactable = enabled && skillAvailable;
    }
}
