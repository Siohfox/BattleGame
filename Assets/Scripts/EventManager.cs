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
    private GameState gameState;

    private void Start()
    {
        gameState = GameObject.Find("GameState").GetComponent<GameState>();

        CreateOptionButtons();
    }

    public void CreateOptionButtons()
    {
        // Before creating any new buttons, remove any null buttons (e.g buttons that have been destroyed)
        optionButtons.RemoveAll(s => s == null);

        // Create a list to store actions that have been used
        List<int> usedActions = new List<int>();

        // Create buttons
        int amountOfButtons = 3;
        for (int i = 0; i < amountOfButtons; i++)
        {
            OptionsButtons(i);
        }
        
    }

    private void OptionsButtons(int _index)
    {
        // Instantiate button under parent transform buttonpos + space them out from left to right
        Button button = Instantiate(objectButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);     
        button.transform.SetParent(GameObject.Find("OptionButtons").transform);
        button.transform.position = GameObject.Find("OptionButtons").transform.position + new Vector3(0, _index * -100, 0);

        int randomAction = Random.Range(0, gameState.actionsLearnt.Count);

        //... For each action learnt
        for (int j = 0; j < gameState.actionsLearnt.Count; j++) 
        {
            while (randomAction == gameState.actionsLearnt[j].Index) // If random action is equal to an action learnt index, reroll
            {
                randomAction = Random.Range(0, gameState.actionList.Count);
                Debug.Log("Random action equal to already learnt action " + gameState.actionsLearnt[j].Name + "... rerolling");
            }
        }

        button.onClick.AddListener(delegate { gameState.UnlockNewAbility(gameState.actionList[randomAction].Index); });
        button.GetComponentInChildren<TMP_Text>().text = gameState.actionList[randomAction].Name;

        // Add button to a list of buttons so they can be removed and replaced later
        optionButtons.Add(button);
    }
}
