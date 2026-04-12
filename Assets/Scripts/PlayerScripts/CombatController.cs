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
    // Update is called once per frame
    void Update()
    {
        if(currMove.selectedWeapons!=1) {
            if(swordPull){
                Debug.Log("Vrati MAC");
                source.PlayOneShot(swordReturn);
                currMove.animator.SetTrigger("return");
                swordPull=false;
            }
            currMove.animator.SetBool("swordActive", swordPull);
            return;
        }

        currMove.animator.SetBool("swordActive", swordPull);

        // Blok - drzi desni klik
        blockControl();

        if (!swordPull && Input.GetMouseButtonDown(0))
        {
            source.PlayOneShot(swordDraw);
            currMove.animator.SetTrigger("pull");
        }

        if(swordPull && Input.GetMouseButtonDown(0) && isAttackAvail)
        {
            source.PlayOneShot(swing2);//
            currMove.animator.SetTrigger(("slash"+count));
            count++;
            if(count>3)
                count=1;
            
            isAttackAvail=false;

        }

        // Vracanje maca u korice na J
        if(swordPull && (Input.GetKeyDown(KeyCode.J)))
        {
            source.PlayOneShot(swordReturn);
            currMove.animator.SetTrigger("return");
            swordPull=false;
        }
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
        bool holding = Input.GetMouseButton(1);
        currMove.animator.SetBool("block", holding);
        isBlock = holding;
        if(holding)
            isAttackAvail=false;
        else if(Input.GetMouseButtonUp(1))
            isAttackAvail=true;
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
