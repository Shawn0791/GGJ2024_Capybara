using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Capybara : Singleton<Capybara>
{
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
    private Vector2 moveInput = Vector2.zero;

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
        inputActions.Basic.Fart.performed += ctx => OnFart();
        inputActions.Basic.Move.performed += ctx => OnMove(ctx);
    }

    // Update is called once per frame
    void Update()
    {
        GroundDetection();
        rb.velocity += new Vector2(moveInput.x * moveSpeed, 0f);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        animator.SetBool("isWalking", true);
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
            if (rb.velocity.magnitude < velocityThreshold)
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
