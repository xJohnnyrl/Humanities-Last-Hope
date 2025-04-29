using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public static Enemy I { get; private set; }

    [SerializeField] public float speed = 5f;
    [SerializeField] private int maxHealth = 3;
    private Rigidbody2D rb;
    private Animator  anim;
    private Transform checkpoint;
    private int index = 0;
    private int currentHealth;
    private bool isDying = false;
    private SpriteRenderer spriteRenderer;
    private float originalScaleX;

    private int rewardCoins = 1;
    private int damage = 1;

    public void Init(float speed, int rewardCoins, int damage, float hpMultiplier)
    {
        this.speed = speed;
        this.rewardCoins = rewardCoins;
        this.damage = damage;
        this.maxHealth = Mathf.RoundToInt(this.maxHealth * hpMultiplier); // ✅ scale HP
        this.currentHealth = maxHealth;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalScaleX = transform.localScale.x;
    }

    void Start()
    {
        checkpoint = WaveManager.I.checkpoints[index];
    }

void Update()
{
    if (isDying) return;

    if (index >= WaveManager.I.checkpoints.Length)
        return;

    checkpoint = WaveManager.I.checkpoints[index];

    Vector2 dir = ((Vector2)checkpoint.position - (Vector2)transform.position).normalized;
    rb.linearVelocity = dir * Mathf.Min(speed, 20f);

    // Flip sprite only if moving mostly horizontally
    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
    {
        if (dir.x < 0f)
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
    }

    // 🚀 Check simple distance to checkpoint
    if (Vector2.Distance(transform.position, checkpoint.position) < 0.2f)
    {
        index++;

        // If we're out of checkpoints -> reached end
        if (index >= WaveManager.I.checkpoints.Length)
        {
            GameManager.I.DamagePlayer(damage);
            Destroy(gameObject);
        }
    }

    bool walking = rb.linearVelocity.sqrMagnitude > 0.01f;
    anim.SetBool("isWalking", walking);
}

    public void TakeDamage(int amount)
    {
        if (isDying) return;
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDying = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Die");
        GameManager.I.EarnCoins(rewardCoins);  // << now reward by type
        StartCoroutine(DestroyAfterDeath());
    }

    private IEnumerator DestroyAfterDeath()
    {
        float delay = 0f;
        foreach (var clip in anim.runtimeAnimatorController.animationClips)
            if (clip.name == "Dying")
                delay = clip.length;
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (WaveManager.I != null)
            WaveManager.I.NotifyEnemyDeath(this);
    }

    [ContextMenu("Test Die")]
    private void TestDie()
    {
        TakeDamage(maxHealth);
    }

}
