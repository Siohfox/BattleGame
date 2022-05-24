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

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<Slider>();
        healthTextValue = healthBar.GetComponentInChildren<TMP_Text>();

        entityMaxHealth = 70;
        entityCurrentHealth = 5;

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

    void OnDeath()
    {
        // remove self from 
        GameObject.Find("BattleManager").GetComponent<BattleManager>().enemyObjects.Remove(gameObject);

        Destroy(gameObject);
    }
}
