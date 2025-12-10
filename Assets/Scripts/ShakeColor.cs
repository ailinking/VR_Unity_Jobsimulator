using UnityEngine;
using UnityEngine.Events;

public class LiquidShaker : MonoBehaviour
{
    public float shakeThreshold = 2.5f;
    public float chargeSpeed = 1.5f;
    public float decaySpeed = 0.8f;
    public Renderer targetRenderer;
    public Material mixedMaterial;
    public UnityEvent OnLiquidMixed;

    private Vector3 lastVelocity;
    private Vector3 lastPosition;
    private float currentCharge = 0f;
    private bool isMixed = false;

    private void Awake()
    {
        if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();
        lastPosition = transform.position;
    }
    private void Start()
    {
    }
    private void Update()
    {
    }

    private void FixedUpdate()
    {
        if (isMixed) return;
        CalculateShakePhysics();
    }

    private void CalculateShakePhysics()
    {
        Vector3 currentVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        Vector3 acceleration = (currentVelocity - lastVelocity) / Time.fixedDeltaTime;
        float accelMagnitude = acceleration.magnitude;
        if (accelMagnitude > shakeThreshold)
        {
            currentCharge += Time.fixedDeltaTime * chargeSpeed;
        }
        else
        {
            currentCharge -= Time.fixedDeltaTime * decaySpeed;
        }
        currentCharge = Mathf.Clamp01(currentCharge);
        if (currentCharge >= 1.0f)
        {
            CompleteMixing();
        }
        lastPosition = transform.position;
        lastVelocity = currentVelocity;
    }

    private void CompleteMixing()
    {
        isMixed = true;
        if (targetRenderer != null && mixedMaterial != null)
        {
            targetRenderer.material = mixedMaterial;
        }
        OnLiquidMixed?.Invoke();
    }

    public void ResetLiquid()
    {
        isMixed = false;
        currentCharge = 0f;
    }
}
