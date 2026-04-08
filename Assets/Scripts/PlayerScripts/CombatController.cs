using UnityEngine;

public class CombatController : MonoBehaviour
{
    bool swordPull=false;
    Animator animator;
    Movement currMove;
    public MaskProp myMask;
    public GameObject swordObj;

    int count=1;
    AudioSource source;
    [SerializeField] AudioClip swordDraw;
    [SerializeField] AudioClip swordReturn;

    [SerializeField] AudioClip swing1;
    [SerializeField] AudioClip swing2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currMove=GetComponent<Movement>();
        animator=currMove.animator;
        myMask=GetComponentInChildren<MaskProp>();

        source=GetComponent<AudioSource>();
        swordObj=myMask.weapons[1];
    }
    int c=1;
    // Update is called once per frame
    void Update()
    {
        if(currMove.selectedWeapons!=1) return;

        currMove.animator.SetBool("swordActive", swordPull);

        if (Input.GetKeyDown(KeyCode.J)) //Activate block
        {
            c++; 
        }
        currMove.animator.SetBool("block", (c%2==0));

        if (!swordPull && Input.GetMouseButtonDown(0))
        {
            //Debug.Log("SWORD ACTIVE "+swordPull);
            //swordObj.SetActive(true);
            source.PlayOneShot(swordDraw);
            currMove.animator.SetTrigger("pull");
        }

        if(swordPull && Input.GetMouseButtonDown(0))
        {
            source.PlayOneShot(swing2);
            currMove.animator.SetTrigger(("slash"+count));

        }




        if(swordPull && Input.GetMouseButtonDown(1))
        {
            source.PlayOneShot(swordReturn);
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

    public void increaseCount()
    {
        count++;
        if(count>3)
            count=1;
    }

    public void swordPulled()
    {
        swordPull=true;
    }
}
