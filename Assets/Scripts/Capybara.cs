using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Capybara : MonoBehaviour, IControllable
{
    [SerializeField] private Transform fartSpawnPoint;
    [SerializeField] private Transform idleFartSpawnPoint;
    [SerializeField] private float fartForce = 100f;
    [SerializeField] private float raycastDistance = 1.0f;
    [SerializeField] private float velocityThreshold = 5f;
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;
    private CircleCollider2D circleCollider2D;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public LayerMask groundLayer;
    private bool isFacingLeft = true;
    private bool isRolling = false;
    private IInteractible currentInteractibleObject;
    private Queue<FartEvent> fartEventQueue = new Queue<FartEvent>();
    private FartEvent playerFartEvent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InputController.Instance.RegisterControllable(this);
        InputController.Instance.SetCurrentControllable(this);
    }

    // Update is called once per frame
    void Update()
    {
        GroundDetection();
        if (!isRolling)
        {
            if (InputController.Instance.ActiveDirectionKey == LastPressedKey.Left)
            {
                isFacingLeft = true;
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            }
            else if (InputController.Instance.ActiveDirectionKey == LastPressedKey.Right)
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

        if (playerFartEvent != null)
        {
            Fart(playerFartEvent);
        }
        else if (fartEventQueue.Count > 0)
        {
            Fart(fartEventQueue.Dequeue());
        }
    }

    public void OnLeftPressed()
    {
        animator.SetBool("isWalking", true);
    }

    public void OnRightPressed()
    {
        animator.SetBool("isWalking", true);
    }

    public void OnLeftReleased()
    {
        animator.SetBool("isWalking", false);
    }

    public void OnRightReleased()
    {
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
    }

    public void EnableSelfOnMount()
    {
        rb.simulated = true;
        enabled = true;
        boxCollider2D.enabled = true;
    }

    public void OnInteract()
    {
        currentInteractibleObject?.Interact();
    }

    public void OnJump()
    {
        // TODO: implement
    }

    public void EnqueueFartEvent(float randomFartForce, float randomDisturbanceAngle)
    {
        fartEventQueue.Enqueue(new FartEvent(randomFartForce, randomDisturbanceAngle));
    }

    public void Fart(FartEvent evt)
    {
        rb.AddForceAtPosition((isFacingLeft ? -1 : 1) * evt.Force * (Quaternion.AngleAxis(evt.Angle, Vector3.back) * idleFartSpawnPoint.right), fartSpawnPoint.position);
    }
}
