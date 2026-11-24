using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float lifetime = 5f;

    [Header("Explosion")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float explosionDamageMultiplier = 0.6f;

    [SerializeField]
    private ParticleSystem explosionEffect;

    [Header("Split")]
    [SerializeField] private float splitAngle = 30f;

    [Header("Ricochet")]
    [SerializeField] private float ricochetRange = 6f;
    [SerializeField] private int maxRicochets = 2;

    [Header("Piercing")]
    [SerializeField] private int maxPierceHits = 3;


    private Rigidbody2D rb;

    private float damage;
    private float speed;

    private GameObject projectilePrefab;

    private bool piercing;
    private bool split;
    private bool explosion;
    private bool ricochet;

    private bool alreadySplit = false;

    private int remainingPierceHits;
    private int ricochetsRemaining;

    // Щоб одна куля не била одного Enemy кілька разів
    private HashSet<int> hitEnemies =
        new HashSet<int>();


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    public void Initialize(
        Vector2 direction,
        float newSpeed,
        float newDamage,
        GameObject prefab,
        bool hasPiercing,
        bool hasSplit,
        bool hasExplosion,
        bool hasRicochet
    )
    {
        speed = newSpeed;
        damage = newDamage;

        projectilePrefab = prefab;

        piercing = hasPiercing;
        split = hasSplit;
        explosion = hasExplosion;
        ricochet = hasRicochet;

        remainingPierceHits =
            piercing ? maxPierceHits : 1;

        ricochetsRemaining =
            ricochet ? maxRicochets : 0;

        rb.linearVelocity =
            direction.normalized * speed;

        Destroy(gameObject, lifetime);
    }


    private void OnTriggerEnter2D(
        Collider2D collision
    )
    {
        if (!collision.CompareTag("Enemy"))
            return;


        int enemyID =
            collision.gameObject.GetInstanceID();


        // Уже били цього Enemy
        if (hitEnemies.Contains(enemyID))
            return;


        hitEnemies.Add(enemyID);


        Health health =
            collision.GetComponent<Health>();


        if (health != null)
        {
            health.TakeDamage(damage);
        }


        // ===========================
        // EXPLOSION
        // ===========================

        if (explosion)
        {
            Explode(collision);
        }


        // ===========================
        // SPLIT
        // ===========================

        if (split && !alreadySplit)
        {
            alreadySplit = true;

            SplitProjectile();
        }


        remainingPierceHits--;


        // ===========================
        // RICOCHET
        // ===========================

        if (ricochet &&
            ricochetsRemaining > 0)
        {
            Transform nextEnemy =
                FindRicochetTarget();

            if (nextEnemy != null)
            {
                Vector2 newDirection =
                    (
                        nextEnemy.position -
                        transform.position
                    ).normalized;

                rb.linearVelocity =
                    newDirection * speed;

                ricochetsRemaining--;

                return;
            }
        }


        // ===========================
        // PIERCING
        // ===========================

        if (piercing &&
            remainingPierceHits > 0)
        {
            return;
        }


        Destroy(gameObject);
    }


    // ===========================
    // EXPLOSION
    // ===========================

    private void Explode(
        Collider2D originalTarget
    )
    {
        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(
                transform.position,
                explosionRadius
            );


        foreach (Collider2D enemy in enemies)
        {
            if (!enemy.CompareTag("Enemy"))
                continue;


            // Основний Enemy вже отримав normal damage
            if (enemy == originalTarget)
                continue;


            Health enemyHealth =
                enemy.GetComponent<Health>();


            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(
                    damage *
                    explosionDamageMultiplier
                );
            }
        }


        // Візуальний explosion effect
        if (explosionEffect != null)
        {
            ParticleSystem effect =
                Instantiate(
                    explosionEffect,
                    transform.position,
                    Quaternion.identity
                );

            effect.Play();

            Destroy(
                effect.gameObject,
                2f
            );
        }
    }


    // ===========================
    // SPLIT
    // ===========================

    private void SplitProjectile()
    {
        Vector2 currentDirection =
            rb.linearVelocity.normalized;


        Vector2 direction1 =
            RotateVector(
                currentDirection,
                splitAngle
            );


        Vector2 direction2 =
            RotateVector(
                currentDirection,
                -splitAngle
            );


        SpawnSplitProjectile(direction1);
        SpawnSplitProjectile(direction2);
    }


    private void SpawnSplitProjectile(
        Vector2 direction
    )
    {
        GameObject newProjectile =
            Instantiate(
                projectilePrefab,
                transform.position,
                Quaternion.identity
            );


        Projectile projectile =
            newProjectile.GetComponent<Projectile>();


        projectile.Initialize(
            direction,
            speed,
            damage,
            projectilePrefab,

            // Split projectile теж може piercing
            piercing,

            // Але більше не ділиться
            false,

            // Може вибухати
            explosion,

            // І може ricochet
            ricochet
        );
    }


    // ===========================
    // RICOCHET
    // ===========================

    private Transform FindRicochetTarget()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag(
                "Enemy"
            );


        Transform nearest = null;

        float nearestDistance =
            ricochetRange;


        foreach (GameObject enemy in enemies)
        {
            int id =
                enemy.GetInstanceID();


            // Не повертаємося до вже пробитих
            if (hitEnemies.Contains(id))
                continue;


            float distance =
                Vector2.Distance(
                    transform.position,
                    enemy.transform.position
                );


            if (distance < nearestDistance)
            {
                nearestDistance =
                    distance;

                nearest =
                    enemy.transform;
            }
        }


        return nearest;
    }


    // ===========================
    // ROTATION
    // ===========================

    private Vector2 RotateVector(
        Vector2 vector,
        float angle
    )
    {
        float radians =
            angle * Mathf.Deg2Rad;


        float cos =
            Mathf.Cos(radians);

        float sin =
            Mathf.Sin(radians);


        return new Vector2(
            vector.x * cos -
            vector.y * sin,

            vector.x * sin +
            vector.y * cos
        );
    }


    // Показує radius вибуху в Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}