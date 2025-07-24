using UnityEngine;

public class BulletObject : MonoBehaviour
{
    public float startForce;
    public float speed = 50f;
    public float lifeTime = 5f;
    public float damage = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * (speed + startForce);        

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        startForce -= Time.deltaTime * lifeTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
