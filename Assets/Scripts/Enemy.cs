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

    private AudioClip enemyAtkClip;
    private AudioClip enemyAtkMissClip;

    private Animator enemyAnimator;

    public int enemyAtkDamage;
    private bool hoverEnabled;

    // Start is called before the first frame update
    void Start()
    {
        // Object refs
        healthBar = GetComponentInChildren<Slider>();
        healthTextValue = healthBar.GetComponentInChildren<TMP_Text>();
        enemyAnimator = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Player>();
        enemyAtkClip = Resources.Load<AudioClip>("Sounds/BasicWhack");
        enemyAtkMissClip = Resources.Load<AudioClip>("Sounds/AttackMiss");
        basicHitFX = Resources.Load<GameObject>("Particles/BasicHitParticles");

        // Variable assigns
        entityMaxHealth = Random.Range(20, 60);
        entityCurrentHealth = entityMaxHealth;
        enemyAtkDamage = Random.Range(10, 20);
        hoverEnabled = false;

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

    // Update both entity HP and health bar to match
    public void UpdateHealth(int currentHPChangeValue, int maxHPChangeValue)
    {
        // Update entity hp:
        if (entityCurrentHealth <= entityMaxHealth)
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
                SfxPlayer.Instance.PlaySound(enemyAtkClip, 1.0f);
                Instantiate(basicHitFX, player.transform.position, transform.rotation);
                CalculateDamage();
            }
            else
            {
                SfxPlayer.Instance.PlaySound(enemyAtkMissClip, 1.0f);
                Debug.Log("Missed because player hidden");
            }
        }
        else
        {
            Instantiate(basicHitFX, player.transform.position, transform.rotation);
            SfxPlayer.Instance.PlaySound(enemyAtkClip, 1.0f);
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

    private void OnMouseEnter()
    {
        if (hoverEnabled)
        {
            GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        } 
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }
}
