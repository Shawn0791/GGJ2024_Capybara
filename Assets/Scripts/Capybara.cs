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
    private BoxCollider2D boxCollider2D;
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
        boxCollider2D = GetComponent<BoxCollider2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
