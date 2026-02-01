using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ChangeMask : MonoBehaviour
{
    [Header("Basic Property")]
    [SerializeField] GameObject[] masks;
    [SerializeField] GameObject baseMask;
    [SerializeField] ParticleSystem smoke;

    [Header("Shadows UI")]

    public int numOfSoldiers=0;
    public int numOfKnights=0;
    public int numOfKingsGuard=0;

    [SerializeField] TMP_Text numSoldiersText;
    [SerializeField] TMP_Text numKnightsText;
    [SerializeField] TMP_Text numKingsGuardText;


    [Header("UI Components")]
    [SerializeField] Image fill;

    Transform maskPos;
    GameObject currentMask;
    Movement movement;
    public bool isChanging;
    PlayerStats playerStats;

    PlayerAnswers anw;
    int maskWeight=0;
    int manaLevel=40;
    int maxMana=40;

    [Header("Sounds")]
    AudioSource source;
    [SerializeField] AudioClip smokeSound;
    Coroutine manaRoutine;

    EnemyController enemy;
    public ParticleSystem prepareTransform; 
    void Start()
    {

        smoke.Stop();
        movement=GetComponent<Movement>();
        maskPos = GameObject.Find("Mask").transform;
        Debug.Log($"Maska je {maskPos.name} nadjena");
        initializeMask();
        playerStats=GetComponent<PlayerStats>();
        playerStats.visibilityLevel=0; // Very visible

        anw=GetComponent<PlayerAnswers>();
        StartCoroutine(decreaseFill());

        source=GetComponent<AudioSource>();

        numSoldiersText.text=""+numOfSoldiers;
        numKnightsText.text=""+numOfKnights;
        numKingsGuardText.text=""+numOfKingsGuard;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartCoroutine(ChangeMyMask(0));
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartCoroutine(ChangeMyMask(1));
        if (Input.GetKeyDown(KeyCode.Alpha3)) StartCoroutine(ChangeMyMask(2));
        if (Input.GetKeyDown(KeyCode.Alpha4)) StartCoroutine(ChangeMyMask(3));

        //Find enemy which talks with youChange
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController currEnemy in enemies)
        {
            if (currEnemy.metYou)
            {
               enemy=currEnemy;
            }
        }


        if(manaLevel<=0) StartCoroutine(ChangeMyMask(3));

        if (movement.animator == null)
        {
            movement.animator=GetComponentInChildren<Animator>();
        }

       
    }

    void preparePowerForTransformation()
    {
        prepareTransform.Play();        
    }


    public void ExecuteCommand(string command)
    {
        Debug.Log("GPT Command received: " + command);

        command = command.ToLower();
        if(enemy==null){
        if (command.Contains("knight"))
            StartCoroutine(ChangeMyMask(1));
        else if (command.Contains("soldier"))
            StartCoroutine(ChangeMyMask(0));
        else if (command.Contains("assassin"))
            StartCoroutine(ChangeMyMask(3));
        else if (command.Contains("kings guard"))
            StartCoroutine(ChangeMyMask(2));}

        
        if(enemy!=null){
        if(command.Contains("long live"))
            anw.answer="long live to king";
        else if(command.Contains("kings guard"))
            anw.answer="kings guard";
        else if(command.Contains("kings soldier"))
            anw.answer="kings soldier";}


        movement.animator=GetComponentInChildren<Animator>();
    }

    private void initializeMask()
    {
        currentMask = Instantiate(baseMask, maskPos);
        currentMask.transform.localPosition = Vector3.zero;
        currentMask.transform.localRotation = Quaternion.identity;
    }

    IEnumerator ChangeMyMask(int index)
    {
        if (isChanging) yield break;
        if(index==0 && numOfSoldiers<=0) yield break;
        if(index==1 && numOfKnights<=0) yield break;
        if(index==2 && numOfKingsGuard<=0) yield break; 

        if(index==0 && numOfSoldiers > 0)
        {
            numOfSoldiers--;
            numSoldiersText.text=""+numOfSoldiers;
        }
        if(index==1 && numOfKnights > 0)
        {
            numOfKnights--;
            numKnightsText.text=""+numOfKnights;
        } 
        if(index==2 && numOfKingsGuard > 0)
        {
            numOfKingsGuard--;
            numKingsGuardText.text=""+numOfKingsGuard;
        }

        source.PlayOneShot(smokeSound);
        movement.animator.SetTrigger("change");

        if (index < 0 || index >= masks.Length) yield break;

        isChanging = true;

        smoke.Play();
        yield return new WaitForSeconds(1f);

        if (currentMask != null)
            Destroy(currentMask);

        playerStats.visibilityLevel=index+1;
        if(index+1>3)
            playerStats.visibilityLevel=0;
        
        currentMask = Instantiate(masks[index], maskPos);
        currentMask.transform.localPosition = Vector3.zero;
        currentMask.transform.localRotation = Quaternion.identity;

        MaskProp prop=currentMask.GetComponent<MaskProp>(); 
        maskWeight=prop.weight;

        //manaRoutine= StartCoroutine(decreaseFill());

        smoke.Stop();
        isChanging = false;
    }


    IEnumerator decreaseFill()
    {
        while(true){
            yield return new WaitForSeconds(5f);

            float delta = (maskWeight == 0) ? 1f : -maskWeight;

            manaLevel += Mathf.RoundToInt(delta * Time.deltaTime * 60f);

            manaLevel = Mathf.Clamp(manaLevel, 0, maxMana);

            fill.fillAmount = (float)manaLevel / maxMana;
        }
        
    }

    public void increaseSoldiers()
    {
        numOfSoldiers++;
        numSoldiersText.text=""+numOfSoldiers;
    }

    public void increaseKnights()
    {
        numOfKnights++;
        numKnightsText.text=""+numOfKnights;
    }

    public void increaseKingsGuard()
    {
        numOfKingsGuard++;
        numKingsGuardText.text=""+numOfKingsGuard;
    }


}
