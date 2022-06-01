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

    [SerializeField] private GameObject player;

    private Animator enemyAnimator;

    public int enemyAtkDamage;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<Slider>();
        healthTextValue = healthBar.GetComponentInChildren<TMP_Text>();
        enemyAnimator = GetComponent<Animator>();
        player = GameObject.Find("Player");

        entityMaxHealth = 70;
        entityCurrentHealth = 50;

        enemyAtkDamage = Random.Range(10, 20);

        healthTextValue.text = entityCurrentHealth.ToString() + "/" + entityMaxHealth.ToString();
        healthBar.maxValue = entityMaxHealth;
        healthBar.value = entityCurrentHealth;
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

        player.GetComponent<Player>().UpdateHealth(-enemyAtkDamage, 0);

        StartCoroutine(enemyAnim());
    }

    IEnumerator enemyAnim()
    {
        //WaitUntil(() => enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("EnemyBasicAtk"));
        yield return new WaitForSeconds(2);

        enemyAnimator.SetBool("EnemyAttacking", false);
    }

    void OnDeath()
    {
        // remove self from 
        GameObject.Find("BattleManager").GetComponent<BattleManager>().enemyObjects.Remove(gameObject);

        Destroy(gameObject);
    }
}
