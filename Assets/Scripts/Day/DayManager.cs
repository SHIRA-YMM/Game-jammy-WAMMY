using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class DayManager : MonoBehaviour
{
    [Header("Day settings")]
    [Tooltip("Total jumlah hari (misal 5).")]
    public int totalDays = 5;

    [Tooltip("Mulai dari hari ke berapa (1-based).")]
    public int startingDay = 1;

    [Header("Backgrounds")]
    [Tooltip("GameObject yang menampilkan background (SpriteRenderer atau UI Image).")]
    public GameObject backgroundObject;
    public GameObject dayanimText;
    public GameObject fadeinBG;
    

    [Tooltip("Sprite yang dipakai untuk mode pagi (sama setiap hari).")]
    public Sprite morningSprite;

    [Tooltip("Array sprite untuk tiap malam: index 0 -> night of day 1, index 1 -> night of day 2, ...")]
    public Sprite[] nightSprites;

    [Header("UI")]
    public TextMeshProUGUI dayText; // TMP Text in UI or worldspace

    [Header("Events")]
    public UnityEvent OnDayChanged; // dipanggil setiap ada perubahan day
    public UnityEvent OnNightStarted;
    public UnityEvent OnMorningStarted;

    // runtime
    [SerializeField, ReadOnly] private int currentDay = 1; // 1-based day
    [SerializeField, ReadOnly] private bool isMorning = true;

    // cached components
    private SpriteRenderer cachedSpriteRenderer;
    private Image cachedImage;

    void Awake()
    {
        // clamp starting day
        if (startingDay < 1) startingDay = 1;
        if (totalDays < 1) totalDays = 1;

        currentDay = Mathf.Clamp(startingDay, 1, totalDays);

        if (backgroundObject == null)
        {
            Debug.LogError("[DayManager] backgroundObject belum diassign!");
        }
        else
        {
            cachedSpriteRenderer = backgroundObject.GetComponent<SpriteRenderer>();
            cachedImage = backgroundObject.GetComponent<Image>();

            if (cachedSpriteRenderer == null && cachedImage == null)
            {
                Debug.LogWarning("[DayManager] backgroundObject tidak memiliki SpriteRenderer atau Image. Pastikan backgroundObject menampilkan sprite.");
            }
        }
    }

    void Start()
    {
        // default start: pagi
        StartMorning();
    }

    IEnumerator fadein()
    {
        // 1️⃣ Tampilkan fadeinBG lebih dulu
        dayanimText.SetActive(true);
        //dayanimText.SetActive(false);

        // tunggu 2 detik
        yield return new WaitForSeconds(1f);

        // 2️⃣ Setelah 2 detik, tampilkan teks
        
        //fadeinBG.SetActive(true);

        // tunggu lagi 2 detik
        yield return new WaitForSeconds(3f);

        // 3️⃣ Setelah total 4 detik, sembunyikan semua
        fadeinBG.SetActive(false);
        dayanimText.SetActive(false);
    }


    #region Public API

    /// <summary>
    /// Panggil ini untuk mulai mode pagi. (Sprite pagi adalah morningSprite, sama tiap hari)
    /// </summary>
    public void StartMorning()
    {
        StartCoroutine(fadein());
        isMorning = true;
        ApplyMorningBackground();
        UpdateDayText();
        OnMorningStarted?.Invoke();
        OnDayChanged?.Invoke();
    }

    /// <summary>
    /// Panggil ini untuk mulai mode malam. (Sprite malam berubah sesuai currentDay)
    /// </summary>
    public void StartNight()
    {
        isMorning = false;
        ApplyNightBackgroundForCurrentDay();
        UpdateDayText();
        OnNightStarted?.Invoke();
        OnDayChanged?.Invoke();
    }

    /// <summary>
    /// Naikkan hari (increase day by 1). Jika melebihi totalDays, akan dikembalikan ke totalDays dan mengembalikan false.
    /// Mengembalikan true jika berhasil naik hari.
    /// </summary>
    public bool NextDay()
    {
        if (currentDay >= totalDays)
        {
            currentDay = totalDays;
            UpdateDayText();
            return false; // sudah mencapai batas
        }

        currentDay++;
        UpdateDayText();
        OnDayChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Set day secara eksplisit (1-based). Akan diklem sesuai totalDays.
    /// </summary>
    public void SetDay(int day)
    {
        currentDay = Mathf.Clamp(day, 1, totalDays);
        UpdateDayText();
        OnDayChanged?.Invoke();
    }

    /// <summary>
    /// Ambil hari sekarang (1-based).
    /// </summary>
    public int GetCurrentDay() => currentDay;

    /// <summary>
    /// Cek apakah saat ini pagi.
    /// </summary>
    public bool IsMorning() => isMorning;

    #endregion

    #region Internal helpers

    private void ApplyMorningBackground()
    {
        if (morningSprite == null)
        {
            Debug.LogWarning("[DayManager] morningSprite belum diassign.");
            return;
        }

        if (cachedSpriteRenderer != null)
        {
            cachedSpriteRenderer.sprite = morningSprite;
        }
        else if (cachedImage != null)
        {
            cachedImage.sprite = morningSprite;
        }
    }

    private void ApplyNightBackgroundForCurrentDay()
    {
        if (nightSprites == null || nightSprites.Length == 0)
        {
            Debug.LogWarning("[DayManager] nightSprites belum diassign atau kosong.");
            return;
        }

        int idx = Mathf.Clamp(currentDay - 1, 0, nightSprites.Length - 1);
        Sprite nightSprite = nightSprites[idx];

        if (nightSprite == null)
        {
            Debug.LogWarning($"[DayManager] nightSprites[{idx}] null.");
            return;
        }

        if (cachedSpriteRenderer != null)
        {
            cachedSpriteRenderer.sprite = nightSprite;
        }
        else if (cachedImage != null)
        {
            cachedImage.sprite = nightSprite;
        }
    }

    private void UpdateDayText()
    {
        if (dayText == null) return;

        // contoh format: "Day 1 - Morning" atau dalam bahasa Indonesia: "Hari 1 (Pagi)"
        string mode = isMorning ? "Daylight" : "Night";
        dayText.text = $"Day {currentDay} ({mode})";
    }

    #endregion
}
