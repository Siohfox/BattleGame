using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

namespace BG.Core
{
    public enum ActionEnum { Bite, Scratch, Defend, Hide, Howl, Sprint, Prepare, Finisher, GambleHit };
    public enum PowersEnum { PowerOfTheMoon, PowerOfTheSun, PowerOfNature, PowerOfChaos, PowerOfOrder, PowerOfLight, PowerOfDark, PowerOfWeather, PowerOfAlpha, PowerOfLuck }
    public enum MiniActionEnum { FinisherMini };

    public class GameState : MonoBehaviour
    {
        // Singleton
        public static GameState Instance { get; private set; }

        // List arrays of actions
        public List<string> enumActionList = System.Enum.GetNames(typeof(ActionEnum)).ToList(); // String names of actions - maybe not needed
        public List<Action> actionsLearnt; // Actions known from entire list
        public List<Action> actionList; // Actions entire list
        public List<Power> powersKnown; // Powers known from entire list
        public List<Power> powerList; // Powers entire list
        public List<MiniAction> miniActionList; // Mini actions entire list

        // UI
        private TMP_Text bonesTextValue;
        private TMP_Text levelTextValue;

        // Game Variables
        [System.NonSerialized] public int bonesAmount;
        [System.NonSerialized] public int tilesExploredAmount;

        // Player Variables
        public int playerMaxHP;
        public int playerCurrentHP;

        // AudioClips
        private AudioClip actionLearnClip;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

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
            bonesAmount = 100;
            tilesExploredAmount = 1;

            playerMaxHP = 100;
            playerCurrentHP = playerMaxHP;

            actionLearnClip = Resources.Load<AudioClip>("Sounds/ActionLearn");

            bonesTextValue = GameObject.Find("BonesText").GetComponent<TMP_Text>();
            levelTextValue = GameObject.Find("LevelText").GetComponent<TMP_Text>();

            bonesTextValue.text = "Bones: " + bonesAmount.ToString();
            levelTextValue.text = "Explored: " + tilesExploredAmount.ToString();

            // Initialise list of Actions
            actionList = new List<Action>
            {
                new Action() { Name = "Bite", Index = 0, SpeedTime = 0.2f},
                new Action() { Name = "Scratch", Index = 1, SpeedTime = 0.2f },
                new Action() { Name = "Defend", Index = 2, SpeedTime = 0.3f },
                new Action() { Name = "Hide", Index = 3, SpeedTime = 0.1f },
                new Action() { Name = "Howl", Index = 4, SpeedTime = 0.3f },
                new Action() { Name = "Sprint", Index = 5, SpeedTime = 0.4f },
                new Action() { Name = "Prepare", Index = 6, SpeedTime = 0.1f },
                new Action() { Name = "Finisher", Index = 7, SpeedTime = 2.0f },
                new Action() { Name = "GambleHit", Index = 8, SpeedTime = 0.2f }
            };

            // Initialise Actions learnt
            actionsLearnt = new List<Action>
            { 
                actionList[0],
                actionList[1],
                actionList[2],
                actionList[3]
            };

            // Initialise list of Powers
            powerList = new List<Power>
            {
                new Power() { Name = "PowerOfTheMoon", Index = 0, State = false},
                new Power() { Name = "PowerOfTheSun", Index = 1, State = false},
                new Power() { Name = "PowerOfNature", Index = 2, State = false},
                new Power() { Name = "PowerOfChaos", Index = 3, State = false},
                new Power() { Name = "PowerOfOrder", Index = 4, State = false},
                new Power() { Name = "PowerOfLight", Index = 5, State = false},
                new Power() { Name = "PowerOfDark", Index = 6, State = false},
                new Power() { Name = "PowerOfWeather", Index = 7, State = false},
                new Power() { Name = "PowerOfAlpha", Index = 8, State = false},
                new Power() { Name = "PowerOfLuck", Index = 9, State = false}
            };

            // Initialise Powers learnt
            powersKnown = new List<Power>();

            // Initialise mini action list
            miniActionList = new List<MiniAction>
            {
                new MiniAction() {Name = "FinisherMini", Index = 0, SpeedTime = 0.1f}
            };
        }

        /// <summary>
        /// Unlocks new ability on given index
        /// </summary>
        /// <param name="_actionIndex"></param>
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
        /// Unlocks new power on given index
        /// </summary>
        /// <param name="_powerIndex"></param>
        public void UnlockNewPower(int _powerIndex)
        {
            bool powerAlreadyKnown = false;
            // Catch to see if power is already known
            foreach (var power in powersKnown)
            {
                if(_powerIndex == power.Index)
                {
                    powerAlreadyKnown = true;
                    Debug.LogWarning($"Power {powerList[_powerIndex].Name} already learnt. Is this an error?");
                }
            }

            if(!powerAlreadyKnown)
            {              
                powersKnown.Add(powerList[_powerIndex]);
                SfxPlayer.Instance.PlaySound(actionLearnClip, 1.0f);
            }

            foreach (var power in powersKnown)
            {
                power.State = true;
            }
        }

        /// <summary>
        /// Adds bones given on value and updates UI
        /// </summary>
        /// <param name="bonesAmount"></param>
        public void AddBones(int _bonesAmount)
        {
            bonesAmount += _bonesAmount;

            GameObject.Find("UIUpdater").GetComponent<UIUpdater>().UpdateUIElements();
        }
    }

    public class Action
    {
        public string Name { get; set; } // The name of the action
        public int Index { get; set; } // The index of the action
        public float SpeedTime { get; set; } // The time it takes to finish animation
    }

    public class Power
    {
        public string Name { get; set; } // The name of the power
        public int Index { get; set; } // The index of the power
        public bool State { get; set; } // Whether the power is enabled or not
    }

    public class MiniAction
    {
        public string Name { get; set; } // The name of the action
        public int Index { get; set; } // The index of the action
        public float SpeedTime { get; set; } // The time it takes to finish animation
    }
}
