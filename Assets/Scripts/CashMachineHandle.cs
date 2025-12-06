using UnityEngine;

public class CashMachineHandle : MonoBehaviour
{
    public Transform handle;
    public Transform drawer;
    public enum Axis { X, Y, Z }
    public Axis axis = Axis.Z;
    public float moveAmount = 0.2f;
    public bool reverse;
    public float thresholdDegrees = 30f;
    private Vector3 drawerInitialPos;
    private Vector3 handleInitialEuler;
    private bool hasInitial;
    public bool isOpen;

    void Start()
    {
        if (handle != null) handleInitialEuler = handle.localEulerAngles;
        if (drawer != null) drawerInitialPos = drawer.localPosition;
        hasInitial = handle != null && drawer != null;
    }

    void Update()
    {
        if (handle == null || drawer == null) return;
        if (!hasInitial)
        {
            handleInitialEuler = handle.localEulerAngles;
            drawerInitialPos = drawer.localPosition;
            hasInitial = true;
        }
        float current = GetAxis(handle.localEulerAngles);
        float initial = GetAxis(handleInitialEuler);
        float delta = NormalizeAngle(current - initial);
        float amt = reverse ? -moveAmount : moveAmount;
        if (!isOpen && Mathf.Abs(delta) >= thresholdDegrees)
        {
            Vector3 p = drawerInitialPos;
            if (axis == Axis.X) p = new Vector3(drawerInitialPos.x + amt, drawerInitialPos.y, drawerInitialPos.z);
            else if (axis == Axis.Y) p = new Vector3(drawerInitialPos.x, drawerInitialPos.y + amt, drawerInitialPos.z);
            else p = new Vector3(drawerInitialPos.x, drawerInitialPos.y, drawerInitialPos.z + amt);
            drawer.localPosition = p;
            isOpen = true;
        }
        else if (isOpen && Mathf.Abs(delta) < thresholdDegrees)
        {
            drawer.localPosition = drawerInitialPos;
            isOpen = false;
        }
    }

    private float GetAxis(Vector3 e)
    {
        if (axis == Axis.X) return e.x;
        if (axis == Axis.Y) return e.y;
        return e.z;
    }

    private float NormalizeAngle(float a)
    {
        a = Mathf.Repeat(a + 180f, 360f) - 180f;
        return a;
    }
}
