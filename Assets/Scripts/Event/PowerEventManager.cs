using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BG.Core;
using System.Linq;

public class PowerEventManager : MonoBehaviour
{
    [SerializeField] private Button powerButtonPrefab;
    [SerializeField] private GameObject powerButtonContainer;
    [SerializeField] private GameObject nextScreenButton;
    private GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();

        CreatePowerButton();
        nextScreenButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreatePowerButton()
    {
        // Create a list to store powers that have been used - this is to avoid dupes
        List<int> usedPowers = new List<int>();

        // Create button
        Button button = Instantiate(powerButtonPrefab, powerButtonContainer.transform.position, Quaternion.identity, powerButtonContainer.transform);

        int lastPower = System.Enum.GetValues(typeof(PowersEnum)).Cast<int>().Max();
        int randomPower = Random.Range(0, lastPower);

        // Create bool for loop
        bool notGood = true;
        int count = 0;

        // For each action known, check randomPower against the index.
        // If any single power is the same, notgood will stay true and break loop
        // Finally, if random power was any of the learnt actions, it'll reroll and try again
        while (notGood)
        {
            for (int i = 0; i < gameState.powersKnown.Count; i++)
            {
                if (randomPower == gameState.powersKnown[i].Index)
                {
                    notGood = true;
                    break;
                }
                else
                {
                    notGood = false;
                }
            }

            // If it's passed the power learnt check:
            // Check if the power chosen is one chosen before instead
            if (!notGood)
            {
                for (int i = 0; i < usedPowers.Count; i++)
                {
                    if (randomPower == usedPowers[i])
                    {
                        notGood = true;
                        break;
                    }
                    else
                    {
                        notGood = false;
                    }
                }
            }

            // If the power did not pass, reroll
            if (notGood)
            {
                randomPower = Random.Range(0, gameState.powerList.Count);
            }

            // Check if infinite loop, if so exit.
            count++;
            if (count >= 200)
            {
                notGood = false;
                Debug.LogWarning("Looped way too many times... likely due to not enough powers left");
            }
        }

        // Power picked gets added to the used power list as to not be picked again (this is for rendering multiple buttons though)
        usedPowers.Add(randomPower);

        // Assign button listeners and text
        button.onClick.AddListener(delegate { gameState.UnlockNewPower(gameState.powerList[randomPower].Index); });
        button.onClick.AddListener(FinishEvent);
        button.GetComponentInChildren<TMP_Text>().text = gameState.powerList[randomPower].Name;
        button.name = $"PwrBtn {gameState.powerList[randomPower].Name}";
    }

    private void FinishEvent()
    {
        powerButtonContainer.SetActive(false);
        nextScreenButton.SetActive(true);
    }
}
