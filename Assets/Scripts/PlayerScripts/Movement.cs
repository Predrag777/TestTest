
using UnityEngine;
using System.Collections;
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

    [SerializeField] float rollSpeed = 5f;
    public bool isRolling = false;


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
                if(enemyAimed.name.Contains("knigsGuard")) changeMask.increaseKingsGuard();
                else if(enemyAimed.name.Contains("knight")) changeMask.increaseKnights();
                else if(enemyAimed.name.Contains("soldier")) changeMask.increaseSoldiers();
                Invoke("destroyEnemy", 0.5f);
            }
            Invoke("SS", 3f);
        }

        if(Input.GetKeyDown(KeyCode.T) && !isRolling)
        {
            StartCoroutine(RollRoutine());
        }

        if(!isRolling)
            MoveController(); 
        else
            RollMove(); 
        if(isRolling) return;//Not move unitil finish the rolling
        MoveController();
        //RotationController();
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
        if (changeMask.isChanging) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            // Uzimamo pravce kamere
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = camForward * vertical + camRight * horizontal;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

            controller.Move(moveDirection * speed * Time.deltaTime);

            activeSpeed = speed;
        }
        else
        {
            activeSpeed = 0f;
        }
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
        Vector3 boxCenter = transform.position + transform.forward * 1f + Vector3.up; 
        Vector3 boxHalfExtents = new Vector3(1f, 1f, 1f); 
        
        Quaternion boxRotation = transform.rotation;

        Collider[] hits = Physics.OverlapBox(boxCenter, boxHalfExtents, boxRotation);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                return hit.gameObject;
            }
        }

        return null; 
    }

    private void OnDrawGizmos()
    {
        Vector3 boxCenter = transform.position + transform.forward * 1f + Vector3.up ;
        Vector3 boxHalfExtents = new Vector3(1f, 1f, 1f);

        Gizmos.color = Color.red;

        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);
    }


IEnumerator RollRoutine()
{
    isRolling = true;

    animator.SetTrigger("roll");

    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    float rollDuration = 1f;


    yield return new WaitForSeconds(rollDuration);

    isRolling = false;
}



    void RollMove()
    {
        Vector3 rollDirection = transform.forward;

        rollDirection.y = 0f;
        rollDirection.Normalize();

        controller.Move(rollDirection * rollSpeed * Time.deltaTime);

        activeSpeed = rollSpeed;
    }


}
