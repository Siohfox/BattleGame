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

        [SerializeField] private TMP_Text goldTextValue;
        [SerializeField] private TMP_Text levelTextValue;

        private int goldAmount;
        private int levelAmount;

        // Start is called before the first frame update
        void Start()
        {
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
            actionsLearnt.Add(actionList[5]);
            actionsLearnt.Add(actionList[2]);

            //for (int i = 0; i < actionsLearnt.Count; i++)
            //{
            //    Debug.Log("Action " + i + " learnt = " + actionsLearnt[i].Name);
            //}

            goldAmount = 0;
            levelAmount = 0;

            goldTextValue.text = "Gold: " + goldAmount.ToString();
            levelTextValue.text = "Level: " + levelAmount.ToString();
        }

        public void UnlockNewAbility(int _actionIndex)
        {
            Debug.Log("Unlocking new ability: " + actionList[_actionIndex].Name);
        }
    }

    public class ActionThing
    {
        public string Name { get; set; }
        public int Index { get; set; }
    }
}
