using System.Security;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public int enemyVisionLevel; 
    [SerializeField] private float viewDistance = 20f;

    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float sprintSpeed = 5f;
    private float speed = 0f;

    Animator animator;
    Transform targetPlayer;

    void Start()
    {
        animator = GetComponent<Animator>();
        speed = walkSpeed; // start speed
    }

    void Update()
    {
        viewRange();

        if (targetPlayer != null)
        {
            MoveTowardsPlayer();
        }
        else
        {
            // Ako nema cilja, možeš tu staviti patrol logiku ili idle
            animator.SetFloat("Speed", 0f);
        }
    }

    void viewRange()
    {
        // Kreiraj sferu oko neprijatelja
        Collider[] hits = Physics.OverlapSphere(transform.position, viewDistance);

        targetPlayer = null; // reset target prije provjere

        foreach (Collider hit in hits)
        {
            PlayerStats playerStats = hit.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                int visibility = playerStats.visibilityLevel;
                Debug.Log("Pronađen igrač! Visibility: " + visibility);

                if (visibility < enemyVisionLevel) // ako je igrač dovoljno vidljiv
                {
                    speed=walkSpeed;
                    targetPlayer = playerStats.transform; // postavi kao cilj
                    Debug.Log("Igrač je vidljiv! Neprijatelj reaguje!");
                    break; // možemo prestati tražiti druge igrače
                }
                else
                {
                    speed=0f;
                    animator.SetFloat("speed", speed);
                }
            }
        }
    }

    void MoveTowardsPlayer()
    {
        if (targetPlayer == null) return;

        // Računaj smjer prema igraču samo po XZ ravnini
        Vector3 direction = targetPlayer.position - transform.position;
        direction.y = 0; // ignoriraj Y
        direction = direction.normalized;

        if (direction != Vector3.zero)
        {
            // Rotiraj neprijatelja prema igraču (XZ ravnina)
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Pomakni neprijatelja prema igraču po XZ
        transform.position += direction * speed * Time.deltaTime;

        // Animacija
        animator.SetFloat("speed", speed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
