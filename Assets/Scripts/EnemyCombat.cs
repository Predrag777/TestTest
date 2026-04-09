using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    Animator animator;
    public int health=3;
    AudioSource source;
    NavMeshAgent agent;
    Transform player;

    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip death;
    [SerializeField] float keepDistance = 2f;
    [SerializeField] float attackCooldown = 1.5f;

    bool isAttacking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source=GetComponent<AudioSource>();
        animator=GetComponent<Animator>();
        agent=GetComponent<NavMeshAgent>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
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

        float distance = Vector3.Distance(transform.position, player.position);

        

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
                animator.SetTrigger("death");
            }
        }
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

      void resetAttack()
    {
        
    }
}
