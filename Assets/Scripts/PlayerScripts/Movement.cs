
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float mouseSensitivity = 10f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 2f;

    private CharacterController controller;
    private Vector3 velocity;
    public Animator animator;

    private float activeSpeed=0f;

    void Start()
    {
        //animator = GetComponentInChildren<Animator>();
        //animator.applyRootMotion = false;
        //Debug.Log("Animator "+animator+" SS");
        controller = GetComponent<CharacterController>();
        Debug.Log("Animator "+animator+" SS   "+controller+" Kontroler");
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        MoveController();
        RotationController();
        ApplyGravityAndJump();
        UpdateAnimator();

    }

    void MoveController()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move =
            transform.forward * vertical +
            transform.right * horizontal;

        if (move != Vector3.zero)
        {
            activeSpeed = speed;  
        }
        else
        {
            activeSpeed = 0f;
        }
            controller.Move(move * speed * Time.deltaTime);
    }

    void RotationController()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        float y = transform.eulerAngles.y;
        y = Mathf.DeltaAngle(0f, y);         
        y = Mathf.Clamp(y + mouseX, -60f, 60f);

        transform.rotation = Quaternion.Euler(0f, y, 0f);
    }


    void ApplyGravityAndJump()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        float currentSpeed = activeSpeed;
        Debug.Log("Speed   "+currentSpeed);
        animator.SetFloat("speed", currentSpeed);
    }
}
