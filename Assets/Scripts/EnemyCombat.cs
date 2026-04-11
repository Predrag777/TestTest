using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] float viewDistance=20f;
    [SerializeField] float viewAngle=60f;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] LayerMask playerMask;
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

    public bool isDanger=false;
    CombatController myController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source=GetComponent<AudioSource>();
        animator=GetComponent<Animator>();
        agent=GetComponent<NavMeshAgent>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        myController=playerObj.GetComponent<CombatController>();

        if(playerObj != null)
            player = playerObj.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null || health <= 0)
        {
            if(agent.enabled) agent.ResetPath();
            animator.SetFloat("speed", 0f);
            return;
        }

        animator.SetBool("swordActive", true);

        // Provjera vizije umjesto samo distance
        if(!CanSeePlayer(player))
        {
            if(agent.enabled) agent.ResetPath();
            animator.SetFloat("speed", 0f);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        myController.enemy=this.gameObject;
        if(distance > keepDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetFloat("speed", agent.velocity.magnitude);
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
        if (collider.CompareTag("katana"))
        {
            
            Debug.Log("Pogodjen je sa "+collider.tag);
            if(health>0){
                source.PlayOneShot(hitSound);
                animator.SetTrigger("hit");
            }else{
                source.PlayOneShot(death);
                animator.SetTrigger("death2");
            }
        }
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
}
