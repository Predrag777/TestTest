using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] float viewDistance=20f;
    [SerializeField] float viewAngle=60f;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] LayerMask playerMask;
    [SerializeField] public int enemyVisionLevel;

    [SerializeField] Image healthUI;

    Animator animator;//
    public int health=3;
    int maxHealth;
    AudioSource source;
    NavMeshAgent agent;
    Transform player;

    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip death;
    [SerializeField] float keepDistance = 2f;
    [SerializeField] float attackCooldown = 1.5f;


    [Header("Patrol")]
    [SerializeField] bool isPatrol=false;
    [SerializeField] Transform pos1;
    [SerializeField] Transform pos2;
    bool goingToPos2 = true;
    bool isTurning = false;
    float turnSpeed = 3f;
    [SerializeField] float patrolSpeed = 2f;
    float originalSpeed;

    bool isAttacking = false;
    bool hasSeenPlayer = false;

    public bool isDanger=false;
    CombatController myController;
    PlayerStats playerStats;

    public bool isDead=false;
    bool isHitCooldown=false;
    public bool isFightingMode=false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source=GetComponent<AudioSource>();
        animator=GetComponent<Animator>();
        agent=GetComponent<NavMeshAgent>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        myController=playerObj.GetComponent<CombatController>();
        playerStats=playerObj.GetComponent<PlayerStats>();

        if(playerObj != null)
            player = playerObj.transform;

        maxHealth=health;
        originalSpeed=agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead) return;
        if(player == null || health <= 0)
        {
            if(agent.enabled) agent.ResetPath();
            animator.SetFloat("speed", 0f);
            return;
        }

        animator.SetBool("swordActive", isFightingMode);

        // Provjera vizije - jednom kad vidi igraca, prati ga zauvijek
        if(!hasSeenPlayer)
        {
            if(!CanSeePlayer(player))
            {
                if(isPatrol)
                    makePatrol();
                else
                {
                    if(agent.enabled) agent.ResetPath();
                    animator.SetFloat("speed", 0f);
                }
                return;
            }

            // Provjera ranga maske - ako je igraceva maska viseg ranga, ignorisi ga
            if(playerStats != null && playerStats.visibilityLevel > enemyVisionLevel)
            {
                if(isPatrol)
                    makePatrol();
                else
                {
                    if(agent.enabled) agent.ResetPath();
                    animator.SetFloat("speed", 0f);
                }
                return;
            }

            hasSeenPlayer = true;
            isFightingMode = true;
            agent.speed = originalSpeed;
        }

        // Ako igrac promijeni masku na visi rang, prestani ga pratiti
        if(playerStats != null && playerStats.visibilityLevel > enemyVisionLevel)
        {
            hasSeenPlayer = false;
            isFightingMode = false;
            if(isPatrol)
                makePatrol();
            else
            {
                if(agent.enabled) agent.ResetPath();
                animator.SetFloat("speed", 0f);
            }
            myController.enemy = null;
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        myController.enemy=this.gameObject;
        if(distance > keepDistance)
        {
            // Ne pomjeraj se dok napadas
            if(isAttacking)
            {
                agent.isStopped = true;
                animator.SetFloat("speed", 0f);
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                animator.SetFloat("speed", agent.velocity.magnitude);
            }
        }
        else
        {
            agent.isStopped = true;
            animator.SetFloat("speed", 0f);

            // Napadaj kad si dovoljno blizu
            if(!isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("sword");
                Invoke("ResetAttack", attackCooldown);
            }
        }

        // Uvek gledaj ka igracu
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;
        if(lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("katana") && !isHitCooldown && !isDead)
        {
            isHitCooldown=true;
            // Debug.Log("Pogodjen je sa "+collider.tag);
            if(health>1){
                source.PlayOneShot(hitSound);
                animator.SetTrigger("hit");
            }else if(health<=1){
                isDead=true;
                health=-2;
                source.PlayOneShot(death);
                animator.SetTrigger("death2");
                healthUI.fillAmount=0f;
            }
            Invoke("hurtReset",0.5f);//
        }
    }

    public void hurtReset()
    {
        isHitCooldown=false;
    }

    public void deathPlay(){
        isDead=true;
        health=-2;
        source.PlayOneShot(death);
        animator.SetTrigger("death2");
        healthUI.fillAmount=0f;
    }

    public void swordDanger()
    {
        isDanger=true;
    }
    public void swordNotDanger()
    {
        isDanger=false;
    }

    public void hurt()
    {
        Debug.Log("HURT ACTIV");
        health--;
        if(health>0)
            healthUI.fillAmount = (float)health / maxHealth;
        else
            healthUI.fillAmount=0f;
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("Sudario sam se sa: " + collision.gameObject.name);
    }

    bool CanSeePlayer(Transform playerTarget)
    {
        Vector3 origin = transform.position + Vector3.up * 1.6f;
        Vector3 directionToPlayer = (playerTarget.position + Vector3.up) - origin;

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
                // Debug.DrawRay(origin, directionToPlayer * distance, Color.green);
                return true;
            }
            else
            {
                // Debug.DrawRay(origin, directionToPlayer * distance, Color.red);
                return false;
            }
        }

        return false;
    }


    void makePatrol(){
        if(pos1 == null || pos2 == null || health<=0) return;

        Transform target = goingToPos2 ? pos2 : pos1;

        if(isTurning)
        {
            // Stoji i okrece se ka novom cilju
            agent.isStopped = true;
            animator.SetFloat("speed", 0f);

            Vector3 dir = (target.position - transform.position);
            dir.y = 0f;
            if(dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);

                // Kada je dovoljno okrenut, nastavi hodati
                if(Quaternion.Angle(transform.rotation, targetRot) < 5f)
                {
                    isTurning = false;
                }
            }
            else
            {
                isTurning = false;
            }
            return;
        }

        agent.speed = patrolSpeed;
        agent.isStopped = false;
        agent.SetDestination(target.position);
        animator.SetFloat("speed", agent.velocity.magnitude);

        if(!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            goingToPos2 = !goingToPos2;
            isTurning = true;
        }
    }

    void resetAttack()
    {
    }

    void finishAttack(){

    }

    void lockAttacking(){}

    void unlockAttacking(){}
}
