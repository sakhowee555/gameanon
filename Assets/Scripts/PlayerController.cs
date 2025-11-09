using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.0f;
    public float runMultiplier = 1.5f;
    public float interactDistance = 0.8f;
    public LayerMask interactableMask;

    [Header("Refs")]
    public Transform interactProbe; // empty child in front of player
    public InteractableScanner scanner; // helper to find nearest interactable

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 facing = Vector2.down; // default face down
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>(); // optional
    }

    void Update()
    {
        // --- Input ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        input = new Vector2(h, v).normalized;

        // Update facing when moving (keep last non-zero dir)
        if (input.sqrMagnitude > 0.01f)
        {
            facing = input;
            if (interactProbe != null)
                interactProbe.localPosition = new Vector3(Mathf.Round(facing.x), Mathf.Round(facing.y), 0f) * 0.5f;
        }

        // Interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }

        // Anim (optional)
        if (animator)
        {
            animator.SetFloat("MoveX", input.x);
            animator.SetFloat("MoveY", input.y);
            animator.SetFloat("Speed", input.sqrMagnitude);
        }
    }

    void FixedUpdate()
    {
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? runMultiplier : 1f);
        rb.linearVelocity = input * speed;
    }

    void TryInteract()
    {
        if (interactProbe == null) return;

        // Overlap small circle at probe to detect Interactable
        Collider2D hit = Physics2D.OverlapCircle(interactProbe.position, 0.25f, interactableMask);
        if (hit == null) return;

        Interactable itx = hit.GetComponentInParent<Interactable>();
        if (itx != null)
        {
            itx.OnInteract(this);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (interactProbe)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactProbe.position, 0.25f);
        }
    }
}
