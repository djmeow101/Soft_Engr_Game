using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterController))]

public class FPSController:MonoBehaviour
{
    public Animator animator;
    public Camera playerCamera;
    public float speed = 5f;
    public float sensitivity = 2f;

    Vector3 moveDir = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;
    public bool attack = false;
    CharacterController controller;


    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);
        animator.SetFloat("move", move.magnitude);
        // Mouse Look
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    
        // attack animation (does not attack)
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetBool("attack", true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("attack", false);
        }
        if(Input.GetKeyDown(KeyCode.LeftShift)) speed = 10f;animator.SetFloat("speed", speed);
        if(Input.GetKey(KeyCode.LeftShift)) speed = 10f;animator.SetFloat("speed", speed);
        if(Input.GetKeyUp(KeyCode.LeftShift)) speed = 5f;animator.SetFloat("speed", speed);
    
    }
}