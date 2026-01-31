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


    [Header("AudioSource")]
    [SerializeField] AudioClip [] audios; 
    public bool isAccepted;
    AudioSource source;

    void Start()
    {
        animator = GetComponent<Animator>();
        speed = walkSpeed; // start speed
        source=GetComponent<AudioSource>();
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

    bool sayOnce1=true;
    bool sayOnce2=true;
    bool sayOnce3=true;

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
                    if (sayOnce1)
                    {
                        sayOnce1=false;
                        source.PlayOneShot(audios[0]);
                    }
                    break; // možemo prestati tražiti druge igrače
                }
                else
                {
                    if (sayOnce2)
                    {
                        sayOnce2=false;
                        source.PlayOneShot(audios[2]);
                    }
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
        float distance = direction.magnitude;

        direction = direction.normalized;

        // Postavi speed na 0 ako je blizu, inače koristi normalni speed
        float currentSpeed = speed;

        if (distance <= 1f)
            currentSpeed = 0f;          // stani
        else if (distance < 2f){
            if (sayOnce3)
            {
                sayOnce3=false;
                source.PlayOneShot(audios[1]);///////////////Assassin
            }
            currentSpeed = 0f; }         // ostani stani dok ne pređe 2f
        // else -> distance >= 2f -> nastavi sa normalnim speed

        // Rotiraj neprijatelja samo ako može da ide
        if (direction != Vector3.zero && currentSpeed > 0f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Pomakni neprijatelja
        transform.position += direction * currentSpeed * Time.deltaTime;

        // Animacija
        animator.SetFloat("speed", currentSpeed);
    }




    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
