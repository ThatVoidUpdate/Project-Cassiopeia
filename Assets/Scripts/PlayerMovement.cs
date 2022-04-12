using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform camera;

    [Header("Speeds")]
    public float Speed;

    public float JumpForce;

    [Header("Sensitivity")]
    public float HorizontalSensitivity;
    public float VerticalSensitivity;

    private CharacterController controller;
    private Vector3 Velocity;

    
    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    
    void Update()
    {
        bool grounded = controller.isGrounded; //Store for later. Prevents weird behaviour around jumping not working

        if (grounded && Velocity.y < 0) //Zero out old vertical velocity
        {
            Velocity.y = 0f;
        }

        Vector3 move = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical"); //Get the movement vector
        controller.Move(move * Time.deltaTime * Speed); //Move in that vector

        if (Input.GetButtonDown("Jump") && grounded) //Check if jumping
        {
            Velocity.y += Mathf.Sqrt(JumpForce * -3.0f * Physics.gravity.y);
        }

        Velocity.y += Physics.gravity.y * Time.deltaTime; //Recalculate the vertical velocity
        controller.Move(Velocity * Time.deltaTime); //Do vertical movement


        camera.transform.eulerAngles = camera.transform.eulerAngles + Vector3.right * Input.GetAxis("Mouse Y") * -VerticalSensitivity; //rotate the camera along its x axis
        transform.eulerAngles = transform.eulerAngles + Vector3.up * Input.GetAxis("Mouse X") * HorizontalSensitivity; //rotate the player along their z axis


        if (Input.GetKeyDown(KeyCode.Escape)) //Allow the mouse to be used
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
