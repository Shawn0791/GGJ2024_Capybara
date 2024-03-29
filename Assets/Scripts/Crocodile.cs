using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : MonoBehaviour, IInteractible, IControllable
{
    private Animator animator;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider2D;
    private Capybara player;
    public Transform ridePoint;

    public float moveSpeed = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool isFacingLeft = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        InputController.Instance.RegisterControllableActionHandler(this, InputControllerAction.Move, () => {});
        InputController.Instance.RegisterControllableActionHandler(this, InputControllerAction.Interact, OnInteract);
    }

    // Update is called once per frame
    void Update()
    {
        Crocodile crocodile = InputController.Instance.GetCurrentControllable() as Crocodile;
        if (crocodile != null && crocodile == this)
        {
            CrocodileMovement();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            animator.SetBool("isSleeping", false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            animator.SetBool("isSleeping", true);
        }
    }

    private void CrocodileMovement()
    {
        // Ground Decetion
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer);

        // Movement
        Vector2 movement = InputController.Instance.Movement;
        if (movement.x < 0)
        {
            isFacingLeft = true;
        }
        else if (movement.x > 0)
        {
            isFacingLeft = false;
        }

        rb2d.velocity = new Vector2(Math.Sign(movement.x) * moveSpeed, rb2d.velocity.y);

        if (isFacingLeft)
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (movement.x == 0 && movement.y == 0)
            animator.SetBool("isWalking", false);
        else
            animator.SetBool("isWalking", true);

        animator.SetBool("isSleeping", false);

        //Attack

    }

    public void Interact(Capybara capybara)
    {
        player = capybara;
        //Start riding
        player.SetCurrentMountableObject(this);
        player.ReparentSelfOnMount(ridePoint);
        //Capybara stop input
        player.DisableSelfOnMount();
        player.ResetScale();
        //Crocodile movement active
        boxCollider2D.isTrigger = false;
        rb2d.simulated = true;
        isFacingLeft = player.IsFacingLeft;
    }

    public void Dismount()
    {
        if (player)
        {
            player.EnableSelfOnMount();
            player.ReparentSelfOnMount(null);
            player.SetCurrentMountableObject(null);
        }
        boxCollider2D.isTrigger = false;
        rb2d.simulated = true;
    }

    public void OnFart()
    {

    }

    public void OnInteract()
    {
        Dismount();
    }

    public void OnJump()
    {
    }
}
