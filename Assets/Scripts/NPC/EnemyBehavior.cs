using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float movementRange = 3f;
    [SerializeField] private float idleTime = 2f;

    [Header("References")]
    [SerializeField] private Animator animator; // hanya 1 animasi
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private GameObject ButtonDialog;

    private Vector3 startPosition;
    private float currentTarget;
    private int facing = 1; // 1 = kanan, -1 = kiri
    private bool canMove = true;
    private Transform playerTransform;
    private float idleTimer = 0f;
    private bool isIdle = false;

    void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        startPosition = transform.position;
        currentTarget = startPosition.x + movementRange;

        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    void Update()
    {
        if (!canMove)
        {
            FacePlayer();
            return;
        }

        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
                isIdle = false;
                idleTimer = 0f;
                SetNextTarget();
            }
            return;
        }

        MoveBetweenPoints();
    }

    private void MoveBetweenPoints()
    {
        float step = moveSpeed * Time.deltaTime;
        Vector3 currentPos = transform.position;

        if (Mathf.Abs(currentPos.x - currentTarget) < 0.1f)
        {
            isIdle = true;
            idleTimer = 0f;
            return;
        }

        float newX = Mathf.MoveTowards(currentPos.x, currentTarget, step);
        transform.position = new Vector3(newX, currentPos.y, currentPos.z);

        // Flip arah sprite saat bergerak
        facing = (newX > currentPos.x) ? 1 : -1;
        spriteRenderer.flipX = (facing == -1);
    }

    private void SetNextTarget()
    {
        float currentX = transform.position.x;
        if (Mathf.Abs(currentX - (startPosition.x + movementRange)) < 0.1f)
        {
            currentTarget = startPosition.x - movementRange;
            facing = -1;
        }
        else if (Mathf.Abs(currentX - (startPosition.x - movementRange)) < 0.1f)
        {
            currentTarget = startPosition.x + movementRange;
            facing = 1;
        }
        else
        {
            if (currentX > startPosition.x)
            {
                currentTarget = startPosition.x - movementRange;
                facing = -1;
            }
            else
            {
                currentTarget = startPosition.x + movementRange;
                facing = 1;
            }
        }

        spriteRenderer.flipX = (facing == -1);
    }

    private void FacePlayer()
    {
        if (playerTransform != null)
        {
            facing = (playerTransform.position.x > transform.position.x) ? 1 : -1;
            spriteRenderer.flipX = (facing == -1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canMove = false;
            playerTransform = other.transform;

            if (interactionUI != null)
                interactionUI.SetActive(true);

            if (ButtonDialog != null)
                ButtonDialog.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canMove = true;
            playerTransform = null;

            if (interactionUI != null)
                interactionUI.SetActive(false);

            if (ButtonDialog != null)
                ButtonDialog.SetActive(false);

            SetNextTarget();
        }
    }
}
