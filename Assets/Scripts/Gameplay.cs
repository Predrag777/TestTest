using UnityEngine;
using UnityEngine.SceneManagement;
public class Gameplay : MonoBehaviour
{
    GameObject player;
    PlayerStats localStats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player=GameObject.FindGameObjectWithTag("Player");
        localStats=player.GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (localStats.lost)
        {
            
            restartScene();
        }   
    }


    void restartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
