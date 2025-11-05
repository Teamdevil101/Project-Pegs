using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro; // Required for TextMeshPro

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Settings")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    public float scaleSpeed = 10f;

    [Header("Color Settings")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 0.5f, 0.5f); // pinkish-red
    public float colorLerpSpeed = 10f;

    [Header("Glow Settings")]
    public float glowIntensity = 0.2f; // how much brighter it glows
    public float glowSpeed = 2f;       // how fast it pulses

    private Vector3 targetScale;
    private Color targetColor;
    private Image buttonImage;
    private TMP_Text buttonText;
    private Color textNormalColor;
    private float glowTimer = 0f;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TMP_Text>();

        targetScale = normalScale;
        targetColor = normalColor;

        if (buttonImage != null)
            buttonImage.color = normalColor;

        if (buttonText != null)
        {
            textNormalColor = buttonText.color;
        }
    }

    void Update()
    {
        // Smooth scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);

        if (buttonImage != null)
        {
            Color baseColor = Color.Lerp(buttonImage.color, targetColor, Time.deltaTime * colorLerpSpeed);

            // Glow effect for button
            if (targetColor == hoverColor)
            {
                glowTimer += Time.deltaTime * glowSpeed;
                float glowFactor = Mathf.Sin(glowTimer) * glowIntensity;

                baseColor.r = Mathf.Clamp01(baseColor.r + glowFactor);
                baseColor.g = Mathf.Clamp01(baseColor.g + glowFactor);
                baseColor.b = Mathf.Clamp01(baseColor.b + glowFactor);
            }

            buttonImage.color = baseColor;
        }

        // Glow for text
        if (buttonText != null)
        {
            Color targetTextColor = (targetColor == hoverColor) ? hoverColor : textNormalColor;

            if (targetColor == hoverColor)
            {
                float glowFactor = Mathf.Sin(glowTimer) * glowIntensity;
                targetTextColor.r = Mathf.Clamp01(targetTextColor.r + glowFactor);
                targetTextColor.g = Mathf.Clamp01(targetTextColor.g + glowFactor);
                targetTextColor.b = Mathf.Clamp01(targetTextColor.b + glowFactor);
            }

            buttonText.color = Color.Lerp(buttonText.color, targetTextColor, Time.deltaTime * colorLerpSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale;
        targetColor = hoverColor;
        glowTimer = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = normalScale;
        targetColor = normalColor;
    }
}
