using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    [Tooltip("berapa kecil input yang dianggap diam")]
    public float movementThreshold = 0.1f;

    [Header("References")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer; // optional, untuk flip sprite

    // ----- generated input actions (ganti nama jika berbeda) -----
    PlayerController inputActions; // <-- contoh nama generated class

    // internal
    Vector2 moveInput = Vector2.zero;
    int facing = 1; // 1 = right, -1 = left

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // inisialisasi input actions
        inputActions = new PlayerController();

        // subscribe ke action Move
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled  += ctx => moveInput = Vector2.zero;
    }

    void OnEnable()
    {
        inputActions?.Enable();
    }

    void OnDisable()
    {
        inputActions?.Disable();
    }

    void FixedUpdate()
    {
        float hx = moveInput.x;

        // movement (hanya horizontal)
        rb.linearVelocity = new Vector2(hx * moveSpeed, rb.linearVelocity.y);

        // update facing direction sesuai arah gerakan
        if (Mathf.Abs(hx) > movementThreshold)
        {
            // Mengubah logika facing:
            // Jalan ke kiri (hx < 0) = facing -1 (untuk animasi _left)
            // Jalan ke kanan (hx > 0) = facing 1 (untuk animasi _right)
            facing = (hx > 0) ? 1 : -1;
        }

        // update animator
        bool isMoving = Mathf.Abs(hx) > movementThreshold;
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
            animator.SetInteger("Direction", facing);
        }
    }
}
