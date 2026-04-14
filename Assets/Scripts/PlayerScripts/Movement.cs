
using UnityEngine;
using System.Collections;
public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float mouseSensitivity = 10f;
    [SerializeField] float sprintSpeed = 8f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 2f;

    private CharacterController controller;
    private Vector3 velocity;
    public Animator animator;

    private float activeSpeed=0f;
    public GameObject enemyAimed;
    bool isAttacking=false;
    ChangeMask changeMask;

    [SerializeField] float rollSpeed = 5f;
    PlayerStats ps;
    public bool isRolling = false;

    public GameObject [] weaponsSelection;
    public AudioClip [] weaponsSelectionClips;
    public int selectedWeapons=0;
    AudioSource source;

    CombatController combatController;

    // Dodge double-tap detection
    float doubleTapTime = 0.3f;
    float lastTapTimeA = -1f;
    float lastTapTimeD = -1f;
    float lastTapTimeS = -1f;
    bool isDodging = false;
    [SerializeField] float dodgeSpeed = 8f;
    Vector3 dodgeDirection = Vector3.zero;
    

    void Start()
    {
        ps=GetComponent<PlayerStats>();
        changeMask=GetComponent<ChangeMask>();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        source=GetComponent<AudioSource>();

        weaponsSelection[0].SetActive(true);
        weaponsSelection[1].SetActive(false);


        combatController=GetComponent<CombatController>();
    }

    void Update()
    {
        //Debug.Log("SSSSSSSSSSSSSSS    "+changeMask.activeMaskProp);
        //if(changeMask.activeMaskProp.weapons.Length>0){
            if ((Input.GetMouseButtonDown(0)) && !isAttacking)
            {
                isAttacking=true;
                if(selectedWeapons==0)
                    animator.SetTrigger("attack");
                /*else
                    animator.SetTrigger("slash");*/

                Invoke("SS", 1f);
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                selectedWeapons=0;

                source.PlayOneShot(weaponsSelectionClips[0]);

                weaponsSelection[0].SetActive(true);
                weaponsSelection[1].SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                selectedWeapons=1;

                source.PlayOneShot(weaponsSelectionClips[1]);

                weaponsSelection[1].SetActive(true);
                weaponsSelection[0].SetActive(false);
            }
        //}

        ///////////////////DODGES
        if (combatController.enemy != null && !isDodging)//
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (Time.time - lastTapTimeA < doubleTapTime)
                {
                    isDodging = true;
                    dodgeDirection = -transform.right;
                    animator.SetTrigger("leftDodge");
                    Invoke("ResetDodge", 0.5f);
                    lastTapTimeA = -1f;
                }
                else
                    lastTapTimeA = Time.time;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (Time.time - lastTapTimeD < doubleTapTime)
                {
                    isDodging = true;
                    dodgeDirection = transform.right;
                    animator.SetTrigger("rightDodge");
                    Invoke("ResetDodge", 0.5f);
                    lastTapTimeD = -1f;
                }
                else
                    lastTapTimeD = Time.time;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (Time.time - lastTapTimeS < doubleTapTime)
                {
                    isDodging = true;
                    dodgeDirection = -transform.forward;
                    animator.SetTrigger("backDodge");
                    Invoke("ResetDodge", 0.5f);
                    lastTapTimeS = -1f;
                }
                else
                    lastTapTimeS = Time.time;
            }
        }



        ////////////////

        if(isDodging)
        {
            DodgeMove();
            return;
        }

        if(isAttacking) return;
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

    void ResetDodge()
    {
        isDodging = false;
        dodgeDirection = Vector3.zero;
    }

    void DodgeMove()
    {
        Vector3 dir = dodgeDirection;
        dir.y = 0f;
        dir.Normalize();
        controller.Move(dir * dodgeSpeed * Time.deltaTime);
    }

    void MoveController()
    {
        if (changeMask.isChanging) return;

        if(combatController == null)
            combatController = GetComponent<CombatController>();

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            bool hasEnemy = combatController != null && combatController.enemy != null;

            Vector3 moveDirection;

            if(hasEnemy)
            {
                // Lock-on: kretanje relativno prema neprijatelju (strafe)
                Vector3 dirToEnemy = combatController.enemy.transform.position - transform.position;
                dirToEnemy.y = 0f;
                dirToEnemy.Normalize();

                Vector3 lockForward = dirToEnemy;
                Vector3 lockRight = Vector3.Cross(Vector3.up, lockForward).normalized;

                moveDirection = lockForward * vertical + lockRight * horizontal;

                // Ne rotiramo igraca - to radi CombatController.LateUpdate
            }
            else
            {
                // Uzimamo pravce kamere
                Vector3 camForward = Camera.main.transform.forward;
                Vector3 camRight = Camera.main.transform.right;

                camForward.y = 0f;
                camRight.y = 0f;

                camForward.Normalize();
                camRight.Normalize();

                moveDirection = camForward * vertical + camRight * horizontal;

                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }

            bool isSprinting = Input.GetKey(KeyCode.LeftShift) && !hasEnemy;
            float currentMoveSpeed = isSprinting ? sprintSpeed : speed;

            controller.Move(moveDirection * currentMoveSpeed * Time.deltaTime);

            activeSpeed = currentMoveSpeed;
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
