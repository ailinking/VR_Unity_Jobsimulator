using UnityEngine;

public class PrinterTray : MonoBehaviour
{
    public GameObject currentObject;
    private void OnTriggerEnter(Collider other)
    {
        currentObject = other.gameObject;
    }
    private void OnTriggerExit(Collider other)
    {
        if (currentObject == other.gameObject) currentObject = null;
    }
}
