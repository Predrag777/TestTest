using UnityEngine;

public class Sword : MonoBehaviour
{
    Enemy enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy=GetComponentInParent<Enemy>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && enemy.sayStop)
        {
            Debug.Log("Ubio me mac");
            other.gameObject.GetComponent<PlayerStats>().lost=true;
        }
    }
}
