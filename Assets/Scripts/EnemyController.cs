using System.Dynamic;
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

        viewRange();

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
        // Kreiraj sferu oko neprijatelja
        Collider[] hits = Physics.OverlapSphere(transform.position, viewDistance);

        targetPlayer = null; // reset target prije provjere

        foreach (Collider hit in hits)
        {   
            anw=hit.GetComponent<PlayerAnswers>();
            PlayerStats playerStats = hit.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                int visibility = playerStats.visibilityLevel;
                Debug.Log("Pronađen igrač! Visibility: " + visibility);

                
                if (visibility <= enemyVisionLevel)
                {
                    speed=walkSpeed;
                    targetPlayer = playerStats.transform; // postavi kao cilj
                    Debug.Log("Igrač je vidljiv! Neprijatelj reaguje!  "+visibility+"  "+enemyVisionLevel);
                    if (sayOnce1 && visibility==enemyVisionLevel)
                    {
                        sayOnce1=false;
                        source.PlayOneShot(audios[0]);
                    }
                    else if(sayOnce3 && visibility<enemyVisionLevel)
                    {
                        anw=null;
                        sayOnce3=false;
                        source.PlayOneShot(audios[1]);///////////////Assassin
                        //animator.SetTrigger("sword");
                        isCombatMode=true;
                    }
                    break;
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

        if (distance <= 1f){
            metYou=true;    
            currentSpeed = 0f;}          // stani
        else if (distance < 3f){
            metYou=true;
            currentSpeed = 0f; }         // ostani stani dok ne pređe 2f
        if(metYou && distance > 3)
        {
            isEscape=true;
        }
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
