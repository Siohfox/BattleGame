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
        public List<Action> actionsLearnt;

        [SerializeField] private TMP_Text goldTextValue;
        [SerializeField] private TMP_Text levelTextValue;

        private int goldAmount;
        private int levelAmount;

        // Start is called before the first frame update
        void Start()
        {
            actionsLearnt = new List<Action> { };
            actionsLearnt.Add(Action.Bite);
            actionsLearnt.Add(Action.Howl);
            actionsLearnt.Add(Action.Scratch);
            actionsLearnt.Add(Action.Defend);
            DebugActionsLearnt();

            goldAmount = 0;
            levelAmount = 0;

            goldTextValue.text = "Gold: " + goldAmount.ToString();
            levelTextValue.text = "Level: " + levelAmount.ToString();
        }

        void DebugActionsLearnt()
        {
            // for each action that exists
            for (int i = 0; i < System.Enum.GetValues(typeof(Action)).Length; i++)
            {
                // compare it against which actions have been learnt
                for (int j = 0; j < actionsLearnt.Count; j++)
                {
                    if (i == (int)actionsLearnt[j])
                    {
                        Debug.Log("Action " + i + " learnt = " + actionList[i].ToString());
                    }
                }
            }
        }
    }
}
