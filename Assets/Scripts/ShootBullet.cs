using UnityEngine;

public class ShootBullet : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletStart;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TestActivate()
    {
        print("Activate Pressed!");
        GameObject newBullet = Instantiate(bulletPrefab, bulletStart.position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody>().linearVelocity = bulletStart.forward * 15;
    }
}