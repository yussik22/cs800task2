using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackSkillSystem : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] private int killsPerSkill = 10;

    [Header("Visual Effect")]
    [SerializeField] private Color skillColor = Color.cyan;
    [SerializeField] private float effectTime = 0.7f;

    private int kills = 0;

    private AutomaticWeapon weapon;
    private SpriteRenderer spriteRenderer;

    private Coroutine effectCoroutine;


    private void Awake()
    {
        weapon =
            GetComponent<AutomaticWeapon>();

        spriteRenderer =
            GetComponent<SpriteRenderer>();
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


        if (kills >= killsPerSkill)
        {
            kills = 0;

            UnlockRandomSkill();
        }
    }


    private void UnlockRandomSkill()
    {
        List<AutomaticWeapon.AttackSkill>
            availableSkills =
            new List<AutomaticWeapon.AttackSkill>();


        foreach (
            AutomaticWeapon.AttackSkill skill
            in System.Enum.GetValues(
                typeof(AutomaticWeapon.AttackSkill)
            )
        )
        {
            if (!weapon.HasSkill(skill))
            {
                availableSkills.Add(skill);
            }
        }


        // Всі skills уже відкриті
        if (availableSkills.Count == 0)
        {
            Debug.Log(
                "ALL ATTACK SKILLS UNLOCKED!"
            );

            return;
        }


        int random =
            Random.Range(
                0,
                availableSkills.Count
            );


        AutomaticWeapon.AttackSkill
            selectedSkill =
            availableSkills[random];


        weapon.UnlockSkill(selectedSkill);


        Debug.Log(
            "SKILL UNLOCKED: "
            + selectedSkill
        );


        if (effectCoroutine != null)
        {
            StopCoroutine(
                effectCoroutine
            );
        }


        effectCoroutine =
            StartCoroutine(
                SkillEffect()
            );
    }


    private IEnumerator SkillEffect()
    {
        if (spriteRenderer == null)
            yield break;


        Color normalColor =
            spriteRenderer.color;


        // Яскравий skill effect
        spriteRenderer.color =
            skillColor;


        // Маленький scale punch
        Vector3 normalScale =
            transform.localScale;


        transform.localScale =
            normalScale * 1.3f;


        yield return new WaitForSeconds(
            effectTime * 0.3f
        );


        transform.localScale =
            normalScale;


        yield return new WaitForSeconds(
            effectTime * 0.7f
        );


        spriteRenderer.color =
            normalColor;


        effectCoroutine = null;
    }
}