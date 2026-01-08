using TMPro;
using UnityEngine;

public class CustomVector3InputField : MonoBehaviour
{
    public TMP_InputField x_inputField;
    public TMP_InputField y_inputField;
    public TMP_InputField z_inputField;

    public void SetValue(Vector3 v3)
    {
        x_inputField.text = v3.x.ToString();
        y_inputField.text = v3.y.ToString();
        z_inputField.text = v3.z.ToString();
    }
}