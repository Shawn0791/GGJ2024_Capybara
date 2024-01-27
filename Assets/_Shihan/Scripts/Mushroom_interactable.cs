using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_interactable : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider2D;
    private Transform player;
    public Transform ridePoint;
    private bool isRiding;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRiding)
            MushroomMovement();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            animator.SetBool("interactable", true);

            if (player == null)
            {
                player = collision.transform.parent;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ViewPort"))
        {
            animator.SetBool("interactable", false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("InteractablePort"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Start riding
                player.SetParent(ridePoint);
                player.transform.position = ridePoint.position;
                //Capybara stop input
                player.GetComponent<Rigidbody2D>().simulated = false;
                player.GetComponent<Capybara>().enabled = false;
                player.GetComponent<BoxCollider2D>().enabled = false;
                //Musroom movement active
                boxCollider2D.isTrigger = false;
                rb2d.simulated = true;
            }
        }
    }

    private void MushroomMovement()
    {
        // Ground Decetion
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer);

        // Movement
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 moveDirection = new Vector2(horizontalInput, 0f);
        rb2d.velocity = new Vector2(moveDirection.x * moveSpeed, rb2d.velocity.y);

        // Jump
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
        }
    }
}
