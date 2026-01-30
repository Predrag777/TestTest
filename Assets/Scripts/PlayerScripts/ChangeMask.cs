using UnityEngine;
using System.Collections;

public class ChangeMask : MonoBehaviour
{
    [SerializeField] GameObject[] masks;
    [SerializeField] GameObject baseMask;
    [SerializeField] ParticleSystem smoke;

    Transform maskPos;
    GameObject currentMask;
    Movement movement;
    bool isChanging;

    void Start()
    {
        smoke.Stop();
        movement=GetComponent<Movement>();
        maskPos = GameObject.Find("Mask").transform;
        Debug.Log($"Maska je {maskPos.name} nadjena");
        initializeMask();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) StartCoroutine(ChangeMyMask(0));
        if (Input.GetKeyDown(KeyCode.Alpha2)) StartCoroutine(ChangeMyMask(1));
        if (Input.GetKeyDown(KeyCode.Alpha3)) StartCoroutine(ChangeMyMask(2));
    }

    // 🔥 OVO ZOVE CHATGPT IZ WEBGL-a
    public void ExecuteCommand(string command)
    {
        Debug.Log("GPT Command received: " + command);

        command = command.ToLower();

        if (command.Contains("knight"))
            StartCoroutine(ChangeMyMask(0));
        else if (command.Contains("soldier"))
            StartCoroutine(ChangeMyMask(1));
        else if (command.Contains("assassin"))
            StartCoroutine(ChangeMyMask(2));
        else if (command.Contains("kings guard"))
            StartCoroutine(ChangeMyMask(3));
        else
            Debug.LogWarning("Unknown mask command: " + command);
        
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
        if (index < 0 || index >= masks.Length) yield break;

        isChanging = true;

        smoke.Play();
        yield return new WaitForSeconds(1f);

        if (currentMask != null)
            Destroy(currentMask);

        currentMask = Instantiate(masks[index], maskPos);
        currentMask.transform.localPosition = Vector3.zero;
        currentMask.transform.localRotation = Quaternion.identity;

        smoke.Stop();
        isChanging = false;
    }
}
