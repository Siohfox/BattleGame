using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BG.Entity;
using BG.Battle;

public class Enemy : Entity
{
    // References
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthTextValue;
    [SerializeField] private TMP_Text actionQuantityValText;
    [SerializeField] private SpriteRenderer actionQuantitySprite;
    private Player player;
    private Animator enemyAnimator;
    public TMP_Text enemyShieldTextValue;

    // AudioClips
    private AudioClip enemyAtkClip;
    private AudioClip enemyAtkMissClip;
    
    // Variables
    public int enemyHealthMinimum, enemyHealthMaximum; // Should be assigned in inspector
    public int enemyAtkDamageMinimum, enemyAtkDamageMaximum; // Should be assigned in inspector

    [System.NonSerialized] public int enemyAtkDamage;
    [System.NonSerialized] public int enemyDefence;
    [System.NonSerialized] public int enemyCurrentDefence;
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
        shieldEquipClip = Resources.Load<AudioClip>("Sounds/ShieldEquip");

        // Variable assigns
        entityMaxHealth = Random.Range(enemyHealthMinimum, enemyHealthMaximum);
        entityCurrentHealth = entityMaxHealth;
        enemyAtkDamage = Random.Range(enemyAtkDamageMinimum, enemyAtkDamageMaximum);
        enemyDefence = 0;
        UpdateDefence(0);
        hoverEnabled = false;

        healthTextValue.text = entityCurrentHealth.ToString() + "/" + entityMaxHealth.ToString();
        healthBar.maxValue = entityMaxHealth;
        healthBar.value = entityCurrentHealth;

        actionQuantityValText.text = enemyAtkDamage.ToString();
        actionQuantitySprite.sprite = Resources.Load<Sprite>("Textures/AtkIcon");
        enemyShieldTextValue.text = enemyCurrentDefence.ToString();
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
                Instantiate(basicHitFX, player.transform.position, transform.rotation);
                CalculateDamage();
            }
            else
            {
                SfxPlayer.Instance.PlaySound(enemyAtkMissClip, 1.0f);
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

    public void Defend()
    {
        Instantiate(shieldPrefab, transform);
        SfxPlayer.Instance.PlaySound(shieldEquipClip, 1.0f);
        enemyCurrentDefence = enemyDefence;
        enemyShieldTextValue.text = enemyCurrentDefence.ToString();
    }

    void CalculateDamage()
    {
        if (enemyAtkDamage > player.playerCurrentShield)
        {
            // Play atk clip
            SfxPlayer.Instance.PlaySound(enemyAtkClip, 1.0f);

            // find dif between atk dmg and players shield. (13 attack - 8 shield = 5 leftover)
            int dif = enemyAtkDamage - player.playerCurrentShield;

            // remove shield entirely
            player.UpdateShield(-player.playerCurrentShield);

            // apply leftover damage
            player.UpdateHealth(-dif, 0);
        }
        else if (enemyAtkDamage <= player.playerCurrentShield)
        {
            SfxPlayer.Instance.PlaySound(Resources.Load<AudioClip>("Sounds/Clang"), 1.0f);
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
        // If enemy attack is being modified, apply change, else pick a random atk range
        if (AtkChange == 0)
        {
            enemyAtkDamage = Random.Range(10, 20);
        }
        else
        {
            enemyAtkDamage += AtkChange;
        }

        // Make sure enemy attack can't be negative
        if (enemyAtkDamage < 0)
        {
            enemyAtkDamage = 0;
        }

        // Show new attack under the enemy
        actionQuantityValText.text = enemyAtkDamage.ToString();
        actionQuantitySprite.sprite = Resources.Load<Sprite>("Textures/AtkIcon");
    }

    public void UpdateDefence(int _defenceChange)
    {
        // If enemy defence is being modified, apply change, else pick a random defence range
        if (_defenceChange == 0)
        {
            enemyDefence = Random.Range(10, 15);
        }
        else
        {
            enemyDefence += _defenceChange;
        }

        // Make sure enemy defence can't be negative
        if (enemyDefence < 0)
        {
            enemyDefence = 0;
        }

        // Show new defence under the enemy
        actionQuantityValText.text = enemyDefence.ToString();
        actionQuantitySprite.sprite = Resources.Load<Sprite>("Textures/SmallShieldIcon");
    }

    public void UpdateShield(int shieldAmount)
    {
        enemyCurrentDefence += shieldAmount;

        enemyShieldTextValue.text = enemyCurrentDefence.ToString();
    }

    void OnDeath()
    {
        // remove self from remaining enemies list
        BattleManager battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        battleManager.enemyObjects.Remove(this);

        // Since an enemy has died, check if there are any enemies left and if not end battle
        battleManager.BattleEndCheck();

        // Destroy self
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
