using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform body;

    [Header("Roll Settings")]
    [SerializeField] private float rollSpeed = 10f;
    [SerializeField] private float rollDuration = 0.3f;

    private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    private bool isRolling = false;
    private float rollTimer = 0f;
    private int facingDir = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandleMove();

        HandleRoll();
    }

    private void HandleMove()
    {
        if (isRolling)
        {
            if (anim != null) anim.SetFloat("Walk", 0f);
            return;
        }

        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveX = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveX = 1f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            moveY = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            moveY = -1f;

        Vector2 moveDir = new Vector2(moveX, moveY).normalized;

        rb.linearVelocity = moveDir * moveSpeed;

        if (moveX != 0)
        {
            facingDir = moveX > 0 ? 1 : -1;

            Vector3 scale = body.localScale;
            scale.x = facingDir;
            body.localScale = scale;
        }

        if (anim != null)
        {
            anim.SetFloat("Walk", moveDir.magnitude);
        }
    }

    public void SpeedUp(float speed)
    {
        moveSpeed += speed;
    }

    private void HandleRoll()
    {
        if (isRolling)
        {
            rollTimer -= Time.deltaTime;

            rb.linearVelocity = new Vector2(facingDir * rollSpeed, rb.linearVelocity.y);

            if (rollTimer <= 0f)
            {
                isRolling = false;
            }

            return;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartRoll();
        }
    }

    private void StartRoll()
    {
        isRolling = true;
        rollTimer = rollDuration;

        rb.linearVelocity = new Vector2(facingDir * rollSpeed, rb.linearVelocity.y);

        if (anim != null)
        {
            anim.SetTrigger("Roll");
        }
    }
}
