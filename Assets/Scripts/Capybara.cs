using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
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
    public bool IsFacingLeft { get; set; } = true;
    private bool isRolling = false;
    private IControllable currentMountableObject;
    private List<IInteractible> interactibleQueue = new();
    private Queue<FartEvent> fartEventQueue = new();
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
                IsFacingLeft = true;
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            }
            else if (InputController.Instance.ActiveDirectionKey == LastPressedKey.Right)
            {
                IsFacingLeft = false;
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        if (IsFacingLeft)
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
        rb.AddForceAtPosition((IsFacingLeft ? -1 : 1) * fartForce * idleFartSpawnPoint.right, fartSpawnPoint.position);
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

    public void AddInteractibleObject(IInteractible interactible)
    {
        interactibleQueue.Add(interactible);
    }

    public void RemoveInteractibleObject(IInteractible interactible)
    {
        interactibleQueue.Remove(interactible);
    }

    public void SetCurrentMountableObject(IControllable mountable)
    {
        currentMountableObject = mountable;
    }

    public void ReparentSelfOnMount(Transform newParentTransform)
    {
        transform.SetParent(newParentTransform);
        if (newParentTransform)
        {
            transform.position = newParentTransform.position;
        }
    }

    public void DisableSelfOnMount()
    {
        rb.simulated = false;
        enabled = false;
        boxCollider2D.enabled = false;
        circleCollider2D.enabled = false;
        InputController.Instance.SetCurrentControllable(currentMountableObject);
    }

    public void EnableSelfOnMount()
    {
        rb.simulated = true;
        enabled = true;
        boxCollider2D.enabled = true;
        InputController.Instance.SetCurrentControllable(this);
    }

    public void OnInteract()
    {
        if (interactibleQueue.Count > 0)
        {
            IInteractible interactible = interactibleQueue[interactibleQueue.Count - 1];
            interactible.Interact(this);
        }
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
        rb.AddForceAtPosition((IsFacingLeft ? -1 : 1) * evt.Force * (Quaternion.AngleAxis(evt.Angle, Vector3.back) * idleFartSpawnPoint.right), fartSpawnPoint.position);
    }

    public void ReverseScale()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void ResetScale()
    {
        transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
    }
}
