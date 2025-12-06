using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CashMachineButton : MonoBehaviour
{
    public GameObject buttonTop;
    public UnityEvent onPress;
    public Transform drawer;
    public enum Axis { X, Y, Z }
    public Axis axis = Axis.Z;
    public float moveAmount = 0.2f;
    public bool reverse;
    public bool isOpen;
    private Vector3 initialPos;
    private bool isPressing;
    private bool hasInitial;
    public bool isCloseButton;
    public float moveTime = 0.35f;
    public float bounceFraction = 0.1f;
    public float bounceTime = 0.08f;
    public bool enableBounce = true;
    private Coroutine moveRoutine;
    public AudioSource audioSource;

    void Start()
    {
        if (drawer != null) { initialPos = drawer.localPosition; hasInitial = true; }
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPressing) return;
        isPressing = true;
        print("button was pressed");
        buttonTop.transform.localPosition = new Vector3(0, -0.2f, 0);
        onPress.Invoke();
        if (audioSource != null) audioSource.Play();
        if (isCloseButton) CloseDrawer(); else OpenDrawer();
    }

    private void OnTriggerExit(Collider other)
    {
        isPressing = false;
        print("button is released");
        buttonTop.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void OpenDrawer()
    {
        if (drawer == null) return;
        if (!hasInitial) { initialPos = drawer.localPosition; hasInitial = true; }
        float amt = reverse ? -moveAmount : moveAmount;
        Vector3 openPos;
        if (axis == Axis.X) openPos = new Vector3(initialPos.x + amt, initialPos.y, initialPos.z);
        else if (axis == Axis.Y) openPos = new Vector3(initialPos.x, initialPos.y + amt, initialPos.z);
        else openPos = new Vector3(initialPos.x, initialPos.y, initialPos.z + amt);
        isOpen = true;
        MoveTo(openPos);
    }

    public void CloseDrawer()
    {
        if (drawer == null) return;
        if (!hasInitial) { initialPos = drawer.localPosition; hasInitial = true; }
        isOpen = false;
        MoveTo(initialPos);
    }

    private void MoveTo(Vector3 target)
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(AnimateTo(target));
    }

    private IEnumerator AnimateTo(Vector3 target)
    {
        Vector3 start = drawer.localPosition;
        Vector3 dir = target - start;
        float dist = dir.magnitude;
        if (dist < 0.00001f) yield break;
        if (enableBounce && bounceFraction > 0f && bounceTime > 0f)
        {
            Vector3 pre = start - dir.normalized * (dist * bounceFraction);
            yield return AnimateSegment(start, pre, bounceTime);
            start = drawer.localPosition;
            dir = target - start;
            dist = dir.magnitude;
            if (dist < 0.00001f) yield break;
        }
        yield return AnimateSegment(start, target, moveTime);
    }

    private IEnumerator AnimateSegment(Vector3 a, Vector3 b, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / time);
            drawer.localPosition = Vector3.LerpUnclamped(a, b, u);
            yield return null;
        }
        drawer.localPosition = b;
    }

}
