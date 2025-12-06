using UnityEngine;
using UnityEngine.Events;

public class PinableObject : MonoBehaviour
{
    public GameObject colliderWith;

    void Start()
    {

    }

    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("anchor"))
        {
            colliderWith = other.gameObject.GetComponentInChildren<Transform>()?.gameObject;
        }
    }
    public void onRelease(){
        if (colliderWith != null) {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.parent = colliderWith.transform;
            transform.localPosition = new Vector3(0, 0, 0);
        } else {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
