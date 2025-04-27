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

    public void Init(float speed) => this.speed = speed;

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
        checkpoint = WaveManager.I.checkpoints[index];

        if (Vector2.Distance(transform.position, checkpoint.position) <= 0.1f)
        {
            index++;
            if (index >= WaveManager.I.checkpoints.Length)
            {
                Destroy(gameObject);
                return;
            }
            checkpoint = WaveManager.I.checkpoints[index];
        }

        // move only—no rotation
        Vector2 dir = ((Vector2)checkpoint.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * speed;
                if (dir.x < -0.01f)
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
        else if (dir.x > 0.01f)
            transform.localScale = new Vector3( originalScaleX, transform.localScale.y, transform.localScale.z);
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
        rb.linearVelocity = Vector2.zero;        // stop moving
        anim.SetTrigger("Die");            // fire your transition
        StartCoroutine( DestroyAfterDeath() );
    }

    private IEnumerator DestroyAfterDeath()
    {
        // wait for the dying animation length
        var info = anim.GetCurrentAnimatorStateInfo(0);
        float delay = info.length;
        // in case the state just changed, you might also sample the clip length directly:
        // delay = anim.runtimeAnimatorController.animationClips
        //     .First(c => c.name == "YourDyingClipName").length;
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
