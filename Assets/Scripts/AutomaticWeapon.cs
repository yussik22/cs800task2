using UnityEngine;

public class AutomaticWeapon : MonoBehaviour
{
    public enum AttackSkill
    {
        Piercing,
        Split,
        Explosion,
        Ricochet
    }

    [Header("Weapon")]
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float projectileSpeed = 8f;

    [Header("Limits")]
    [SerializeField] private float minimumAttackInterval = 0.15f;

    private float attackTimer;

    // Skills
    private bool piercing;
    private bool split;
    private bool explosion;
    private bool ricochet;


    private void Update()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            Transform target = FindNearestEnemy();

            if (target != null)
            {
                Shoot(target);
                attackTimer = 0f;
            }
        }
    }


    private Transform FindNearestEnemy()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        Transform nearest = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }


    private void Shoot(Transform target)
    {
        Vector2 direction =
            (target.position - transform.position).normalized;

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Initialize(
                direction,
                projectileSpeed,
                damage,
                projectilePrefab,
                piercing,
                split,
                explosion,
                ricochet
            );
        }
    }


    // ===========================
    // NORMAL LEVEL UP
    // ===========================

    public void UpgradeWeapon(
        float damageMultiplier,
        float intervalMultiplier
    )
    {
        damage *= damageMultiplier;

        attackInterval *= intervalMultiplier;

        attackInterval = Mathf.Max(
            attackInterval,
            minimumAttackInterval
        );
    }


    // ===========================
    // SKILLS
    // ===========================

    public void UnlockSkill(AttackSkill skill)
    {
        switch (skill)
        {
            case AttackSkill.Piercing:
                piercing = true;
                break;

            case AttackSkill.Split:
                split = true;
                break;

            case AttackSkill.Explosion:
                explosion = true;
                break;

            case AttackSkill.Ricochet:
                ricochet = true;
                break;
        }

        Debug.Log("NEW SKILL: " + skill);
    }


    public bool HasSkill(AttackSkill skill)
    {
        switch (skill)
        {
            case AttackSkill.Piercing:
                return piercing;

            case AttackSkill.Split:
                return split;

            case AttackSkill.Explosion:
                return explosion;

            case AttackSkill.Ricochet:
                return ricochet;
        }

        return false;
    }
}