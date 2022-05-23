using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public enum Action { Bite, Scratch, Defend, Hide, Howl };

    private int playerMaxEnergy;
    private int playerCurrentEnergy;

    private GameObject playerObject;
    private List<GameObject> enemyObjects;

    [SerializeField] private TMP_Text playerEnergyTextValue;

    // Start is called before the first frame update
    void Start()
    {
        UpdateEnergy(5, 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateEnergy(int currentEnergy, int maxEnergy)
    {
        playerCurrentEnergy += currentEnergy;
        playerMaxEnergy += maxEnergy;

        playerEnergyTextValue.text = playerCurrentEnergy.ToString() + "/" + playerMaxEnergy.ToString();
    }

    public void UseEnergy(Action action)
    {
        switch (action)
        {
            case Action.Bite:
                // bite
                break;

            case Action.Scratch:
                // bite
                break;

            default:
                Debug.LogError("Failed to use proper action when using energy");
                break;
        }
    }
}
