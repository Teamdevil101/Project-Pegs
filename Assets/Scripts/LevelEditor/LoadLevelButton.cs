using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadLevelButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI levelTitleElement;
    [SerializeField]
    private TextMeshProUGUI levelPathElement;
    [SerializeField]
    private TextMeshProUGUI lastModifiedElement;
    
    public Button button;

    public void SetElements(string title, string path, string date)
    {
        levelTitleElement.text = title;
        levelPathElement.text = path;
        lastModifiedElement.text = date;
    }
}