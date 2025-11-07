using UnityEngine;

[CreateAssetMenu(fileName = "DayData", menuName = "DeathLine/DayData")]
public class DayDataSO : ScriptableObject
{
    public int dayNumber = 1;
    [Tooltip("Background sprite for this day (siang/night depends on scene)")]
    public Sprite background;
    [Range(0f, 1f)]
    public float vibrantScale = 0.1f; // sesuai GDD: 10%,30%...
    [TextArea]
    public string notes;
}
