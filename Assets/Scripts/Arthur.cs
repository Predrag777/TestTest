using UnityEngine;

public class Arthur : MonoBehaviour
{
    EnemyCombat enemyCombat;
    public Animator animator;
    [SerializeField] GameObject endGame;
    PlayerStats playerStats;
    bool isDead=false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyCombat=GetComponent<EnemyCombat>();
        animator=GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null)
            playerStats = playerObj.GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (!isDead && enemyCombat.health <= 0)
        {
            animator.SetTrigger("death2");
            isDead=true;
            endGame.SetActive(true);

            if(playerStats != null)
                playerStats.isArthurDead = true;
        }
    }

}
