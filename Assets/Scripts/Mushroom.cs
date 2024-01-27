using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Mushroom : MonoBehaviour, IMountable
{
    private Animator animator;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider2D;
    private Capybara player;
    public Transform ridePoint;
    private bool isRiding;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool allowInteraction;
    private MountInputActions mountInputActions;
    private LastPressedKey lastPressedKey;
    private bool isFacingLeft = true;
    void Awake()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        mountInputActions = new MountInputActions();
        mountInputActions.Disable();
    }

    private void Start()
    {
        mountInputActions.Basic.Left.started += ctx => OnLeftPressed();
        mountInputActions.Basic.Left.canceled += ctx => OnLeftReleased();
        mountInputActions.Basic.Right.started += ctx => OnRightPressed();
        mountInputActions.Basic.Right.canceled += ctx => OnRightReleased();
        mountInputActions.Basic.Dismount.started += ctx => Dismount();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRiding)
        {
            MushroomMovement();
        }
        Debug.Log(rb2d.velocity);
    }

    public void Interact()
    {
        if (allowInteraction)
        {
            player.SetCurrentMountableObject(this);
            //Start riding
            player.ReparentSelfOnMount(ridePoint);
            //Capybara stop input
            player.DisableSelfOnMount();
            mountInputActions.Enable();
            //Mushroom movement active
            boxCollider2D.isTrigger = false;
            rb2d.simulated = true;
            isRiding = true;
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            animator.enabled = false;
        }
    }

    public void Dismount()
    {
        mountInputActions.Disable();
        player.EnableSelfOnMount();
        player.ReparentSelfOnMount(null);
        player.SetCurrentMountableObject(null);
        boxCollider2D.isTrigger = true;
        rb2d.simulated = false;
        isRiding = false;
        rb2d.bodyType = RigidbodyType2D.Static;
        animator.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            animator.SetBool("interactable", true);
            if (player == null)
            {
                player = collision.transform.parent.GetComponent<Capybara>();
                player.SetCurrentInteractibleObject(this);
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            animator.SetBool("interactable", false);
        }
        else if (collision.CompareTag("InteractablePort"))
        {
            allowInteraction = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("InteractablePort"))
        {
            allowInteraction = true;
        }
    }

    private void MushroomMovement()
    {
        // Ground Decetion
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer);

        // Movement
        if (lastPressedKey == LastPressedKey.Left)
        {
            isFacingLeft = true;
            rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
        }
        else if (lastPressedKey == LastPressedKey.Right)
        {
            isFacingLeft = false;
            rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
        }
        else
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }

        if (isFacingLeft)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Jump
        if (isGrounded && mountInputActions.Basic.Jump.IsPressed())
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
        }
    }

    public void OnLeftPressed()
    {
        lastPressedKey = LastPressedKey.Left;
    }

    public void OnLeftReleased()
    {
        if (lastPressedKey != LastPressedKey.Left)
        {
            return;
        }

        lastPressedKey = LastPressedKey.None;
    }

    public void OnRightPressed()
    {
        lastPressedKey = LastPressedKey.Right;
    }

    public void OnRightReleased()
    {
        if (lastPressedKey != LastPressedKey.Right)
        {
            return;
        }

        lastPressedKey = LastPressedKey.None;
    }
}
