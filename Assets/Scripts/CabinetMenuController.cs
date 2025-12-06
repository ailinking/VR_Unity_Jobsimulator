using UnityEngine;
using System.Collections;

public class CabinetMenuController : MonoBehaviour
{
    public Transform knob;
    public Transform doorTop;
    public Transform doorBottom;
    public int levelCount = 5;
    public Transform[] levelTransforms;
    public Collider[] leverSegments;
    public float probeRadius = 0.05f;
    public string segmentTag = "CabinetMenu";
    public float forwardOffset = 0.2f;
    public float downOffset = 0.2f;
    public float doorTime = 0.25f;
    public float contentTime = 0.2f;
    public Vector3 knobTopLocal;
    public Vector3 knobBottomLocal;
    public Vector3 doorTopClosedLocal;
    public Vector3 doorTopOpenLocal;
    public Vector3 doorBottomClosedLocal;
    public Vector3 doorBottomOpenLocal;

    private Vector3[] levelBasePos;
    private int currentIndex = -1;
    private Coroutine routine;

    void Start()
    {
        if (levelTransforms != null)
        {
            levelBasePos = new Vector3[levelTransforms.Length];
            for (int i = 0; i < levelTransforms.Length; i++)
            {
                if (levelTransforms[i] != null) levelBasePos[i] = levelTransforms[i].localPosition;
            }
        }
    }

    void Update()
    {
        if (knob == null || levelCount <= 0) return;
        int idx = -1;
        if (leverSegments != null && leverSegments.Length > 0)
        {
            Collider[] hits = Physics.OverlapSphere(knob.position, probeRadius);
            for (int i = 0; i < leverSegments.Length; i++)
            {
                Collider c = leverSegments[i];
                if (c == null) continue;
                for (int h = 0; h < hits.Length; h++)
                {
                    Collider hit = hits[h];
                    if (!hit.CompareTag(segmentTag)) continue;
                    if (hit == c) { print("trigger level and it's number: " + i); idx = i; break; }
                }
                if (idx >= 0) break;
            }
        }
        if (idx >= 0 && idx != currentIndex && routine == null) SelectIndex(idx);
    }

    public void SelectIndex(int idx)
    {
        if (levelTransforms == null || idx < 0 || idx >= levelTransforms.Length) return;
        print("select level: " + idx);
        routine = StartCoroutine(SwitchRoutine(idx));
    }

    private IEnumerator SwitchRoutine(int idx)
    {
        Transform prev = currentIndex >= 0 && currentIndex < levelTransforms.Length ? levelTransforms[currentIndex] : null;
        Transform next = levelTransforms[idx];
        if (doorTop != null || doorBottom != null) print("closing doors");
        yield return AnimatePair(doorTop, doorTop.localPosition, doorTopClosedLocal, doorBottom, doorBottom.localPosition, doorBottomClosedLocal, doorTime);
        if (prev != null)
        {
            Vector3 pBase = levelBasePos[currentIndex];
            Vector3 pDown = pBase + new Vector3(0f, -downOffset, 0f);
            yield return AnimateLocal(prev, prev.localPosition, pDown, contentTime);
        }
        for (int i = 0; i < levelTransforms.Length; i++)
        {
            if (levelTransforms[i] == null) continue;
            levelTransforms[i].localPosition = levelBasePos[i];
        }
        Vector3 nBase = levelBasePos[idx];
        Vector3 nFront = nBase + new Vector3(0f, 0f, forwardOffset);
        yield return AnimateLocal(next, next.localPosition, nFront, contentTime);
        currentIndex = idx;
        if (doorTop != null || doorBottom != null) print("opening doors");
        yield return AnimatePair(doorTop, doorTop.localPosition, doorTopOpenLocal, doorBottom, doorBottom.localPosition, doorBottomOpenLocal, doorTime);
        routine = null;
    }

    private IEnumerator AnimateLocal(Transform tr, Vector3 a, Vector3 b, float time)
    {
        if (tr == null) yield break;
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / time);
            tr.localPosition = Vector3.LerpUnclamped(a, b, u);
            yield return null;
        }
        tr.localPosition = b;
    }

    private IEnumerator AnimatePair(Transform aTr, Vector3 a, Vector3 b, Transform bTr, Vector3 c, Vector3 d, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / time);
            if (aTr != null) aTr.localPosition = Vector3.LerpUnclamped(a, b, u);
            if (bTr != null) bTr.localPosition = Vector3.LerpUnclamped(c, d, u);
            yield return null;
        }
        if (aTr != null) aTr.localPosition = b;
        if (bTr != null) bTr.localPosition = d;
    }
}
