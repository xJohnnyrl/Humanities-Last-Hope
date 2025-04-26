using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D rb;
    private Transform checkpoint;
    private int index = 0;

    public void Init(float speed) => this.speed = speed;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Start()
    {
        checkpoint = WaveManager.I.checkpoints[index];
    }

    void Update()
    {
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

        // move & face target
        Vector2 dir = (checkpoint.position - transform.position).normalized;
        rb.linearVelocity = dir * speed;
        transform.right = dir;
    }

    private void OnDestroy()
    {
        if (WaveManager.I != null)
            WaveManager.I.NotifyEnemyDeath(this);
    }
}
