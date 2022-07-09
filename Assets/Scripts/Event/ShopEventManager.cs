using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BG.Core;

public class ShopEventManager : MonoBehaviour
{
    public List<Button> optionButtons;
    [SerializeField] private Button objectButtonPrefab;
    [SerializeField] private GameObject optionButtonContainer;
    [SerializeField] private GameObject nextScreenButton;
    [SerializeField] private GameObject optionsButtonsHolder;
    private GameState gameState;

    int maxHpIncreasePrice;
    int maxHpIncreaseAmount;
    int actionPrice;
    int hpRegenPrice;
    int hpRegenAmount;

    private void Start()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();

        // Destroys all placeholders
        foreach (Transform child in GameObject.Find("BuyActionButtonsList").GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }


        // Set variables
        maxHpIncreasePrice = Random.Range(20, 26);
        maxHpIncreaseAmount = 5;
        actionPrice = Random.Range(25, 36);
        hpRegenPrice = Random.Range(8,11);
        hpRegenAmount = 8;

        GameObject.Find("BuyHealthButtons").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = $"Cost: {hpRegenPrice}";
        GameObject.Find("BuyMaxHealthButtons").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = $"Cost: {maxHpIncreasePrice}";


        StartCoroutine(LateStart());
        MapManager.Instance.mapClosable = true;
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        CreateOptionButtons();
    }

    /// <summary>
    /// Adds hp to player and updates UI
    /// </summary>
    public void BuyHealth()
    {
        if(gameState.playerCurrentHP < gameState.playerMaxHP)
        {
            if (gameState.bonesAmount >= hpRegenPrice)
            {
                gameState.AddBones(-hpRegenPrice);

                if(gameState.playerCurrentHP + hpRegenAmount > gameState.playerMaxHP)
                {
                    int dif = gameState.playerCurrentHP + hpRegenAmount - gameState.playerMaxHP;
                    gameState.ModifyHP(hpRegenAmount - dif);
                }
                else
                {
                    gameState.ModifyHP(hpRegenAmount);
                }
            }
        }
    }

    /// <summary>
    /// Adds max hp to player and updates UI
    /// </summary>
    public void BuyMaxHealth()
    {
        if (gameState.bonesAmount >= maxHpIncreasePrice)
        {
            gameState.AddBones(-maxHpIncreasePrice);

            gameState.ModifyHP(maxHpIncreaseAmount);
            gameState.ModifyMaxHP(maxHpIncreaseAmount);

            maxHpIncreasePrice += 5;

            // Update button text
            GameObject.Find("BuyMaxHealthButtons").transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = $"Cost: {maxHpIncreasePrice}";
        }
    }

    public void BuyAction(int _actionIndex, Button _actionBuyButton)
    {
        if (gameState.bonesAmount >= actionPrice)
        {
            gameState.AddBones(-actionPrice);

            //buy action
            gameState.UnlockNewAbility(gameState.actionList[_actionIndex].Index);
            Destroy(_actionBuyButton.gameObject);
        }
    }

    public void CreateOptionButtons()
    {
        // Before creating any new buttons, remove any null buttons (e.g buttons that have been destroyed)
        optionButtons.RemoveAll(s => s == null);

        // Create a list to store actions that have been used - this is to avoid dupes
        List<int> usedActions = new List<int>();

        // Create buttons
        int amountOfButtons = 3;
        for (int i = 0; i < amountOfButtons; i++)
        {
            OptionsButtons(i, usedActions);
        }
    }

    private void OptionsButtons(int _index, List<int> _usedActions)
    {
        // Instantiate button under parent transform buttonpos + space them out from left to right
        Button button = Instantiate(objectButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        button.transform.SetParent(optionsButtonsHolder.transform);
        button.transform.position = optionsButtonsHolder.transform.position + new Vector3(0, _index * -100, 0);

        // Create a random action and set it to a random int
        int randomAction = Random.Range(0, gameState.actionList.Count);

        // Create bool for loop
        bool notGood = true;
        int count = 0;

        // For each action known, check randomAction against the index.
        // If any single action is the same, notgood will stay true and break loop
        // Finally, if random action was any of the learnt actions, it'll reroll and try again
        while (notGood)
        {
            for (int i = 0; i < gameState.actionsLearnt.Count; i++)
            {
                if (randomAction == gameState.actionsLearnt[i].Index)
                {
                    notGood = true;
                    break;
                }
                else
                {
                    notGood = false;
                }
            }

            // If it's passed the actions learnt check:
            // Check if the action chosen is one chosen before instead
            if (!notGood)
            {
                for (int i = 0; i < _usedActions.Count; i++)
                {
                    if (randomAction == _usedActions[i])
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

            // If the action did not pass, reroll
            if (notGood)
            {
                randomAction = Random.Range(0, gameState.actionList.Count);
            }

            // Check if infinite loop, if so exit.
            count++;
            if (count >= 200)
            {
                notGood = false;
                Debug.LogWarning("Looped way too many times... likely due to not enough actions left");
            }
        }

        if(count < 200)
        {
            // Action picked gets added to the used actions list as to not be picked again
            _usedActions.Add(randomAction);

            // Assign button listeners and text
            button.onClick.AddListener(delegate { BuyAction(randomAction, button); });
            button.GetComponentInChildren<TMP_Text>().text = gameState.actionList[randomAction].Name;
            button.transform.GetChild(1).GetComponentInChildren<TMP_Text>().text = $"{actionPrice}";

            // Add button to a list of buttons so they can be removed and/or replaced later
            optionButtons.Add(button);
        }
        else
        {
            // UPGRADE BUTTON DEPLOY LATER
            Debug.Log("Destroying button");
            Destroy(button.gameObject);
        }
       
    }
}
