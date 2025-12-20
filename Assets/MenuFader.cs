using UnityEngine;
using System.Collections;

public class MenuFader : MonoBehaviour
{
    public CanvasGroup canvasGroup; // reference to the canvas group
    public float fadeDuration = 1.2f; // how long it takes to fade in

    void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;


        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f; // ensure it ends fully visible
    }
}
