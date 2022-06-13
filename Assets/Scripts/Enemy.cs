using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BG.Entity;
using BG.Battle;

public class Enemy : Entity
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthTextValue;
    [SerializeField] private TMP_Text actionQuantityValText;

    [SerializeField] private Player player;

    private Animator enemyAnimator;

    public int enemyAtkDamage;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<Slider>();
        healthTextValue = healthBar.GetComponentInChildren<TMP_Text>();
        enemyAnimator = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Player>();

        entityMaxHealth = 70;
        entityCurrentHealth = 10;

        enemyAtkDamage = Random.Range(10, 20);

        healthTextValue.text = entityCurrentHealth.ToString() + "/" + entityMaxHealth.ToString();
        healthBar.maxValue = entityMaxHealth;
        healthBar.value = entityCurrentHealth;

        actionQuantityValText.text = enemyAtkDamage.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (entityCurrentHealth <= 0)
        {
            OnDeath();
        }
    }

    // Update both player HP and health bar to match
    public void UpdateHealth(int currentHPChangeValue, int maxHPChangeValue)
    {
        // Update player hp:
        if (entityCurrentHealth < entityMaxHealth)
        {
            entityCurrentHealth += currentHPChangeValue;
        }
        entityMaxHealth += maxHPChangeValue;

        // Update health bar:
        healthBar.maxValue = entityMaxHealth;
        healthBar.value = entityCurrentHealth;
        healthTextValue.text = entityCurrentHealth.ToString() + "/" + entityMaxHealth.ToString();
    }

    public void Attack()
    {

        enemyAnimator.SetBool("EnemyAttacking", true);

        if(player.playerState[0] == Player.State.Hidden)
        {
            int hitOrMiss = Random.Range(0, 4);
            if(hitOrMiss < 1) // 20% chance of being hit
            {
                Debug.Log("Player found, hitting anyway");
                CalculateDamage();
            }
            else
            {
                Debug.Log("Missed because player hidden");
            }
        }
        else
        {
            CalculateDamage();
        }
        
        StartCoroutine(EnemyAnim());
    }

    void CalculateDamage()
    {
        if (enemyAtkDamage > player.playerCurrentShield)
        {
            // find dif between atk dmg and players shield. (13 attack - 8 shield = 5 leftover)
            int dif = enemyAtkDamage - player.playerCurrentShield;

            // remove shield entirely
            player.UpdateShield(-player.playerCurrentShield);

            // apply leftover damage
            player.UpdateHealth(-dif, 0);
        }
        else if (enemyAtkDamage <= player.playerCurrentShield)
        {
            player.UpdateShield(-enemyAtkDamage);
        }
    }

    IEnumerator EnemyAnim()
    {
        yield return new WaitUntil(() => enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("EnemyBasicAtk"));

        enemyAnimator.SetBool("EnemyAttacking", false);
    }

    public void UpdateAttack(int AtkChange)
    {
        if (AtkChange == 0)
        {
            enemyAtkDamage = Random.Range(10, 20);
        }
        else
        {
            enemyAtkDamage += AtkChange;
        }

        if (enemyAtkDamage < 0)
        {
            enemyAtkDamage = 0;
        }

        actionQuantityValText.text = enemyAtkDamage.ToString();
    }

    void OnDeath()
    {
        // remove self from remaining enemies list
        BattleManager battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        battleManager.enemyObjects.Remove(gameObject);

        battleManager.BattleEndCheck();

        Destroy(gameObject);
    }
}
