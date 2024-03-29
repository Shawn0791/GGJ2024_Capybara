using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : MonoBehaviour, IInteractible, IControllable
{
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider2D;
    private Capybara player;
    public Transform ridePoint;
    private Animator animator;

    public float moveSpeed = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool isFacingLeft = true;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        InputController.Instance.RegisterControllableActionHandler(this, InputControllerAction.Move, () => {});
        InputController.Instance.RegisterControllableActionHandler(this, InputControllerAction.Interact, OnInteract);
    }

    // Update is called once per frame
    void Update()
    {
        Turtle turtle = InputController.Instance.GetCurrentControllable() as Turtle;
        if (turtle != null && turtle == this)
        {
            TurtleMovement();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            //animator.SetBool("isSleeping", false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            // animator.SetBool("isSleeping", true);
        }
    }

    private void TurtleMovement()
    {
        // Ground Decetion
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer);

        // Movement
        Vector2 movement = new Vector2(InputController.Instance.Movement.x, InputController.Instance.IsInWater ? InputController.Instance.Movement.y : 0);
        if (movement.x < 0)
        {
            isFacingLeft = true;
        }
        else if (movement.x > 0)
        {
            isFacingLeft = false;
        }

        rb2d.velocity = movement.normalized * moveSpeed;

        if (isFacingLeft)
        {
            transform.localScale = new Vector3(-Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (movement.x == 0 && movement.y == 0)
            animator.SetBool("isWalking", false);
        else
            animator.SetBool("isWalking", true);

    }

    public void Interact(Capybara capybara)
    {
        player = capybara;
        //Start riding
        player.SetCurrentMountableObject(this);
        player.ReparentSelfOnMount(ridePoint);
        //Capybara stop input
        player.DisableSelfOnMount();
        player.ReverseScale();
        //Pelican movement active
        boxCollider2D.isTrigger = false;
        rb2d.simulated = true;
        isFacingLeft = player.IsFacingLeft;
    }

    public void Dismount()
    {
        player.EnableSelfOnMount();
        player.ReparentSelfOnMount(null);
        player.SetCurrentMountableObject(null);
        boxCollider2D.isTrigger = false;
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
