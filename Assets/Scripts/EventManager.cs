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

    public void CreateOptionButtons()
    {
        // Before creating any new buttons, remove any null buttons (e.g buttons that have been destroyed)
        optionButtons.RemoveAll(s => s == null);

        // Create a list to store actions that have been used
        List<int> usedActions = new List<int>();

        // Create buttons
        int amountOfButtons = 5;
        for (int i = 0; i < amountOfButtons; i++)
        {
            Action randomAction = (Action)Random.Range(0, System.Enum.GetValues(typeof(Action)).Length);

            // Instantiate button under parent transform buttonpos + space them out from left to right
            Button button = Instantiate(objectButtonPrefab, GameObject.Find("ButtonPos").transform.position + new Vector3(i * 550, 0, 0), Quaternion.identity);
            button.transform.SetParent(GameObject.Find("OptionButtons").transform);
            button.transform.position = GameObject.Find("OptionButtons").transform.position + new Vector3(0, i * -30, 0);

            // Update button to listen to it's new action + update button text to action name
            button.onClick.AddListener(UnlockNewAbilityThing);
            button.GetComponentInChildren<TMP_Text>().text = randomAction.ToString();

            // Add button to a list of buttons so they can be removed and replaced later
            optionButtons.Add(button);
        }
    }

    public void UnlockNewAbilityThing()
    {

    }
}
