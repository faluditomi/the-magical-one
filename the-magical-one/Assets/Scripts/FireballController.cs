using UnityEngine;

public class FireballController : MonoBehaviour
{
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform fireballSpawnPoint;

    public void ShootFireball(float speed, float radius)
    {
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);

        fireball.AddComponent<Fireball>().Initialize(speed, radius);
    }
}

public class Fireball : MonoBehaviour
{
    private float speed;
    private float radius;
    private float lifetime = 5f;

    public void Initialize(float speed, float radius)
    {
        this.speed = speed;
        this.radius = radius;
        
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
    }
}
