using UnityEngine;

public class Sword : MonoBehaviour
{
    EnemyCombat enemyCombat;

    AudioSource source;
    [SerializeField] AudioClip swordClash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source=GetComponent<AudioSource>();
        enemyCombat=GetComponentInParent<EnemyCombat>();
        //enemy=GetComponentInParent<Enemy>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && enemyCombat.isDanger)
        {
            
            if(other.gameObject.GetComponent<CombatController>().isBlock){
                MaskProp myMask=other.gameObject.GetComponentInChildren<MaskProp>();
                myMask.playSparks();    
                source.PlayOneShot(swordClash);
                return;
            }
            Debug.Log("MAC ME JE POGODIO "+other.gameObject.GetComponent<PlayerStats>().healthUI);
            other.gameObject.GetComponent<PlayerStats>().takeDamage();
            if(other.gameObject.GetComponent<PlayerStats>().health<=0)
                other.gameObject.GetComponent<PlayerStats>().lost=true;
        }
    }
}
