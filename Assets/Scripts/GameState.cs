using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

namespace BG.Core
{
    public enum Action { Bite, Scratch, Defend, Hide, Howl, Sprint, Prepare };

    public class GameState : MonoBehaviour
    {
        public List<string> enumActionList = System.Enum.GetNames(typeof(Action)).ToList();
        public List<ActionThing> actionsLearnt;
        public List<ActionThing> actionList;

        private TMP_Text goldTextValue;
        private TMP_Text levelTextValue;

        private int goldAmount;
        private int levelAmount;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            goldAmount = 10;
            levelAmount = 0;

            UpdateGameStateVariableHolds();

            actionsLearnt = new List<ActionThing> { };

            actionList = new List<ActionThing>
            {
                new ActionThing() { Name = "Bite", Index = 0 },
                new ActionThing() { Name = "Scratch", Index = 1 },
                new ActionThing() { Name = "Defend", Index = 2 },
                new ActionThing() { Name = "Hide", Index = 3 },
                new ActionThing() { Name = "Howl", Index = 4 },
                new ActionThing() { Name = "Sprint", Index = 5 },
                new ActionThing() { Name = "Prepare", Index = 6 }
            };


            actionsLearnt.Add(actionList[0]);
            actionsLearnt.Add(actionList[1]);
            actionsLearnt.Add(actionList[2]);
            actionsLearnt.Add(actionList[4]);

            //for (int i = 0; i < actionsLearnt.Count; i++)
            //{
            //    Debug.Log("Action " + i + " learnt = " + actionsLearnt[i].Name);
            //}
        }

        public void UnlockNewAbility(int _actionIndex)
        {
            bool abilityAlreadyLearnt = false;
            // Catch to see if ability is already learnt
            foreach (var action in actionsLearnt)
            {
                if (_actionIndex == action.Index)
                {
                    abilityAlreadyLearnt = true;
                    Debug.LogWarning("Action " + actionList[_actionIndex].Name + " already learnt. Is this an error?");
                }
            }
            
            if (!abilityAlreadyLearnt)
            {
                Debug.Log("Unlocking new ability: " + actionList[_actionIndex].Name);
                actionsLearnt.Add(actionList[_actionIndex]);
            }
        }

        public void UpdateGameStateVariableHolds()
        {
            goldTextValue = GameObject.Find("GoldText").GetComponent<TMP_Text>();
            levelTextValue = GameObject.Find("LevelText").GetComponent<TMP_Text>();

            goldTextValue.text = "Gold: " + goldAmount.ToString();
            levelTextValue.text = "Level: " + levelAmount.ToString();
        }
    }

    public class ActionThing
    {
        public string Name { get; set; }
        public int Index { get; set; }
    }
}
