using UnityEngine;
using TMPro;

public class LampController : MonoBehaviour
{
    public GameObject lamp;
    public bool isOn;
    public int currentLetter = 0;
    public string textToWrite = "hello world";
    public TextMeshProUGUI textObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchLamp()
    {
        lamp.SetActive(!lamp.activeSelf);
        if (isOn == true)
        {
            isOn = false;
            lamp.SetActive(false);
        }
        else
        {
            isOn = true;
            lamp.SetActive(true);
        }
    }
    public void TypeText()
    {
        textObj.text += textToWrite[currentLetter];
        currentLetter++;
    }
}
