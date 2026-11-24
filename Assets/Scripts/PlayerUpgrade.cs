using UnityEngine;
using System.Collections;

public class PlayerUpgrade : MonoBehaviour
{
    [Header("Level Up")]
    [SerializeField] private int killsPerUpgrade = 5;

    [Header("Upgrade Values")]
    [SerializeField] private float speedMultiplier = 1.1f;
    [SerializeField] private float damageMultiplier = 1.2f;
    [SerializeField] private float attackIntervalMultiplier = 0.9f;
    [SerializeField] private float healthIncrease = 10f;

    [Header("Level Up Effect")]
    [SerializeField] private Color levelUpColor = Color.green;
    [SerializeField] private float effectTime = 0.4f;

    private int kills = 0;
    private int level = 1;

    private PlayerMovement playerMovement;
    private AutomaticWeapon weapon;
    private Health health;
    private SpriteRenderer spriteRenderer;

    private Coroutine effectCoroutine;


    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        weapon = GetComponent<AutomaticWeapon>();
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void OnEnable()
    {
        Health.OnEnemyDied += EnemyKilled;
    }


    private void OnDisable()
    {
        Health.OnEnemyDied -= EnemyKilled;
    }


    private void EnemyKilled()
    {
        kills++;

        if (kills >= killsPerUpgrade)
        {
            kills = 0;

            LevelUp();
        }
    }


    private void LevelUp()
    {
        level++;

        Debug.Log("LEVEL UP! Level: " + level);

        // Movement upgrade
        if (playerMovement != null)
        {
            playerMovement.MultiplySpeed(
                speedMultiplier
            );
        }


        // Weapon upgrade
        if (weapon != null)
        {
            weapon.UpgradeWeapon(
                damageMultiplier,
                attackIntervalMultiplier
            );
        }


        // Health upgrade
        if (health != null)
        {
            health.IncreaseMaxHealth(
                healthIncrease
            );
        }


        // Visual effect
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        effectCoroutine =
            StartCoroutine(LevelUpEffect());
    }


    private IEnumerator LevelUpEffect()
    {
        if (spriteRenderer == null)
            yield break;

        Color oldColor =
            spriteRenderer.color;

        spriteRenderer.color =
            levelUpColor;

        yield return new WaitForSeconds(
            effectTime
        );

        spriteRenderer.color =
            oldColor;

        effectCoroutine = null;
    }
}