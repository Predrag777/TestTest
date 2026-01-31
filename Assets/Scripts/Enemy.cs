using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public int enemyVisionLevel; 
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float sprintSpeed = 5f;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] LayerMask playerMask;
    [SerializeField] float viewAngle=60f;
    [SerializeField] float viewDistance=10f;
    private float speed = 0f;

    Animator animator;
    Transform targetPlayer;

    [Header("AudioSource")]
    [SerializeField] AudioClip [] audios; 
    AudioSource source;

    [Header("Noticed")]
    bool sayStop=false;
    bool isCaught=false;
    bool sayHi=false;


    Rigidbody rb;    
    GameObject assassin;
    float currSpeed;
    bool isAttacking=true;
    bool isGoodDistance=false;
    GameObject enemyAimed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb=GetComponent<Rigidbody>();
        source=GetComponent<AudioSource>();
        assassin=GameObject.FindGameObjectWithTag("Player");
        currSpeed=walkSpeed;
        animator=GetComponent<Animator>();

        targetPlayer=assassin.transform;
    }

    // Update is called once per frame
    void Update()
    {
        seekForPlayer(assassin.transform);
        if (sayStop)
        {
            MoveTowardsPlayer(sprintSpeed);
        }
        enemyAimed=detectEnemy();
        if (enemyAimed!=null && isAttacking && sayStop)
        {
            isAttacking=false;
            animator.SetTrigger("sword");
        }

    }

    void resetAttacking()
    {
        isAttacking=true;
    }

    bool seekForPlayer(Transform player)
    {
        Vector3 origin = transform.position + Vector3.up * 1.3f;
        Vector3 directionToPlayer = (player.position + Vector3.up) - origin;

        float distance = directionToPlayer.magnitude;

        if (distance > viewDistance)
            return false;

        directionToPlayer.Normalize();
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle * 0.5f)
            return false;
        RaycastHit hit;
        if (Physics.Raycast(origin, directionToPlayer, out hit, viewDistance, obstacleMask | playerMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                if(hit.collider.GetComponent<Movement>().isRolling) return false;
                if (hit.collider.GetComponent<PlayerStats>().visibilityLevel <= enemyVisionLevel)
                {
                    Debug.Log("UHVACEN SI");
                    if (!sayStop)
                    {
                        sayStop=true;
                        source.PlayOneShot(audios[0]);
                    }

                    return true;
                }
                else
                {
                    if(!sayHi){
                        sayHi=true;
                        source.PlayOneShot(audios[1]);
                    }
                    return false;
                }
                
                
            }
            else
            {
                return false;
            }
        }

        return false;
    }



    GameObject detectEnemy()
    {
        Vector3 boxCenter = transform.position + transform.forward * 1f + Vector3.up; 
        Vector3 boxHalfExtents = new Vector3(1f, 1f, 1f); 
        
        Quaternion boxRotation = transform.rotation;

        Collider[] hits = Physics.OverlapBox(boxCenter, boxHalfExtents, boxRotation);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Nanisanjen");
                return hit.gameObject;
            }
        }

        return null; 
    }


    void MoveTowardsPlayer(float pace)
    {   
        Vector3 toPlayer = targetPlayer.position - transform.position;
        float distance = toPlayer.magnitude;

        Vector3 moveDir = toPlayer;
        moveDir.y = 0f;
        moveDir.Normalize();

        float currentSpeed = pace;

        if (distance <= 2f)
        {
            //metYou = true;
            currentSpeed = 0f;
        }

        Vector3 lookDir = targetPlayer.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        transform.position += moveDir * currentSpeed * Time.deltaTime;

        animator.SetFloat("speed", currentSpeed);
    }



    private void OnDrawGizmos()
    {

        Vector3 boxCenter = transform.position + transform.forward * 1f + Vector3.up ;
        Vector3 boxHalfExtents = new Vector3(1f, 1f, 1f);

        Vector3 eyeHeight = transform.position + Vector3.up * 1.3f; 

        
        Vector3 leftLimit = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(eyeHeight, leftLimit * viewDistance);
        Gizmos.DrawRay(eyeHeight, rightLimit * viewDistance);
    }
}
