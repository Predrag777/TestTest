using UnityEngine;

public class CombatController : MonoBehaviour
{
    bool swordPull=false;
    Animator animator;
    Movement currMove;
    public MaskProp myMask;
    public GameObject swordObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currMove=GetComponent<Movement>();
        animator=currMove.animator;
        myMask=GetComponentInChildren<MaskProp>();

        swordObj=myMask.weapons[1];
    }

    // Update is called once per frame
    void Update()
    {
        if(currMove.selectedWeapons!=1) return;
        
        if (!swordPull && Input.GetMouseButtonDown(0))
        {
            //Debug.Log("SWORD ACTIVE "+swordPull);
            //swordObj.SetActive(true);
            
            currMove.animator.SetTrigger("pull");
        }

        if(swordPull && Input.GetMouseButtonDown(0))
        {
            currMove.animator.SetTrigger("slash");
        }




        if(swordPull && Input.GetMouseButtonDown(1))
        {
            currMove.animator.SetTrigger("return");
            swordPull=false;
        }
        

        /*if(!swordPull && Input.GetMouseButtonDown(0))
        {
            swordPull=true;
            currMove.animator.SetTrigger("pull");
        }
        if(swordPull && Input.GetMouseButtonDown(1))
        {
            swordPull=false;
            currMove.animator.SetTrigger("return");
        }*/
    }

    public void swordPulled()
    {
        swordPull=true;
    }
}
