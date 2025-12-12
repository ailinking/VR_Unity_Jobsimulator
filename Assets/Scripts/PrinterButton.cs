using UnityEngine;
using UnityEngine.Events;

public class PrinterButton : MonoBehaviour
{
    public GameObject buttonTop;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    public PrinterTray tray;
    public Transform outputPoint;
    public float ejectSpeed = 2f;
    public enum Axis { X, Y, Z }
    public Axis slideAxis = Axis.Z;
    public float slideAmount = 0.15f;
    public float slideTime = 0.2f;
    public bool reverseSlide;
    public Transform spawnPoint;
    public float ejectTime = 0.6f;
    private Vector3 outputInitial;
    private bool hasOutputInitial;
    private Coroutine slideRoutine;

    void Start()
    {
        if (outputPoint != null) { outputInitial = outputPoint.localPosition; hasOutputInitial = true; }
    }

    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        print("printer button was pressed");
        buttonTop.transform.localPosition = new Vector3(0, -0.2f, 0);
        onPress.Invoke();
        PrintCopy();
    }

    private void OnTriggerExit(Collider other)
    {
        print("printer button is released");
        buttonTop.transform.localPosition = new Vector3(0, 0, 0);
        onRelease.Invoke();
    }

    public void PrintCopy()
    {
        if (tray == null) return;
        if (outputPoint == null) return;
        GameObject src = tray.currentObject;
        if (src == null) return;
        if (!hasOutputInitial) { outputInitial = outputPoint.localPosition; hasOutputInitial = true; }
        if (slideRoutine != null) StopCoroutine(slideRoutine);
        slideRoutine = StartCoroutine(SlideAndPrint(src));
    }

    private System.Collections.IEnumerator SlideAndPrint(GameObject src)
    {
        Vector3 start = outputInitial;
        float amt = reverseSlide ? -slideAmount : slideAmount;
        Vector3 target;
        if (slideAxis == Axis.X) target = new Vector3(start.x + amt, start.y, start.z);
        else if (slideAxis == Axis.Y) target = new Vector3(start.x, start.y + amt, start.z);
        else target = new Vector3(start.x, start.y, start.z + amt);
        float t = 0f;
        while (t < slideTime)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / slideTime);
            outputPoint.localPosition = Vector3.LerpUnclamped(start, target, u);
            yield return null;
        }
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : outputPoint.position;
        Quaternion spawnRot = spawnPoint != null ? spawnPoint.rotation : outputPoint.rotation;
        GameObject clone = Instantiate(src, spawnPos, spawnRot);
        yield return EjectToOutput(clone);
        Rigidbody rb = clone.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = outputPoint.forward * ejectSpeed;
        print("printed a copy");
        t = 0f;
        while (t < slideTime)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / slideTime);
            outputPoint.localPosition = Vector3.LerpUnclamped(target, start, u);
            yield return null;
        }
        outputPoint.localPosition = start;
        slideRoutine = null;
    }

    private System.Collections.IEnumerator EjectToOutput(GameObject obj)
    {
        Vector3 a = obj.transform.position;
        Vector3 b = outputPoint.position;
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        bool hadRb = rb != null;
        if (hadRb) rb.isKinematic = true;
        float t = 0f;
        while (t < ejectTime)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / ejectTime);
            obj.transform.position = Vector3.LerpUnclamped(a, b, u);
            yield return null;
        }
        obj.transform.position = b;
        if (hadRb) rb.isKinematic = false;
    }
}
