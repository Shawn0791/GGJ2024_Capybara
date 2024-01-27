using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Capybara : Singleton<Capybara>
{
    enum LastPressedKey
    {
        None,
        Left,
        Right
    }

    [SerializeField] private Transform fartSpawnPoint;
    [SerializeField] private Transform idleFartSpawnPoint;
    [SerializeField] private float fartForce = 100f;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite rollingSprite;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float raycastDistance = 1.0f;
    [SerializeField] private float velocityThreshold = 5f;
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private CapybaraInputActions inputActions;
    private BoxCollider2D boxCollider2D;
    private CircleCollider2D circleCollider2D;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public LayerMask groundLayer;
    private bool isGround = false;
    private Vector2 baseInputVelocity = Vector2.zero;
    private Vector2 inputVelocityToApply = Vector2.zero;
    private bool isAdditionalForceApplied = false;
    private LastPressedKey lastPressedKey = LastPressedKey.None;
    private bool canPressKey = true;

    protected override void Awake()
    {
        base.Awake();
        inputActions = new CapybaraInputActions();
        inputActions.Enable();
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultSprite;
    }

    private void Start()
    {
        inputActions.Basic.Fart.started += ctx => OnFart();
        inputActions.Basic.Left.started += ctx => OnLeftPressed(ctx);
        inputActions.Basic.Left.canceled += ctx => OnLeftReleased(ctx);
        inputActions.Basic.Right.started += ctx => OnRightPressed(ctx);
        inputActions.Basic.Right.canceled += ctx => OnRightReleased(ctx);
    }

    // Update is called once per frame
    void Update()
    {
        GroundDetection();
        if (!animator.GetBool("isRolling"))
        {
            if (lastPressedKey == LastPressedKey.Left)
            {
                spriteRenderer.flipX = false;
                rb.velocity = new Vector2(-moveSpeed, 0);
            }
            else if (lastPressedKey == LastPressedKey.Right)
            {
                spriteRenderer.flipX = true;
                rb.velocity = new Vector2(moveSpeed, 0);
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    private void OnLeftPressed(InputAction.CallbackContext context)
    {

        lastPressedKey = LastPressedKey.Left;
        animator.SetBool("isWalking", true);
    }

    private void OnRightPressed(InputAction.CallbackContext context)
    {
        lastPressedKey = LastPressedKey.Right;
        animator.SetBool("isWalking", true);
    }

    private void OnLeftReleased(InputAction.CallbackContext context)
    {
        if (lastPressedKey != LastPressedKey.Left)
        {
            return;
        }

        lastPressedKey = LastPressedKey.None;
        animator.SetBool("isWalking", false);
    }

    private void OnRightReleased(InputAction.CallbackContext context)
    {
        if (lastPressedKey != LastPressedKey.Right)
        {
            return;
        }

        lastPressedKey = LastPressedKey.None;
        animator.SetBool("isWalking", false);
    }

    // TODO: delete this, only here for testing
    public void OnFart()
    {
        rb.AddForceAtPosition(-idleFartSpawnPoint.right * fartForce, fartSpawnPoint.position);
    }

    private void GroundDetection()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);
        if (hit.collider != null)
        {
            isGround = true;
            if (rb.velocity.magnitude < velocityThreshold && animator.GetBool("isRolling"))
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                animator.SetBool("isRolling", false);
                boxCollider2D.enabled = true;
                circleCollider2D.enabled = false;
                transform.rotation = Quaternion.identity;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + new Vector3(0, -raycastDistance, 0), Color.green) ;
            isGround = false;
            animator.SetBool("isRolling", true);
            boxCollider2D.enabled = false;
            circleCollider2D.enabled = true;
        }
    }
}
