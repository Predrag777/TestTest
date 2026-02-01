using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] TMP_Text text;       // tekst koji će blinkovati i zatamniti
    [SerializeField] Image fadeImage;     // crni fullscreen image za fade out (UI Image)
    [SerializeField] float fadeDuration = 1.5f; // trajanje fade-a u sekundama
    [SerializeField] float blinkSpeed = 2f;     // brzina blinkanja

    private bool isFading = false;
    private bool isBlinking = true;

    void Start()
    {
        // Postavi crni Image potpuno providnim na startu
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        // Start blinking korutine
        if (text != null)
            StartCoroutine(BlinkText());
    }

    void Update()
    {
        if (!isFading && Input.GetKeyDown(KeyCode.Space))
        {
            isBlinking = false; // zaustavi blinkanje
            StartCoroutine(FadeAndLoadScene("SampleScene"));
        }
    }

    private IEnumerator BlinkText()
    {
        Color originalColor = text.color;

        while (isBlinking)
        {
            float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f; // 0..1
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Kada se blinkanje završi, zadrži punu providnost pre fade-a
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        isFading = true;

        float elapsed = 0f;

        Color textColor = text.color;
        Color imageColor = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            if (text != null)
            {
                text.color = new Color(textColor.r, textColor.g, textColor.b, Mathf.Lerp(textColor.a, 0f, t));
            }

            if (fadeImage != null)
            {
                fadeImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, Mathf.Lerp(0f, 1f, t));
            }

            yield return null;
        }

        // Osiguraj da su završne vrednosti tačne
        if (text != null)
            text.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
        if (fadeImage != null)
            fadeImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, 1f);

        // Promeni scenu
        SceneManager.LoadScene(sceneName);
    }
}
