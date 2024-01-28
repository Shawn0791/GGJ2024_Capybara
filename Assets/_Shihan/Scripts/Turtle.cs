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

    public float moveSpeed = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool isFacingLeft = true;
    private bool isInWater = false;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        InputController.Instance.RegisterControllableActionHandler(this, InputControllerAction.Move, () => {});
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
        else if (collision.CompareTag("Water"))
        {
            isInWater = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            // animator.SetBool("isSleeping", true);
        }
        else if (collision.CompareTag("Water"))
        {
            isInWater = false;
        }
    }

    private void TurtleMovement()
    {
        // Ground Decetion
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer);

        // Movement
        Vector2 movement = new Vector2(InputController.Instance.Movement.x, isInWater ? InputController.Instance.Movement.y : 0);
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
        boxCollider2D.isTrigger = true;
        rb2d.simulated = false;
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
