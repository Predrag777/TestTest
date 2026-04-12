using UnityEngine;

public class Dagger : MonoBehaviour
{
    public ParticleSystem blood;
    public AudioClip stabb;
    AudioSource source;
    Movement player;
    PlayerStats ps;
    ChangeMask changeMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player=GetComponentInParent<Movement>();
        source=GetComponent<AudioSource>();
        blood.Stop();
        ps=GetComponentInParent<PlayerStats>();
        changeMask=GetComponentInParent<ChangeMask>();
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Udario u "+other.gameObject.tag);
        if (other.gameObject.CompareTag("Enemy"))
        {
            source.PlayOneShot(stabb);
            blood.Play();
            other.GetComponent<EnemyCombat>().deathPlay();

            /*if (player.enemyAimed != null)
            {
                if(!player.enemyAimed.name.Contains("Arthur")){
                    player.enemyAimed.GetComponent<Enemy>().animator.SetTrigger("death2");
                }else{
                    player.enemyAimed.GetComponent<Arthur>().animator.SetTrigger("death2");
                    ps.isArthurDead=true;
                }
                if(player.enemyAimed.name.Contains("knigsGuard")) changeMask.increaseKingsGuard();
                else if(player.enemyAimed.name.Contains("knight")) changeMask.increaseKnights();
                else if(player.enemyAimed.name.Contains("soldier")) changeMask.increaseSoldiers();
                
            }*/
            
        }
    }

    void destroyEnemy()
    {
        if(player.enemyAimed!=null)
        Destroy(player.enemyAimed);
    }


    
}
