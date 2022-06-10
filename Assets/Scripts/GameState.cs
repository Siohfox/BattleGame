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
        public List<string> actionList = System.Enum.GetNames(typeof(Action)).ToList();
        public List<ActionThing> actionsLearnt;
        public List<ActionThing> actionThingList;

        [SerializeField] private TMP_Text goldTextValue;
        [SerializeField] private TMP_Text levelTextValue;

        private int goldAmount;
        private int levelAmount;

        // Start is called before the first frame update
        void Start()
        {
            actionsLearnt = new List<ActionThing> { };

            actionThingList = new List<ActionThing>
            {
                new ActionThing() { Name = "actionBite", Index = 0 },
                new ActionThing() { Name = "actionScratch", Index = 1 },
                new ActionThing() { Name = "actionDefend", Index = 2 },
                new ActionThing() { Name = "actionHide", Index = 3 },
                new ActionThing() { Name = "actionHowl", Index = 4 },
                new ActionThing() { Name = "actionSprint", Index = 5 },
                new ActionThing() { Name = "actionPrepare", Index = 6 }
            };


            actionsLearnt.Add(actionThingList[0]);
            actionsLearnt.Add(actionThingList[1]);
            actionsLearnt.Add(actionThingList[5]);
            actionsLearnt.Add(actionThingList[2]);

            for (int i = 0; i < actionsLearnt.Count; i++)
            {
                Debug.Log("Action " + i + " learnt = " + actionsLearnt[i].Name);
            }

            goldAmount = 0;
            levelAmount = 0;

            goldTextValue.text = "Gold: " + goldAmount.ToString();
            levelTextValue.text = "Level: " + levelAmount.ToString();
        }

        public void DebugActionsLearnt()
        {
            // for each action that exists
            for (int i = 0; i < System.Enum.GetValues(typeof(Action)).Length; i++)
            {
                // compare it against which actions have been learnt
                for (int j = 0; j < actionsLearnt.Count; j++)
                {
                    if (i == actionsLearnt[j].Index)
                    {
                        Debug.Log("Action " + i + " learnt = " + actionList[i].ToString());
                    }
                }
            }
        }
    }

    public class ActionThing
    {
        public string Name { get; set; }
        public int Index { get; set; }
    }
}
