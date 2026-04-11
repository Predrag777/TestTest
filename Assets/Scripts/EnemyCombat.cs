using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] float viewDistance=20f;
    [SerializeField] float viewAngle=60f;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] LayerMask playerMask;
    [SerializeField] public int enemyVisionLevel;
    Animator animator;//
    public int health=3;
    AudioSource source;
    NavMeshAgent agent;
    Transform player;

    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip death;
    [SerializeField] float keepDistance = 2f;
    [SerializeField] float attackCooldown = 1.5f;

    bool isAttacking = false;
    bool hasSeenPlayer = false;

    public bool isDanger=false;
    CombatController myController;
    PlayerStats playerStats;

    bool isDead=false;
    bool isHitCooldown=false;
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

        animator.SetBool("swordActive", true);

        // Provjera vizije - jednom kad vidi igraca, prati ga zauvijek
        if(!hasSeenPlayer)
        {
            if(!CanSeePlayer(player))
            {
                if(agent.enabled) agent.ResetPath();
                animator.SetFloat("speed", 0f);
                return;
            }

            // Provjera ranga maske - ako je igraceva maska viseg ranga, ignorisi ga
            if(playerStats != null && playerStats.visibilityLevel > enemyVisionLevel)
            {
                if(agent.enabled) agent.ResetPath();
                animator.SetFloat("speed", 0f);
                return;
            }

            hasSeenPlayer = true;
        }

        // Ako igrac promijeni masku na visi rang, prestani ga pratiti
        if(playerStats != null && playerStats.visibilityLevel > enemyVisionLevel)
        {
            hasSeenPlayer = false;
            if(agent.enabled) agent.ResetPath();
            animator.SetFloat("speed", 0f);
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
        if (collider.CompareTag("katana") && !isHitCooldown)
        {
            isHitCooldown=true;
            Debug.Log("Pogodjen je sa "+collider.tag);
            if(health>1){
                source.PlayOneShot(hitSound);
                animator.SetTrigger("hit");
            }else if(health<=1){
                isDead=true;
                health=-2;
                source.PlayOneShot(death);
                animator.SetTrigger("death2");
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
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Sudario sam se sa: " + collision.gameObject.name);
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

    void resetAttack()
    {
    }

    void finishAttack(){

    }

    void lockAttacking(){}

    void unlockAttacking(){}
}
