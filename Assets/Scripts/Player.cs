using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BG.Entity;
using System;

public class Player : Entity 
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthTextValue;
    [SerializeField] private TMP_Text playerEnergyTextValue;
    [SerializeField] private TMP_Text playerShieldTextValue;
    [SerializeField] private GameObject shieldPrefab;

    private Animator playerAnimator;

    public int playerMaxEnergy;
    public int playerCurrentEnergy;

    public int playerCurrentShield;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<Slider>();
        healthTextValue = healthBar.GetComponentInChildren<TMP_Text>();
        playerAnimator = GetComponent<Animator>();

        entityMaxHealth = 100;
        entityCurrentHealth = 50;

        healthTextValue.text = entityCurrentHealth.ToString() + "/" + entityMaxHealth.ToString();
        healthBar.maxValue = entityMaxHealth;
        healthBar.value = entityCurrentHealth;

        UpdateEnergy(5, 5);
        UpdateShield(0);

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

    public void UpdateEnergy(int currentEnergy, int maxEnergy)
    {
        playerCurrentEnergy += currentEnergy;
        playerMaxEnergy += maxEnergy;

        playerEnergyTextValue.text = playerCurrentEnergy.ToString() + "/" + playerMaxEnergy.ToString();
    }

    public void UpdateShield(int shieldAmount)
    {
        playerCurrentShield += shieldAmount;

        playerShieldTextValue.text = playerCurrentShield.ToString();
    }

    public void Attack()
    {
        playerAnimator.SetBool("Attacking", true);

        StartCoroutine(AtkAnim());
    }

    IEnumerator AtkAnim()
    {
        yield return new WaitUntil(() => playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerBasicAtk"));

        playerAnimator.SetBool("Attacking", false);
    }

    // Shields will stay attached to player... remember to remove later
    public void Shield()
    {
        Instantiate(shieldPrefab, transform);
    }
}
