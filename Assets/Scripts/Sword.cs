using UnityEngine;

public class Sword : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Ubio me mac");
            other.gameObject.GetComponent<PlayerStats>().lost=true;
        }
    }
}
