using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;

public class VRButton : MonoBehaviour
{
    public GameObject buttonTop;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    public Material isOnMat, isOffMat;
    public bool isOn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        print("button was pressed");
        buttonTop.transform.localPosition = new Vector3(0, -0.2f, 0);
        onPress.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        print("button is released");
        buttonTop.transform.localPosition = new Vector3(0, 0, 0); onRelease.Invoke();
    }

    public void SwitchColor()
    {
        if (isOn)
        {
            buttonTop.GetComponent<MeshRenderer>().material = isOffMat;
        }
        else
        {
            buttonTop.GetComponent<MeshRenderer>().material = isOnMat;
        }
        isOn = !isOn;
    }
}

