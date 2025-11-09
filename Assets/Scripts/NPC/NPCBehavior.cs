using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float movementRange = 3f;
    [SerializeField] private float idleTime = 2f; // Waktu untuk diam

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject interactionUI; // UI untuk interaksi
    [SerializeField] private GameObject ButtonDialog;


    private Vector3 startPosition;
    private float currentTarget;
    private int facing = 1; // 1 = right, -1 = left
    private bool canMove = true;
    private Transform playerTransform;
    private float idleTimer = 0f;
    private bool isIdle = false;

    private Vector3 lastPosition;

    void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        startPosition = transform.position;
        currentTarget = startPosition.x + movementRange;
        lastPosition = transform.position;

        // Pastikan UI interaksi tidak aktif saat mulai
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    void Update()
    {
        if (!canMove)
        {
            if (playerTransform != null)
            {
                facing = (playerTransform.position.x > transform.position.x) ? 1 : -1;
                UpdateAnimation(true, false);
            }
            lastPosition = transform.position;
            return;
        }

        // Handle idle state
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
                isIdle = false;
                idleTimer = 0f;
                SetNextTarget();
            }
            UpdateAnimation(true, false);
            lastPosition = transform.position;
            return;
        }

        // Logika pergerakan normal
        float step = moveSpeed * Time.deltaTime;
        Vector3 currentPos = transform.position;

        if (Mathf.Abs(currentPos.x - currentTarget) < 0.1f)
        {
            // Masuk ke idle state
            isIdle = true;
            idleTimer = 0f;
            SetNextTarget();
            UpdateAnimation(true, false);
            lastPosition = transform.position;
            return;
        }

        // Bergerak ke target jika tidak sedang idle
        bool movedThisFrame = false;
        float newX = Mathf.MoveTowards(currentPos.x, currentTarget, step);
        movedThisFrame = !Mathf.Approximately(newX, currentPos.x);

        if (movedThisFrame)
            transform.position = new Vector3(newX, currentPos.y, currentPos.z);

        // Update facing direction berdasarkan arah gerakan
        if (movedThisFrame)
        {
            facing = (newX > currentPos.x) ? 1 : -1;
        }

        UpdateAnimation(false, movedThisFrame);
        lastPosition = transform.position;
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
    }

    private void UpdateAnimation(bool forceIdle, bool actuallyMoving)
    {
        if (animator != null)
        {
            bool isMoving = !forceIdle && actuallyMoving;
            animator.SetBool("isMoving", isMoving);

            // Langsung set Direction ke facing (1 untuk right, -1 untuk left)
            // Untuk semua state: idle dan walk menggunakan Direction yang sama
            animator.SetInteger("Direction", facing);
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
           
            ButtonDialog.SetActive(true);

            //ButtonDialog.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canMove = true;
            playerTransform = null;

            Vector3 currentPos = transform.position;
            float distanceToTarget = Mathf.Abs(currentPos.x - currentTarget);

            if (distanceToTarget > movementRange * 2f ||
                (facing == 1 && currentTarget < currentPos.x) ||
                (facing == -1 && currentTarget > currentPos.x))
            {
                SetNextTarget();
            }

            if (interactionUI != null)
                interactionUI.SetActive(false);

                    //ButtonDialog.SetActive(false);

            ButtonDialog.SetActive(false);

        }
    }
}