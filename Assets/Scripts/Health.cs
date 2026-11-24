using UnityEngine;
using System;
using System.Collections;

public class Health : MonoBehaviour
{
    // Event-based score
    public static event Action OnEnemyDied;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }

    [Header("Hit Feedback")]
    [SerializeField] private Color flashColor = Color.black;
    [SerializeField] private float flashTime = 0.1f;

    [SerializeField] private float punchScale = 1.5f;
    [SerializeField] private float punchTime = 0.15f;

    [Header("Death Polish")]
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float deathAnimationTime = 0.3f;

    private SpriteRenderer spriteRenderer;

    private Color normalColor;
    private Vector3 normalScale;

    private Coroutine hitFeedbackCoroutine;

    private bool isDead = false;


    private void Awake()
    {
        CurrentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();

        normalScale = transform.localScale;

        if (spriteRenderer != null)
        {
            normalColor = spriteRenderer.color;
        }
    }


    // =============================
    // DAMAGE
    // =============================

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (damage <= 0)
            return;

        CurrentHealth -= damage;

        if (CurrentHealth > 0)
        {
            PlayHitFeedback();
        }
        else
        {
            CurrentHealth = 0;
            Die();
        }
    }


    // =============================
    // HEAL
    // =============================

    public void Heal(float amount)
    {
        if (isDead)
            return;

        if (amount <= 0)
            return;

        CurrentHealth += amount;

        if (CurrentHealth > maxHealth)
        {
            CurrentHealth = maxHealth;
        }
    }


    // Для EnemyType
    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        CurrentHealth = maxHealth;
    }

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;

        CurrentHealth += amount;

        if (CurrentHealth > maxHealth)
        {
            CurrentHealth = maxHealth;
        }
    }

    // =============================
    // HIT FEEDBACK
    // =============================

    private void PlayHitFeedback()
    {
        if (hitFeedbackCoroutine != null)
        {
            StopCoroutine(hitFeedbackCoroutine);

            transform.localScale = normalScale;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = normalColor;
            }
        }

        // Запам'ятовуємо поточний колір
        // Normal / Fast / Tank
        if (spriteRenderer != null)
        {
            normalColor = spriteRenderer.color;
        }

        hitFeedbackCoroutine =
            StartCoroutine(HitFeedback());
    }


    private IEnumerator HitFeedback()
    {
        // Flash
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
        }

        // Scale Punch
        transform.localScale =
            normalScale * punchScale;

        yield return new WaitForSeconds(punchTime);

        transform.localScale = normalScale;

        if (flashTime > punchTime)
        {
            yield return new WaitForSeconds(
                flashTime - punchTime
            );
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }

        hitFeedbackCoroutine = null;
    }


    // =============================
    // DEATH
    // =============================

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;


        // =============================
        // ENEMY
        // =============================

        if (CompareTag("Enemy"))
        {
            // EVENT
            // GameManager сам почує цю подію
            OnEnemyDied?.Invoke();

            StartCoroutine(EnemyDeath());
        }


        // =============================
        // PLAYER
        // =============================

        else if (CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
        }
    }


    // =============================
    // ENEMY DEATH
    // =============================

    private IEnumerator EnemyDeath()
    {
        // Зупиняємо hit feedback
        if (hitFeedbackCoroutine != null)
        {
            StopCoroutine(hitFeedbackCoroutine);

            hitFeedbackCoroutine = null;
        }


        // =============================
        // STOP MOVEMENT
        // =============================

        EnemyMovement movement =
            GetComponent<EnemyMovement>();

        if (movement != null)
        {
            movement.enabled = false;
        }


        Rigidbody2D rb =
            GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }


        // =============================
        // DISABLE DAMAGE
        // =============================

        EnemyDamage enemyDamage =
            GetComponent<EnemyDamage>();

        if (enemyDamage != null)
        {
            enemyDamage.enabled = false;
        }


        // =============================
        // DISABLE COLLIDER
        // =============================

        Collider2D col =
            GetComponent<Collider2D>();

        if (col != null)
        {
            col.enabled = false;
        }


        // =============================
        // PARTICLES
        // =============================

        if (deathParticles != null)
        {
            ParticleSystem particles =
                Instantiate(
                    deathParticles,
                    transform.position,
                    Quaternion.identity
                );

            particles.Play();

            ParticleSystem.MainModule main =
                particles.main;

            Destroy(
                particles.gameObject,
                main.duration +
                main.startLifetime.constantMax
            );
        }


        // =============================
        // SOUND
        // =============================

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(
                deathSound,
                transform.position
            );
        }


        // =============================
        // DEATH ANIMATION
        // =============================

        Vector3 startScale =
            normalScale;

        Color startColor =
            Color.white;

        if (spriteRenderer != null)
        {
            startColor = normalColor;

            spriteRenderer.color =
                startColor;
        }


        float timer = 0f;


        while (timer < deathAnimationTime)
        {
            timer += Time.deltaTime;

            float t =
                timer / deathAnimationTime;


            // =============================
            // SCALE ANIMATION
            // =============================

            float scale;

            if (t < 0.25f)
            {
                // Спочатку трохи збільшуємо
                scale = Mathf.Lerp(
                    1f,
                    1.3f,
                    t / 0.25f
                );
            }
            else
            {
                // Потім стискаємо до 0
                scale = Mathf.Lerp(
                    1.3f,
                    0f,
                    (t - 0.25f) / 0.75f
                );
            }

            transform.localScale =
                startScale * scale;


            // =============================
            // FADE
            // =============================

            if (spriteRenderer != null)
            {
                Color color =
                    startColor;

                color.a =
                    1f - t;

                spriteRenderer.color =
                    color;
            }


            yield return null;
        }


        Destroy(gameObject);
    }
}