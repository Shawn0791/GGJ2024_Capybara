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
    private Rigidbody2D rb;
    private CapybaraInputActions inputActions;
    private PolygonCollider2D polygonCollider;
    private CircleCollider2D circleCollider2D;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public LayerMask groundLayer;
    public float raycastDistance = 1.0f;
    private bool isGround = false;

    protected override void Awake()
    {
        base.Awake();
        inputActions = new CapybaraInputActions();
        inputActions.Enable();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer.sprite = defaultSprite;
    }

    private void Start()
    {
        inputActions.Basic.Fart.performed += ctx => OnFart();
    }

    // Update is called once per frame
    void Update()
    {
        GroundDetection();
    }

    // TODO: delete this, only here for testing
    public void OnFart()
    {
        rb.AddForceAtPosition(-idleFartSpawnPoint.right * fartForce, fartSpawnPoint.position);
    }

    private void DetectGroundCollision()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, new Vector2(0, -1), out raycastHit))
        {

        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            animator.enabled = false;
            spriteRenderer.sprite = rollingSprite;
            polygonCollider.enabled = false;
            circleCollider2D.enabled = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            spriteRenderer.sprite = defaultSprite;
            animator.enabled = true;
            polygonCollider.enabled = true;
            circleCollider2D.enabled = false;
        }
    }

    private void GroundDetection()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);

        if (hit.collider != null)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }
    }
}
