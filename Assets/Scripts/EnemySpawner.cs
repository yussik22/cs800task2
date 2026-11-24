using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Distance")]
    [SerializeField] private float minSpawnDistance = 8f;
    [SerializeField] private float maxSpawnDistance = 12f;

    [Header("Difficulty Progression")]
    [SerializeField] private float progressionTime = 180f;

    [SerializeField] private float startSpawnInterval = 2f;
    [SerializeField] private float minSpawnInterval = 0.5f;

    [SerializeField] private int startMaxEnemies = 15;
    [SerializeField] private int endMaxEnemies = 60;

    private float timer;
    private float gameTime;

    private void Update()
    {
        gameTime += Time.deltaTime;
        timer += Time.deltaTime;

        float difficulty = Mathf.Clamp01(gameTime / progressionTime);

        // Spawn стає частішим
        float currentSpawnInterval = Mathf.Lerp(
            startSpawnInterval,
            minSpawnInterval,
            difficulty
        );

        // Дозволена кількість ворогів збільшується
        int currentMaxEnemies = Mathf.RoundToInt(
            Mathf.Lerp(
                startMaxEnemies,
                endMaxEnemies,
                difficulty
            )
        );

        if (timer >= currentSpawnInterval)
        {
            int enemyCount =
                GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (enemyCount < currentMaxEnemies)
            {
                SpawnEnemy(difficulty);
            }

            timer = 0f;
        }
    }

    private void SpawnEnemy(float difficulty)
    {
        Vector2 direction =
            Random.insideUnitCircle.normalized;

        float distance = Random.Range(
            minSpawnDistance,
            maxSpawnDistance
        );

        Vector2 spawnPosition =
            PlayerMovement.playerPosition +
            direction * distance;

        GameObject enemy = Instantiate(
            enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );

        EnemyType enemyType =
            enemy.GetComponent<EnemyType>();

        EnemyType.Type type =
            GetRandomEnemyType(difficulty);

        enemyType.SetType(type);
    }

    private EnemyType.Type GetRandomEnemyType(float difficulty)
    {
        // На початку:
        // Normal 80%
        // Fast   15%
        // Tank    5%

        // Наприкінці:
        // Normal 40%
        // Fast   35%
        // Tank   25%

        float fastChance =
            Mathf.Lerp(0.15f, 0.35f, difficulty);

        float tankChance =
            Mathf.Lerp(0.05f, 0.25f, difficulty);

        float random = Random.value;

        if (random < tankChance)
        {
            return EnemyType.Type.Tank;
        }

        if (random < tankChance + fastChance)
        {
            return EnemyType.Type.Fast;
        }

        return EnemyType.Type.Normal;
    }
}