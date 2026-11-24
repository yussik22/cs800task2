using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackInterval = 1f;

    private float lastAttackTime = -999f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time >= lastAttackTime + attackInterval)
        {
            Health health = collision.gameObject.GetComponent<Health>();

            if (health != null)
            {
                health.TakeDamage(damage);
                lastAttackTime = Time.time;
            }
        }
    }
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }
}