using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Capybara : MonoBehaviour
{
    [SerializeField] private Transform fartSpawnPoint;
    [SerializeField] private Transform idleFartSpawnPoint;
    [SerializeField] private float fartForce = 100f;
    [SerializeField] private float raycastDistance = 1.0f;
    [SerializeField] private float velocityThreshold = 5f;
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private CapybaraInputActions inputActions;
    private BoxCollider2D boxCollider2D;
    private CircleCollider2D circleCollider2D;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public LayerMask groundLayer;
    private LastPressedKey lastPressedKey = LastPressedKey.None;
    private bool isFacingLeft = true;
    private bool isRolling = false;
    private IInteractible currentInteractibleObject;

    private void Awake()
    {
        inputActions = new CapybaraInputActions();
        inputActions.Enable();
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        inputActions.Basic.Fart.started += ctx => OnFart();
        inputActions.Basic.Left.started += ctx => OnLeftPressed(ctx);
        inputActions.Basic.Left.canceled += ctx => OnLeftReleased(ctx);
        inputActions.Basic.Right.started += ctx => OnRightPressed(ctx);
        inputActions.Basic.Right.canceled += ctx => OnRightReleased(ctx);
        inputActions.Basic.Interact.started += ctx => OnInteract();
    }

    // Update is called once per frame
    void Update()
    {
        GroundDetection();
        if (!isRolling)
        {
            if (lastPressedKey == LastPressedKey.Left)
            {
                isFacingLeft = true;
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            }
            else if (lastPressedKey == LastPressedKey.Right)
            {
                isFacingLeft = false;
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        if (isFacingLeft)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnLeftPressed(InputAction.CallbackContext context)
    {
        lastPressedKey = LastPressedKey.Left;
        animator.SetBool("isWalking", true);
    }

    private void OnRightPressed(InputAction.CallbackContext context)
    {
        lastPressedKey = LastPressedKey.Right;
        animator.SetBool("isWalking", true);
    }

    private void OnLeftReleased(InputAction.CallbackContext context)
    {
        if (lastPressedKey != LastPressedKey.Left)
        {
            return;
        }

        lastPressedKey = LastPressedKey.None;
        animator.SetBool("isWalking", false);
    }

    private void OnRightReleased(InputAction.CallbackContext context)
    {
        if (lastPressedKey != LastPressedKey.Right)
        {
            return;
        }

        lastPressedKey = LastPressedKey.None;
        animator.SetBool("isWalking", false);
    }

    // TODO: delete this, only here for testing
    public void OnFart()
    {
        Debug.Log($"idleFartSpawnPoint {idleFartSpawnPoint.right}, fartSpawnPoint {fartSpawnPoint.position}");
        rb.AddForceAtPosition((isFacingLeft ? -1 : 1) * fartForce * idleFartSpawnPoint.right, fartSpawnPoint.position);
    }

    private void GroundDetection()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);
        if (hit.collider != null)
        {
            if (rb.velocity.magnitude < velocityThreshold && isRolling)
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                animator.SetTrigger("stopRolling");
                isRolling = false;
                boxCollider2D.enabled = true;
                circleCollider2D.enabled = false;
                transform.rotation = Quaternion.identity;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + new Vector3(0, -raycastDistance, 0), Color.green);
            if (!isRolling)
            {
                animator.SetTrigger("startRolling");
            }
            isRolling = true;
            boxCollider2D.enabled = false;
            circleCollider2D.enabled = true;
        }
    }

    public void SetCurrentInteractibleObject(IInteractible interactible)
    {
        currentInteractibleObject = interactible;
    }

    public void SetCurrentMountableObject(IMountable mountable)
    {
        currentInteractibleObject = null;
    }

    public void ReparentSelfOnMount(Transform newParentTransform)
    {
        transform.SetParent(newParentTransform);
        transform.position = newParentTransform.position;
    }

    public void DisableSelfOnMount()
    {
        rb.simulated = false;
        enabled = false;
        boxCollider2D.enabled = false;
        circleCollider2D.enabled = false;
        inputActions.Disable();
    }

    public void EnableSelfOnMount()
    {
        rb.simulated = true;
        enabled = true;
        boxCollider2D.enabled = true;
        inputActions.Enable();
    }

    public void OnInteract()
    {
        currentInteractibleObject?.Interact();
    }
}
