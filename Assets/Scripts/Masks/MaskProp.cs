using UnityEngine;

public class MaskProp : MonoBehaviour
{
    public int weight=10;
    public GameObject [] weapons;
    int selectedWeapons=0;
    Movement movement;
    void Start()
    {
        movement=GetComponentInParent<Movement>();
        Debug.Log("Uzeo sam movement =>   "+movement);
        foreach(GameObject ss in weapons)
            ss.SetActive(false);
    }

    void activeWeapon()
    {
        weapons[movement.selectedWeapons].SetActive(true);
    }
    void deactiveWeapon()
    {
        weapons[movement.selectedWeapons].SetActive(false);
    }
}
