using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : MonoBehaviour, IMountable
{
    private Animator animator;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider2D;
    private Capybara player;
    public Transform ridePoint;
    private bool isRiding;

    public float moveSpeed = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;
    private MountInputActions mountInputActions;
    private bool allowInteraction;
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
            CrocodileMovement();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            animator.SetBool("isSleeping", false);
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
            animator.SetBool("isSleeping", true);
            player.SetCurrentInteractibleObject(null);
            player = null;
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

    private void CrocodileMovement()
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

        //Attack

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

    public void Interact()
    {
        if (allowInteraction)
        {
            //Start riding
            player.SetCurrentInteractibleObject(this);
            player.ReparentSelfOnMount(ridePoint);
            //Capybara stop input
            player.DisableSelfOnMount();
            mountInputActions.Enable();
            //Crocodile movement active
            boxCollider2D.isTrigger = false;
            rb2d.simulated = true;
            isRiding = true;
        }
    }

    public void Dismount()
    {
        mountInputActions.Disable();
        player.EnableSelfOnMount();
        player.ReparentSelfOnMount(null);
        player.SetCurrentInteractibleObject(null);
        boxCollider2D.isTrigger = true;
        rb2d.simulated = false;
        isRiding = false;
    }
}
