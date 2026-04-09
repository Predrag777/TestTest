using UnityEngine;

public class Sword : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //enemy=GetComponentInParent<Enemy>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(other.gameObject.GetComponent<CombatController>().isBlock) return;
            Debug.Log("MAC ME JE POGODIO "+other.gameObject.GetComponent<PlayerStats>().healthUI);
            other.gameObject.GetComponent<PlayerStats>().takeDamage();
            if(other.gameObject.GetComponent<PlayerStats>().health<=0)
                other.gameObject.GetComponent<PlayerStats>().lost=true;
        }
    }
}
