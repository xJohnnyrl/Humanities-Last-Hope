using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Stats")]
    public float range     = 3f;
    public int   damage    = 1;
    public float fireRate  = 1f;        // shots per second
       private RangeIndicator rangeIndicator;

    [Header("References")]
    public Transform firePoint;         // empty child where projectiles spawn
    public GameObject projectilePrefab; // your “ball” effect

    Animator        anim;
    SpriteRenderer  spriteRenderer;
    float           cooldown = 0f;
    LayerMask       enemyMask;

    void Awake()
    {
        anim           = GetComponent<Animator>();
        // assume your Enemy objects sit on layer named “Enemy”
        enemyMask      = LayerMask.GetMask("Enemy");
        rangeIndicator = GetComponent<RangeIndicator>();
    }

    void Start()
    {
        if (rangeIndicator == null)
        {
            Debug.LogWarning($"[{name}] no RangeIndicator found!");
            return;
        }

        // 1) set the actual tower range
        rangeIndicator.radius = range / transform.lossyScale.x;

        // 2) rebuild the circle mesh
        rangeIndicator.DrawCircle();

        // 3) hide it until we click
        rangeIndicator.Hide();
    }
    void Update()
    {
    cooldown -= Time.deltaTime;

    if (cooldown <= 0f)
    {
        Enemy target = FindNearestEnemy();
        if (target != null)
        {
            Debug.Log($"[Tower] cooldown ready, found target = {target}");
            Shoot(target);
        }
        else
        {
            anim.ResetTrigger("Attack");
            anim.Play("Idle", 0);
        }

        // Reset cooldown AFTER trying
        cooldown = 1f / fireRate;
    }
    }

Enemy FindNearestEnemy()
{
    Vector2 center = (rangeIndicator.rangeOrigin != null)
        ? (Vector2)rangeIndicator.rangeOrigin.position
        : (Vector2)transform.position;

    Collider2D[] hits = Physics2D.OverlapCircleAll(center, range); // no scaling here!

    Enemy nearest = null;
    float bestSq = float.MaxValue;

    foreach (var hit in hits)
    {
        if (!hit.CompareTag("Enemy")) continue;

        Enemy e = hit.GetComponent<Enemy>() ?? hit.GetComponentInParent<Enemy>();
        if (e == null) continue;

        float sq = (e.transform.position - (Vector3)center).sqrMagnitude;
        if (sq < bestSq)
        {
            bestSq = sq;
            nearest = e;
        }
    }

    return nearest;
}

void Shoot(Enemy target)
{
    // flip sprite to face target
    bool left = target.transform.position.x < transform.position.x;
        
    Vector3 s = transform.localScale;
    s.x = Mathf.Abs(s.x) * (left ? -1 : 1);
    transform.localScale = s;

    anim.SetTrigger("Attack");

    if (projectilePrefab && firePoint)
    {
        var go = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        var p = go.GetComponent<Projectile>();
        if (p != null) p.Initialize(target, damage);

        // 👉 FLIP the projectile based on tower facing
        Vector3 projScale = go.transform.localScale;
        projScale.x = Mathf.Abs(projScale.x) * (left ? -1 : 1);
        go.transform.localScale = projScale;
    }
}

    // visualize range in the editor
    void OnDrawGizmosSelected()
    {
        if (rangeIndicator == null) return;

        Vector3 center = (rangeIndicator.rangeOrigin != null)
            ? rangeIndicator.rangeOrigin.position
            : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, rangeIndicator.radius);
    }

    void OnMouseDown()
    {
        // toggle ring on click
        if (rangeIndicator != null)
            rangeIndicator.Toggle();
    }
}
