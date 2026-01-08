using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SectionHeaderRow : MonoBehaviour
{
    public TextMeshProUGUI labelText;

    public Transform sectionContentRoot;
    public Image buttonImage;

    public Sprite collapsedSprite;
    public Sprite openSprite;

    public void Setup(string title)
    {
        labelText.text = title;
    }

    private bool collapsed;

    public void ToggleCollapse()
    {
        collapsed = !collapsed;

        if(buttonImage != null)
            if(collapsed)
                buttonImage.sprite = collapsedSprite;
            else
                buttonImage.sprite = openSprite;

        if (sectionContentRoot != null)
        {
            sectionContentRoot.gameObject.SetActive(!collapsed);
        }
    }
}