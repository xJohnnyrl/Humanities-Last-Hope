using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public bool rotateTowardsTarget = true;  // << ADD THIS

    private Enemy target;
    private int   damage;

    public void Initialize(Enemy target, int damage)
    {
        this.target = target;
        this.damage = damage;
    }

    void Update()
    {
        if (target == null) { Destroy(gameObject); return; }

        Vector3 start = transform.position;
        Vector3 end   = target.transform.position;
        float step    = speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(start, end, step);

        if (rotateTowardsTarget)
        {
            Vector3 dir = end - start;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var e = other.GetComponent<Enemy>();
        if (e == null) return;

        e.TakeDamage(damage);
        Destroy(gameObject);
    }
}
