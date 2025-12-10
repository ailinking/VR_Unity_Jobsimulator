using UnityEngine;

public class SimpleJoystick : MonoBehaviour
{
    public Transform pivot;
    public bool freezeX = true;
    public bool freezeY = true;
    public bool freezeZ = false;
    public float maxAngle = 45f;
    public float returnSpeed = 10f;
    public bool invert;
    public Transform hand;
    public bool dragging;
    public float value;

    private Quaternion rest;

    private void Awake()
    {
        rest = transform.rotation;
    }

    private void Update()
    {
        if (dragging && hand != null)
        {
            int ax = ActiveAxis();
            Vector3 axisLocal = ax == 0 ? Vector3.right : ax == 1 ? Vector3.up : Vector3.forward;
            Vector3 axisWorld = pivot != null ? pivot.TransformDirection(axisLocal) : transform.TransformDirection(axisLocal);
            Vector3 p = pivot != null ? pivot.position : transform.position;
            Vector3 dir = hand.position - p;
            Vector3 refDir = transform.position - p;
            Vector3 dProj = Vector3.ProjectOnPlane(dir, axisWorld);
            Vector3 rProj = Vector3.ProjectOnPlane(refDir, axisWorld);
            if (dProj.sqrMagnitude > 0.0001f && rProj.sqrMagnitude > 0.0001f)
            {
                float a = Vector3.SignedAngle(rProj, dProj, axisWorld);
                a = Mathf.Clamp(a * (invert ? -1f : 1f), -maxAngle, maxAngle);
                transform.rotation = Quaternion.AngleAxis(a, axisWorld) * rest;
                value = a / maxAngle;
            }
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rest, Time.deltaTime * returnSpeed);
        }
    }

    public void BeginDrag(Transform h)
    {
        hand = h;
        dragging = true;
    }

    public void EndDrag()
    {
        hand = null;
        dragging = false;
    }

    private int ActiveAxis()
    {
        bool x = !freezeX;
        bool y = !freezeY;
        bool z = !freezeZ;
        if (x && !y && !z) return 0;
        if (!x && y && !z) return 1;
        if (!x && !y && z) return 2;
        return 2;
    }

    private float NormalizeAngle(float a)
    {
        if (a > 180f) a -= 360f;
        return a;
    }
}
