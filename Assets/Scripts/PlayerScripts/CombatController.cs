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


    CapsuleCollider myCollider;

    public bool isBlock=false;

    public bool swordDanger=false;


    public GameObject enemy;

    public bool isAttackAvail=false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currMove=GetComponent<Movement>();
        animator=currMove.animator;
        myMask=GetComponentInChildren<MaskProp>();

        source=GetComponent<AudioSource>();
        swordObj=myMask.weapons[1];

        myCollider=GetComponent<CapsuleCollider>();
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
        
        blockControl();

        if (!swordPull && Input.GetMouseButtonDown(0))
        {
            //Debug.Log("SWORD ACTIVE "+swordPull);
            //swordObj.SetActive(true);
            source.PlayOneShot(swordDraw);
            currMove.animator.SetTrigger("pull");
        }

        if(swordPull && Input.GetMouseButtonDown(0) && isAttackAvail)
        {
            source.PlayOneShot(swing2);
            currMove.animator.SetTrigger(("slash"+count));
            isAttackAvail=false;

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

    public void stopAttack()
    {
        isAttackAvail=true;
    }

    public void increaseCount()
    {
        count++;
        if(count>3)
            count=1;
    }

    public void swordPulled()
    {
        isAttackAvail=true;
        swordPull=true;
    }

    void blockControl()
    {
        currMove.animator.SetBool("block", (c%2==0));
        if (c % 2 == 0)
        {
            isBlock=true;
        }
        else
        {
            isBlock=false;
        }

    }

    void LateUpdate()
    {
        if(enemy != null)
        {
            EnemyCombat ec = enemy.GetComponent<EnemyCombat>();
            if(ec != null && ec.health <= 0)
            {
                enemy = null;
                return;
            }

            Vector3 lookDir = enemy.transform.position - transform.position;
            lookDir.y = 0f;
            if(lookDir.sqrMagnitude > 0.01f)
            {
                Quaternion rot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
            }
        }
    }

    

  
}
