using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public int health=100;
    public int visibilityLevel;
    private Image healthUI;
    public bool lost=false;
    Movement movement;
    bool isdead=false;
    public bool isArthurDead=false;
    bool isWon=false;


    public bool isFightingMode=false;
    public GameObject opponent=null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement=GetComponent<Movement>();
        //animator=GetComponent<Animator>();
        healthUI=GetComponentInChildren<Image>();
    }

    void Update()
    {
        if (lost && !isdead)
        {
            isdead=true;
            movement.animator.SetTrigger("death");
        }
        if(isArthurDead)
        {
            isWon=true;
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

    public void takeDamage()
    {
        health-=10;
        healthUI.fillAmount=health/100f;
    }

}
