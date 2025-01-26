using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;

    public float playerHeight;
    public LayerMask groundLayer;
    bool grounded;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown = 0.25f;
    public float airMultiplier;
    public bool canJump = true;

    public Transform orientation;

    float hInput;
    float vInput;

    Vector3 moveDir;

    Rigidbody rb;

    private Alteruna.Avatar _avatar;

    private void Start() {
        _avatar = GetComponent<Alteruna.Avatar>();

        if (!_avatar.IsMe) return;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update() {
        if (!_avatar.IsMe) return;

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        MyInput();

        if (grounded) {
            rb.drag = groundDrag;
        } else {
            rb.drag = 0;
        }
    }

    void FixedUpdate() {
        if (!_avatar.IsMe) return;
        
        MovePlayer();
    }

    private void MyInput() {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        // jump
        if (Input.GetKey(KeyCode.Space) && canJump && grounded) {
            Jump();
            canJump = false;


            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer() {
        moveDir = orientation.forward * vInput + orientation.right * hInput;

        if (grounded) {
            rb.AddForce(moveDir * moveSpeed * 10f, ForceMode.Force);
        } else {
            rb.AddForce(moveDir * moveSpeed * airMultiplier * 10f, ForceMode.Force);
        }
        
    }

    private void SpeedControl() {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed) {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump() {

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump() {
        canJump = true;
    }
}
