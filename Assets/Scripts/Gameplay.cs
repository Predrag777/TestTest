using UnityEngine;
using UnityEngine.SceneManagement;
public class Gameplay : MonoBehaviour
{
    GameObject player;
    PlayerStats localStats;

    public GameObject [] objectToHide;
    public GameObject showBanner;
    public AudioSource backgroundMusic;
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
            Invoke("EndGame", 2f);
        }
    }


    void EndGame()
    {
        if (localStats.lost)
        {
            backgroundMusic.Stop();
            foreach(GameObject ss in objectToHide)
                ss.SetActive(false);
            showBanner.SetActive(true);
            Time.timeScale = 0f;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                restartScene();
            }
            
        } 
    }


    void restartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
