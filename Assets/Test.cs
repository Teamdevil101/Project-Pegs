using UnityEngine;

public class Test : MonoBehaviour
{
    public string text;
    public int number;
    public float floatNumber;

    public GameObject p_gameObject;
    private AudioSource audioSource;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = p_gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
