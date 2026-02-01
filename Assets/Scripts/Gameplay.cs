using UnityEngine;
using UnityEngine.SceneManagement;
public class Gameplay : MonoBehaviour
{
    GameObject player;
    PlayerStats localStats;

    public GameObject [] objectToHide;
    public GameObject showBanner;
    public GameObject showWinBanner;
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                restartGame();
            }
        }

         if (localStats.isArthurDead)
        {
            Invoke("EndGameWin", 2f);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                restartGame();
            }
        }
    }


    void EndGameWin()
    {
        if (localStats.isArthurDead)
        {
            backgroundMusic.Stop();
            foreach(GameObject ss in objectToHide)
                ss.SetActive(false);
            showWinBanner.SetActive(true);
            Time.timeScale = 0f;
 
            
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
 
            
        } 
    }


    void restartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void restartGame()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

}
