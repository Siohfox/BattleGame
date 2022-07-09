using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BG.Core;

public class EventManager : MonoBehaviour
{
    public List<Button> optionButtons;
    [SerializeField] private Button objectButtonPrefab;
    [SerializeField] private GameObject optionButtonContainer;
    [SerializeField] private GameObject nextScreenButton;
    [SerializeField] private GameObject optionsButtonsHolder;
    private GameState gameState;

    private void Start()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();

        CreateOptionButtons();

        StartCoroutine(Test());

        MapManager.Instance.mapClosable = true;
    }

    IEnumerator Test()
    {
        yield return new WaitForEndOfFrame();
        
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
        while(notGood)
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
            if(count >= 200)
            {
                notGood = false;
                Debug.LogWarning("Looped way too many times... likely due to not enough actions left");
            }
        }

        // Action picked gets added to the used actions list as to not be picked again
        _usedActions.Add(randomAction);
        
        // Assign button listeners and text
        button.onClick.AddListener(delegate { gameState.UnlockNewAbility(gameState.actionList[randomAction].Index);});
        button.onClick.AddListener(FinishEvent);
        button.GetComponentInChildren<TMP_Text>().text = gameState.actionList[randomAction].Name;

        // Add button to a list of buttons so they can be removed and/or replaced later
        optionButtons.Add(button);
    }

    private void FinishEvent()
    {
        optionButtonContainer.SetActive(false);
        nextScreenButton.SetActive(true);
    }
}
