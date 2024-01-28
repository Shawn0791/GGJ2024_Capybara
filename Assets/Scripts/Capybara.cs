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
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private AudioClip bounceClip;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;
    private CircleCollider2D circleCollider2D;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;
    public LayerMask groundLayer;
    public bool IsFacingLeft { get; set; } = true;
    private bool isRolling = false;
    private IControllable currentMountableObject;
    private List<IInteractible> interactibleQueue = new();
    private Queue<FartEvent> fartEventQueue = new();
    private FartEvent playerFartEvent;
    private bool isGrounded = true;

    public ParticleSystem fartParticle;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        InputController.Instance.RegisterControllableActionHandler(this, InputControllerAction.Move, OnMovement);
        InputController.Instance.RegisterControllableActionHandler(this, InputControllerAction.Interact, OnInteract);
        InputController.Instance.RegisterControllableActionHandler(this, InputControllerAction.Fart, OnFart);
        InputController.Instance.SetCurrentControllable(this);
    }

    // Update is called once per frame
    void Update()
    {
        GroundDetection();
        Vector2 movement = new Vector2(InputController.Instance.Movement.x, InputController.Instance.IsInWater ? InputController.Instance.Movement.y : 0);
        if (!isRolling)
        {
            if (movement.x < 0)
            {
                IsFacingLeft = true;
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            }
            else if (movement.x > 0)
            {
                IsFacingLeft = false;
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

        }
        if (InputController.Instance.IsInWater)
        {
            rb.velocity = movement.normalized * moveSpeed;
        }
        if (IsFacingLeft)
        {
            transform.localScale = new Vector3(2, 2, 2);
        }
        else
        {
            transform.localScale = new Vector3(-2, 2, 2);
        }

        if (playerFartEvent != null)
        {
            Fart(playerFartEvent);
        }
        else if (fartEventQueue.Count > 0)
        {
            Fart(fartEventQueue.Dequeue());
        }
        else if ((isGrounded || InputController.Instance.IsInWater) && InputController.Instance.IsJumping)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(bounceClip);
            }
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        Debug.Log($"isInWater {InputController.Instance.IsInWater}");
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
            isGrounded = true;
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
            isGrounded = false;
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

    public void EnqueueFartEvent(float randomFartForce, float randomDisturbanceAngle)
    {
        fartEventQueue.Enqueue(new FartEvent(randomFartForce, randomDisturbanceAngle));
    }

    public void Fart(FartEvent evt)
    {
        rb.AddForceAtPosition((IsFacingLeft ? -1 : 1) * evt.Force * (Quaternion.AngleAxis(evt.Angle, Vector3.back) * idleFartSpawnPoint.right), fartSpawnPoint.position);
        fartParticle.Stop();
        fartParticle.Clear();
        fartParticle.Play();
    }

    public void ReverseScale()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void ResetScale()
    {
        transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
    }

    public void OnMovement()
    {
        animator.SetBool("isWalking", InputController.Instance.Movement.x != 0);
    }

    public void OnJump()
    {
    }
}
