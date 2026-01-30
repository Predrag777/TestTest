using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int visibilityLevel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.rigidbody != null && hit.rigidbody.CompareTag("Enemy"))
        {
            Debug.Log("Igrač je udaren od neprijatelja!");
        }
    }

}
