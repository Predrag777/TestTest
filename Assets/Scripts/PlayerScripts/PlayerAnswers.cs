using UnityEngine;

public class PlayerAnswers : MonoBehaviour
{
    public string answer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
         if (Input.GetKeyDown(KeyCode.G))
        {
            answer="guard";
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            answer = "kings soldier";
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            answer="long live to king";
        }
    }

    public void gptTranslation(string command)
    {
        answer=command;       
    }
}
