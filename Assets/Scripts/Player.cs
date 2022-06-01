using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BG.Entity;

public class Player : Entity 
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthTextValue;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<Slider>();
        healthTextValue = healthBar.GetComponentInChildren<TMP_Text>();

        entityMaxHealth = 100;
        entityCurrentHealth = 2;

        healthTextValue.text = entityCurrentHealth.ToString() + "/" + entityMaxHealth.ToString();
        healthBar.maxValue = entityMaxHealth;
        healthBar.value = entityCurrentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            UpdateHealth(-1, 0);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UpdateHealth(0, 1);
        }

        if (entityCurrentHealth <= 0)
        {
            OnDeath();
        }
    }

    // Update both player HP and health bar to match
    public void UpdateHealth(int currentHPChangeValue, int maxHPChangeValue)
    {
        // Update player hp:
        if(entityCurrentHealth < entityMaxHealth)
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
        // Show Death Sprite
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/DeadCircle");
        // GameState.gameover
    }
}
