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
    private Rigidbody2D rb;
    private CapybaraInputActions inputActions;
    private PolygonCollider2D polygonCollider;
    private CircleCollider2D circleCollider2D;
    private bool isTouchingGround = false;

    protected override void Awake()
    {
        base.Awake();
        inputActions = new CapybaraInputActions();
        inputActions.Enable();
        rb = GetComponent<Rigidbody2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        inputActions.Basic.Fart.performed += ctx => OnFart();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // TODO: delete this, only here for testing
    public void OnFart()
    {
        rb.AddForceAtPosition(-idleFartSpawnPoint.right * fartForce, fartSpawnPoint.position);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isTouchingGround = false;
            polygonCollider.enabled = false;
            circleCollider2D.enabled = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isTouchingGround = true;
            polygonCollider.enabled = true;
            circleCollider2D.enabled = false;
        }
    }
}
