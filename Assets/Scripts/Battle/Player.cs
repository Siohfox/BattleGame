using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BG.Entity;
using BG.Battle;
using System;
using BG.Core;

public class Player : Entity 
{
    public enum State { Normal, Hidden, Energized };

    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text healthTextValue;
    [SerializeField] private TMP_Text playerEnergyTextValue;
    [SerializeField] private TMP_Text playerShieldTextValue;
    
    [SerializeField] private BattleManager battleManager;
    private AudioClip atkSoundClip;
    

    public GameObject playerStateIcon;

    public State[] playerState;

    private Animator playerAnimator;

    public int playerMaxEnergy;
    public int playerCurrentEnergy;

    public int playerCurrentShield;

    // Start is called before the first frame update
    void Start()
    {
        // Object refs
        healthBar = GetComponentInChildren<Slider>();
        healthTextValue = healthBar.GetComponentInChildren<TMP_Text>();
        playerAnimator = GetComponent<Animator>();
        shieldEquipClip = Resources.Load<AudioClip>("Sounds/ShieldEquip");
        atkSoundClip = Resources.Load<AudioClip>("Sounds/BasicWhack");
        basicHitFX = Resources.Load<GameObject>("Particles/BasicHitParticles");
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        // Variable assigns
        entityMaxHealth = GameState.Instance.playerMaxHP;
        entityCurrentHealth = GameState.Instance.playerCurrentHP;
        playerState = new State[2];
        playerState[0] = State.Normal;
        playerState[1] = State.Normal;

        // UI assigns
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

    public int GetPlayerCurrentHP()
    {
        return entityCurrentHealth;
    }

    public int GetPlayerMaxHP()
    {
        return entityMaxHealth;
    }

    // Update both player HP and health bar to match
    public void UpdateHealth(int currentHPChangeValue, int maxHPChangeValue)
    {
        // Update player hp:
        if(entityCurrentHealth <= entityMaxHealth)
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
        SfxPlayer.Instance.PlaySound(atkSoundClip, 1.0f);

        Instantiate(basicHitFX, battleManager.enemyObjects[0].transform.position, transform.rotation);

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
        SfxPlayer.Instance.PlaySound(shieldEquipClip, 1.0f);
    }
}
