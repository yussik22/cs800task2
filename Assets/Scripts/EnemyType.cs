using UnityEngine;

public class EnemyType : MonoBehaviour
{
    public enum Type
    {
        Normal,
        Fast,
        Tank
    }

    [SerializeField] private Type enemyType;

    private EnemyMovement movement;
    private EnemyDamage enemyDamage;
    private Health health;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        enemyDamage = GetComponent<EnemyDamage>();
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetType(Type type)
    {
        enemyType = type;

        switch (enemyType)
        {
            case Type.Normal:
                movement.SetSpeed(2f);
                health.SetMaxHealth(100f);
                enemyDamage.SetDamage(10f);
                spriteRenderer.color = Color.white;
                break;

            case Type.Fast:
                movement.SetSpeed(4f);
                health.SetMaxHealth(60f);
                enemyDamage.SetDamage(7f);
                spriteRenderer.color = Color.yellow;
                break;

            case Type.Tank:
                movement.SetSpeed(1f);
                health.SetMaxHealth(300f);
                enemyDamage.SetDamage(20f);
                spriteRenderer.color = Color.red;
                break;
        }
    }
}