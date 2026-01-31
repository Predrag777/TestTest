
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
    GameObject enemyAimed;
    bool isAttacking=false;
    ChangeMask changeMask;
    void Start()
    {
        changeMask=GetComponent<ChangeMask>();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        if ((Input.GetMouseButtonDown(0)) && !isAttacking)
        {
            isAttacking=true;
            animator.SetTrigger("attack");
            if (enemyAimed != null)
            {
                
                Invoke("destroyEnemy", 0.5f);
            }
            Invoke("SS", 3f);
        }

        MoveController();
        RotationController();
        ApplyGravityAndJump();
        UpdateAnimator();
        enemyAimed=detectEnemy();

    }

    public void SS()
    {
        isAttacking=false;
    }

    void destroyEnemy()
    {
        if(enemyAimed!=null)
        Destroy(enemyAimed);
    }

    void MoveController()
    {
        if(changeMask.isChanging) return;
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

    float yRotation = 0f;

    void RotationController()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        yRotation += mouseX;   

        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
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
        if(animator==null) return;
        float currentSpeed = activeSpeed;
        animator.SetFloat("speed", currentSpeed);
    }

    GameObject detectEnemy()
    {
        // Položaj centra kockice 2x2x2 ispred igrača
        Vector3 boxCenter = transform.position + transform.forward * 1f + Vector3.up; 
        Vector3 boxHalfExtents = new Vector3(1f, 1f, 1f); // polovina dimenzija (2x2x2)
        
        // Rotacija kockice ista kao igrača
        Quaternion boxRotation = transform.rotation;

        // Dobij sve kolidere u boxu
        Collider[] hits = Physics.OverlapBox(boxCenter, boxHalfExtents, boxRotation);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                return hit.gameObject;
            }
        }

        return null; // Nema neprijatelja
    }

    private void OnDrawGizmos()
    {
        Vector3 boxCenter = transform.position + transform.forward * 1f + Vector3.up ;
        Vector3 boxHalfExtents = new Vector3(1f, 1f, 1f);

        Gizmos.color = Color.red;

        // Postavimo matricu za rotaciju
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);

        // Nacrtaj wireframe cube
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);
    }


}
