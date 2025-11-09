using UnityEngine;
using UnityEngine.UI;

public class UINPCWalker : MonoBehaviour
{
    [Header("Patrol Points")]
    public RectTransform pointA;  // Left point (UI Image)
    public RectTransform pointB;  // Right point (UI Image)

    [Header("Settings")]
    public float moveSpeed = 100f;
    public float waitTime = 2f;

    private RectTransform npcRect;
    private Image npcImage;
    private bool movingToB = true;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    void Start()
    {
        npcRect = GetComponent<RectTransform>();
        npcImage = GetComponent<Image>();

        // Start at point A
        if (pointA != null)
            npcRect.anchoredPosition = pointA.anchoredPosition;
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
                isWaiting = false;
            return;
        }

        // Determine target position
        Vector2 targetPos = movingToB ? pointB.anchoredPosition : pointA.anchoredPosition;

        // Move towards target
        npcRect.anchoredPosition = Vector2.MoveTowards(
            npcRect.anchoredPosition,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // Flip sprite based on direction
        if (npcImage != null)
        {
            npcImage.transform.localScale = new Vector3(
                movingToB ? 1f : -1f,  // Flip when going left
                1f,
                1f
            );
        }

        // Check if reached target
        if (Vector2.Distance(npcRect.anchoredPosition, targetPos) < 1f)
        {
            // Switch direction and wait
            movingToB = !movingToB;
            isWaiting = true;
            waitTimer = waitTime;
        }
    }
}