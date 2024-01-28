using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Mushroom : MonoBehaviour, IControllable, IInteractible
{
    private Animator animator;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider2D;
    private Capybara player;
    public Transform ridePoint;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
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
        Mushroom mushroom = InputController.Instance.GetCurrentControllable() as Mushroom;
        if (mushroom != null && mushroom == this)
        {
            MushroomMovement();
        }
    }

    public void Interact(Capybara capybara)
    {
        player = capybara;
        player.SetCurrentMountableObject(this);
        //Start riding
        player.ReparentSelfOnMount(ridePoint);
        //Capybara stop input
        player.DisableSelfOnMount();
        //Mushroom movement active
        boxCollider2D.isTrigger = false;
        rb2d.simulated = true;
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        animator.enabled = false;
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
        rb2d.bodyType = RigidbodyType2D.Static;
        animator.enabled = true;
        player = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            animator.SetBool("interactable", true);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            animator.SetBool("interactable", false);
        }
    }

    private void MushroomMovement()
    {
        // Ground Decetion
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer);

        // Movement
        Vector2 movement = InputController.Instance.Movement;
        if (movement.x < 0)
        {
            isFacingLeft = true;
        }
        else
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

        // Jump
        if (isGrounded && InputController.Instance.IsJumping)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
        }
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
