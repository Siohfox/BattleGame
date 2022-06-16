using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

namespace BG.Core
{
    public enum ActionEnum { Bite, Scratch, Defend, Hide, Howl, Sprint, Prepare, Finisher };
    public enum MiniActionEnum { FinisherMini };

    public class GameState : MonoBehaviour
    {
        // List arrays of actions
        public List<string> enumActionList = System.Enum.GetNames(typeof(ActionEnum)).ToList(); // String names of actions - maybe not needed
        public List<Action> actionsLearnt; // Actions known from entire list
        public List<Action> actionList; // Actions entire list
        public List<MiniAction> miniActionList; // Mini actions entire list

        // UI
        private TMP_Text goldTextValue;
        private TMP_Text levelTextValue;

        // Game Variables
        private int goldAmount;
        private int levelAmount;

        // AudioClips
        private AudioClip actionLearnClip;

        private void Awake()
        {
            // Make sure there aren't duplicate gamestates
            GameObject[] obj = GameObject.FindGameObjectsWithTag("GameState");
            if (obj.Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(this);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Initialise values
            goldAmount = 100;
            levelAmount = 1;
            actionLearnClip = Resources.Load<AudioClip>("Sounds/ActionLearn");

            UpdateGameStateVariableHolds();

            actionList = new List<Action>
            {
                new Action() { Name = "Bite", Index = 0, SpeedTime = 0.2f},
                new Action() { Name = "Scratch", Index = 1, SpeedTime = 0.2f },
                new Action() { Name = "Defend", Index = 2, SpeedTime = 0.3f },
                new Action() { Name = "Hide", Index = 3, SpeedTime = 0.1f },
                new Action() { Name = "Howl", Index = 4, SpeedTime = 0.3f },
                new Action() { Name = "Sprint", Index = 5, SpeedTime = 0.4f },
                new Action() { Name = "Prepare", Index = 6, SpeedTime = 0.1f },
                new Action() { Name = "Finisher", Index = 7, SpeedTime = 2.0f }
            };

            // initialise Actions learnt
            actionsLearnt = new List<Action>
            { 
                actionList[0],
                actionList[1],
                actionList[2],
                actionList[3]
            };


            miniActionList = new List<MiniAction>
            {
                new MiniAction() {Name = "FinisherMini", Index = 0, SpeedTime = 0.1f}
            };
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
                //Debug.Log("Unlocking new ability: " + actionList[_actionIndex].Name);
                actionsLearnt.Add(actionList[_actionIndex]);
                SfxPlayer.Instance.PlaySound(actionLearnClip, 1.0f);
            }
        }

        /// <summary>
        /// Updates the GameState's pointer to text components and updates them
        /// </summary>
        private void UpdateGameStateVariableHolds()
        {
            goldTextValue = GameObject.Find("GoldText").GetComponent<TMP_Text>();
            levelTextValue = GameObject.Find("LevelText").GetComponent<TMP_Text>();

            goldTextValue.text = "Gold: " + goldAmount.ToString();
            levelTextValue.text = "Level: " + levelAmount.ToString();
        }
    }

    public class Action
    {
        public string Name { get; set; } // The name of the action
        public int Index { get; set; } // The index of the action
        public float SpeedTime { get; set; } // The time it takes to finish animation
    }

    public class MiniAction
    {
        public string Name { get; set; } // The name of the action
        public int Index { get; set; } // The index of the action
        public float SpeedTime { get; set; } // The time it takes to finish animation
    }
}
