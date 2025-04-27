using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;

    Enemy target;
    int   damage;

    public void Initialize(Enemy target, int damage)
    {
        this.target = target;
        this.damage = damage;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // move toward the target
        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // optional: rotate to face flight direction
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // collision check by distance
        if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
