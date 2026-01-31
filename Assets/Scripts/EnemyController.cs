using System.Dynamic;
using System.Security;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public int enemyVisionLevel; 
    [SerializeField] private float viewDistance = 20f;

    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float sprintSpeed = 5f;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] LayerMask playerMask;
    [SerializeField] float viewAngle=60f;
    private float speed = 0f;

    Animator animator;
    Transform targetPlayer;


    [Header("AudioSource")]
    [SerializeField] AudioClip [] audios; 
    public bool isAccepted;
    AudioSource source;


    bool tryToEscape=false;
    bool sayOnce1=true;
    bool sayOnce2=true;
    bool sayOnce3=true;

    public bool metYou=false;
    bool isEscape=false;

    PlayerAnswers anw;
    bool isCombatMode=false;
    bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        speed = walkSpeed; // start speed
        source=GetComponent<AudioSource>();
    }

    void Update()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (CanSeePlayer(player.transform))
        {
            Debug.Log("Ugledan");
            targetPlayer = player.transform;
        }
        else
        {
            targetPlayer = null;
        }


        //viewRange();

        if (targetPlayer != null)
        {
            MoveTowardsPlayer();
        }
        else
        {
            // Ako nema cilja, možeš tu staviti patrol logiku ili idle
            animator.SetFloat("speed", 0f);
        }


        if (isEscape && sayOnce3)
        {
            anw=null;
            sayOnce3=false;
            source.PlayOneShot(audios[1]);///////////////Assassin
          //   animator.SetTrigger("sword");
             isCombatMode=true;
        }

        if (isEscape)
        {
            MoveTowardsPlayer();
        }

        
        if (anw != null && anw.answer!=null && anw.answer.Length>0)
        {
            Debug.Log("Anser   "+anw.answer);
            string ans = anw.answer.ToLower();

            if (ans.Contains("guard") || ans.Contains("long live to king") || ans.Contains("kings soldier"))
            {
                Debug.Log("Dobar odgovor! ");
                source.PlayOneShot(audios[3]);
                this.enabled=false;
            }
            else
            {
                if (sayOnce3)
                {
                    anw=null;
                    sayOnce3=false;
                    source.PlayOneShot(audios[1]);///////////////Assassin
                    isCombatMode=true;
                    //animator.SetTrigger("sword");
                }
            }
        }
        if (isCombatMode && !isAttacking)
        {
            isAttacking=true;
            animator.SetTrigger("sword");
            Invoke("resetAttack", 0.8f);
        }
    }

    public void resetAttack()
    {
        isAttacking=false;
    }



    void viewRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, viewDistance);

        targetPlayer = null;

        foreach (Collider hit in hits)
        {
            PlayerStats playerStats = hit.GetComponentInParent<PlayerStats>();
            PlayerAnswers answers = hit.GetComponentInParent<PlayerAnswers>();

            if (playerStats == null) continue;

            anw = answers;

            int visibility = playerStats.visibilityLevel;
            Debug.Log("Pronađen igrač! Visibility: " + visibility);

            if (visibility <= enemyVisionLevel)
            {
                speed = walkSpeed;
                targetPlayer = playerStats.transform;

                if (sayOnce1 && visibility == enemyVisionLevel)
                {
                    sayOnce1 = false;
                    source.PlayOneShot(audios[0]);
                }
                else if (sayOnce3 && visibility < enemyVisionLevel)
                {
                    anw = null;
                    sayOnce3 = false;
                    source.PlayOneShot(audios[1]);
                    isCombatMode = true;
                }

                break;
            }
            else
            {
                if (sayOnce2)
                {
                    sayOnce2 = false;
                    source.PlayOneShot(audios[2]);
                }

                speed = 0f;
                animator.SetFloat("speed", speed);
            }
        }
    }

    bool CanSeePlayer(Transform player)
    {
        Vector3 origin = transform.position + Vector3.up * 1.6f; // visina očiju
        Vector3 directionToPlayer = (player.position + Vector3.up) - origin;

        float distance = directionToPlayer.magnitude;

        // 1. Distance check
        if (distance > viewDistance)
            return false;

        directionToPlayer.Normalize();

        // 2. Angle check (da li je ispred)
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle * 0.5f)
            return false;

        // 3. Raycast check (da li ima zid između)
        RaycastHit hit;
        if (Physics.Raycast(origin, directionToPlayer, out hit, viewDistance, obstacleMask | playerMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Debug.DrawRay(origin, directionToPlayer * distance, Color.green);
                return true;
            }
            else
            {
                Debug.DrawRay(origin, directionToPlayer * distance, Color.red);
                return false;
            }
        }

        return false;
    }


    void MoveTowardsPlayer()
    {
        if (targetPlayer == null) return;

        Vector3 toPlayer = targetPlayer.position - transform.position;
        float distance = toPlayer.magnitude;

        Vector3 moveDir = toPlayer;
        moveDir.y = 0f;
        moveDir.Normalize();

        float currentSpeed = speed;

        if (distance <= 1f)
        {
            metYou = true;
            currentSpeed = 0f;
        }
        else if (distance < 3f)
        {
            metYou = true;
            currentSpeed = 0f;
        }

        if (metYou && distance > 3f)
        {
            isEscape = true;
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

        Vector3 eyeHeight = transform.position + Vector3.up * 1.6f; // visina očiju

        // Limitni ray-evi (FOV)
        Vector3 leftLimit = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(eyeHeight, leftLimit * viewDistance);
        Gizmos.DrawRay(eyeHeight, rightLimit * viewDistance);
    }


}
