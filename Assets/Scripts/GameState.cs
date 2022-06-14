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
        // List arrays of actions
        public List<string> enumActionList = System.Enum.GetNames(typeof(Action)).ToList();
        public List<ActionThing> actionsLearnt;
        public List<ActionThing> actionList;

        // UI
        private TMP_Text goldTextValue;
        private TMP_Text levelTextValue;

        // Game Variables
        private int goldAmount;
        private int levelAmount;

        // AudioClips
        private AudioClip actionLearnClip;

        // Script Refs
        MapManager mapManager;

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
            mapManager = FindObjectOfType<MapManager>();

            Debug.Log("Tile state 0,0 = " + mapManager.GetTileAtPosition(new Vector2(0, 0)).GetComponent<Tile>().GetTileState().ToString());

            // Initialise values
            goldAmount = 100;
            levelAmount = 1;
            actionLearnClip = Resources.Load<AudioClip>("Sounds/ActionLearn");

            UpdateGameStateVariableHolds();

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

            // initialise Actions learnt
            actionsLearnt = new List<ActionThing>
            { 
                actionList[0],
                actionList[1],
                actionList[2],
                actionList[3]
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
                MusicPlayer.Instance.PlaySound(actionLearnClip, 10);
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

    public class ActionThing
    {
        public string Name { get; set; }
        public int Index { get; set; }
    }
}
