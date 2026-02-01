using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int visibilityLevel;
    public bool lost=false;
    Movement movement;
    bool isdead=false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement=GetComponent<Movement>();
        //animator=GetComponent<Animator>();
    }

    void Update()
    {
        if (lost && !isdead)
        {
            isdead=true;
            movement.animator.SetTrigger("death");
        }
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
