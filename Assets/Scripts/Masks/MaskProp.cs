using UnityEngine;

public class MaskProp : MonoBehaviour
{
    public int weight = 10;
    public GameObject[] weapons;
    int selectedWeapons = 0;
    PlayerStats stats;

    Movement movement;
    CombatController combatController;

    [SerializeField] ParticleSystem trail;
    [SerializeField] ParticleSystem sparks;

    bool isHurtable=true;
    void Start()
    {
        stats=GetComponentInParent<PlayerStats>();
        movement = GetComponentInParent<Movement>();
        combatController = GetComponentInParent<CombatController>();

        Debug.Log("Uzeo sam movement => " + movement);

        sparks.Stop();

        foreach (GameObject ss in weapons)
            ss.SetActive(false);
    }

    void Update()
    {
        if (combatController.swordDanger)
        {
            weapons[1].GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            weapons[1].GetComponent<BoxCollider>().enabled = false;
        }
    }

    void activeWeapon()
    {
        weapons[movement.selectedWeapons].SetActive(true);
    }

    void deactiveWeapon()
    {
        weapons[movement.selectedWeapons].SetActive(false);
    }

    public void playSparks()
    {
        sparks.Play();
    }


    void turnOff()
    {
        trail.Stop();
    }
    void turnOn()
    {
        trail.Play();
    }

    // OVO zove Animation Event
    public void SwordPulledEvent()
    {
        if (combatController != null)
        {
            combatController.swordPulled();
        }
    }

    public void swordDanger()
    {
        combatController.swordDanger=true;
    }

    public void swordNotDanger()
    {
        combatController.swordDanger=false;
    }

    public void hurt()
    {
        if(!isHurtable) return;

        isHurtable=false;
        Debug.Log("HIT aktiviran kod mog igraca");
        stats.takeDamage();
    }

    public void hurtReset(){
        isHurtable=true;
    }

    public void finishAttack(){//
        combatController.isAttackAvail=true;
    }

    void lockAttacking(){
        combatController.isAttackAvail=true;
    }

    void unlockAttacking(){
        combatController.isAttackAvail=true;
    }
}