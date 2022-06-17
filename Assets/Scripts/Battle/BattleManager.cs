using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using BG.Core;

namespace BG.Battle 
{
    public class BattleManager : MonoBehaviour
    {
        // Object lists
        public List<Enemy> enemyObjects;
        public List<Button> atkButtons;
        private List<Action> bufferList;
        private List<MiniAction> minibufferList;

        // Variables
        private bool waitForBuffer;
        private bool waitForMiniBuffer;
        public int actionsUsed;
        public int playerDefenceBonus;

        // References
        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private Button actionButtonPrefab;
        [SerializeField] private TMP_Text playerEnergyTextValue;
        [SerializeField] private Player player;
        private GameState gameState;

        // Audio clips
        AudioClip battleWonClip;

        // Start is called before the first frame update
        void Start()
        {
            // Find references
            battleWonClip = Resources.Load<AudioClip>("Sounds/BattleWon");
            gameState = GameObject.Find("GameState").GetComponent<GameState>();

            // Initialise lists
            bufferList = new List<Action>();
            minibufferList = new List<MiniAction>();

            // Set variables
            waitForBuffer = false;
            waitForMiniBuffer = false;
            playerDefenceBonus = 1;
            actionsUsed = 0;

            // Disable map from being usable, only look at-able
            // This will be enabled true again when the battle ends
            MapManager.Instance.mapUsable = false;

            // Make a bunch of enemies
            int amountOfEnemiesToMake = 1;
            for (int i = 0; i < amountOfEnemiesToMake; i++)
            {
                Enemy newEnemy = Instantiate(enemyPrefab);
                enemyObjects.Add(newEnemy);
            }

            // try to space them out equally i guess (doesn't really work)
            for (int i = 0; i < enemyObjects.Count; i++)
            {
                int enemyPixelWidth = 1;
                int totalAreaOccupied = enemyPixelWidth * enemyObjects.Count;
                int spaceAvailable = 16 / 2;
                int totalAreaFromWidth = spaceAvailable - totalAreaOccupied;
                int spacialPos = totalAreaFromWidth / enemyObjects.Count;

                enemyObjects[i].transform.position = new Vector3(1 + spacialPos * i, 0, 0);
            }
        }

        public void CreateBattleButtons()
        {
            // Before creating any new buttons, remove any null buttons (e.g buttons that have been destroyed)
            atkButtons.RemoveAll(s => s == null);

            // Create a list to store actions that have been used - this is to avoid dupes
            List<int> usedActions = new List<int>();

            // Create buttons
            int amountOfButtons = 4;
            for (int i = 0; i < amountOfButtons; i++)
            {
                CreateButtons(i, amountOfButtons, usedActions);
            }
        }

        /// <summary>
        /// Creates battle buttons. It's important to note this void function must be seperate from the intial for loop. Not sure why this is.
        /// </summary>
        /// <param name="i"></param>
        private void CreateButtons(int _i, int _amountOfButtons, List<int> _usedActions)
        {
            // Instantiate button under parent transform buttonpos + space them out from left to right
            Button button = Instantiate(actionButtonPrefab, GameObject.Find("ButtonPos").transform.position + new Vector3(_i * 550, 0, 0), Quaternion.identity);
            button.transform.SetParent(GameObject.Find("ButtonPos").transform);
            button.transform.position = GameObject.Find("ButtonPos").transform.position + new Vector3(_i * (GameObject.Find("AtkOptionsBackground").transform.GetComponent<Image>().rectTransform.sizeDelta.x / _amountOfButtons), 0, 0) + new Vector3(_i * 30, 0);

            // Create a random action which is a selection from 0 to the max action learnt number
            int randomAction = Random.Range(0, gameState.actionsLearnt.Count);

            //Debug.Log("actions learnt count: " + gameState.actionsLearnt.Count);

            // For each used action, while the random action is equal to the used action, reroll until unused action
            for (int j = 0; j < _usedActions.Count; j++)
            {
                while (randomAction == _usedActions[j])
                {
                    randomAction = Random.Range(0, gameState.actionsLearnt.Count);
                    //Debug.Log("Random action equal to used action " + gameState.actionsLearnt[_usedActions[j]].Name + "... rerolling");
                }
            }

            // Add new action to the list of used actions so they aren't repeated
            _usedActions.Add(randomAction);

            // Update button to listen to it's new action + update button text to action name
            button.onClick.AddListener(delegate { BufferAction(gameState.actionsLearnt[randomAction]); });
            button.GetComponentInChildren<TMP_Text>().text = gameState.actionsLearnt[randomAction].Name;

            // Add button to a list of buttons so they can be removed and replaced later
            atkButtons.Add(button);
        }

        private void Update()
        {
            // If there is something in the buffer and it's not waiting for buffer to end
            // Then execute that action
            if (bufferList.Count > 0 && !waitForBuffer)
            {
                StartCoroutine(ExecuteAction());
            }

            // If there is something in the minibuffer and it's not waiting for minibuffer to end
            // Then execute that mini action
            if (minibufferList.Count > 0 && !waitForMiniBuffer)
            {
                StartCoroutine(ExecuteMiniAction());
            }
        }

        /// <summary>
        /// Buffers an action given parameter <paramref name="_action"/>
        /// </summary>
        /// <param name="_action"></param>
        private void BufferAction(Action _action)
        {
            if (enemyObjects.Count > 0)
            {
                bufferList.Add(_action);
            }
        }

        /// <summary>
        /// Executes an action and waits for action time to be over
        /// </summary>
        /// <returns></returns>
        IEnumerator ExecuteAction()
        {
            waitForBuffer = true;

            // Use first action in buffer
            UseAction(bufferList[0].Index);
            
            // If the buffered action is a finisher, then the speed of finisher is 0.2 * action used
            if (bufferList[0].Index == (int)ActionEnum.Finisher)
            {
                gameState.actionList[(int)ActionEnum.Finisher].SpeedTime = 0.2f * actionsUsed;
            }

            // Wait for animation time
            yield return new WaitForSeconds(bufferList[0].SpeedTime); 
            
            // Remove action from the buffer list
            bufferList.RemoveAt(0);

            waitForBuffer = false;
        }

        /// <summary>
        /// Executes a MiniAction and waits for action time to be over
        /// </summary>
        /// <returns></returns>
        IEnumerator ExecuteMiniAction()
        {
            waitForMiniBuffer = true;

            // Use first action in buffer
            UseMiniAction(minibufferList[0].Index);

            // Wait for animation time
            yield return new WaitForSeconds(minibufferList[0].SpeedTime);

            // Remove action from the buffer list
            minibufferList.RemoveAt(0);

            waitForMiniBuffer = false;
        }

        /// <summary>
        /// Use an action based on given param "<paramref name="action"/>" and define their behaviour
        /// </summary>
        /// <param name="action"></param>
        public void UseAction(int action)
        {
            switch (action)
            {
                case (int)ActionEnum.Bite:
                    if (player.playerCurrentEnergy > 0)
                    {           
                        if(enemyObjects.Count > 0)
                        {
                            //Debug.Log("Biting");
                            player.UpdateEnergy(-1, 0);
                            player.Attack();
                            CalculatePlayerDamage(Random.Range(1, 7));
                        }
                        actionsUsed++;
                    }
                    else { Debug.Log("Player energy is less than 0"); }
                    break;

                case (int)ActionEnum.Scratch:
                    if (player.playerCurrentEnergy > 1)
                    {
                        
                        if (enemyObjects.Count > 0)
                        {
                            //Debug.Log("Scratching");
                            player.UpdateEnergy(-2, 0);
                            player.Attack();
                            CalculatePlayerDamage(Random.Range(4, 16));
                        }
                        actionsUsed++;
                    } 
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Defend:
                    if (player.playerCurrentEnergy > 0)
                    {
                        //Debug.Log("Defending");
                        player.Shield();
                        player.UpdateShield(Random.Range(2 * playerDefenceBonus, 8 * playerDefenceBonus));
                        player.UpdateEnergy(-1, 0);
                        actionsUsed++;
                    } 
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Hide:
                    if(player.playerCurrentEnergy > 3)
                    {
                        //Debug.Log("Hiding");
                        player.playerState[0] = Player.State.Hidden;
                        if(player.playerState[0] == Player.State.Hidden)
                        {
                            player.playerStateIcon.SetActive(true);
                            player.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                        }
                        player.UpdateEnergy(-4, 0);
                        actionsUsed++;
                        //Debug.Log("Player state is: " + player.playerState[0]);
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Howl:
                    //Debug.Log("Howling");
                    if (player.playerCurrentEnergy > 0)
                    {
                        enemyObjects[0].UpdateAttack(-Random.Range(1, 8));  
                        player.UpdateEnergy(-1, 0);
                        SfxPlayer.Instance.PlaySound(Resources.Load<AudioClip>("Sounds/Howl"), 1.0f);
                        actionsUsed++;
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Sprint:
                    //Debug.Log("Sprinting");
                    if (player.playerCurrentEnergy > 0)
                    {
                        playerDefenceBonus = 2;
                        player.UpdateEnergy(-1, 0);
                        actionsUsed++;
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Prepare:
                    //Debug.Log("Preparing");
                    if (player.playerCurrentEnergy > 0)
                    {
                        player.playerState[1] = Player.State.Energized;
                        if(player.playerState[1] == Player.State.Energized)
                        {
                            // Set icon thingy
                            player.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
                            actionsUsed++;
                        }
                        player.UpdateEnergy(-1, 0);
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Finisher:
                    //Debug.Log("Finisher");
                    if (player.playerCurrentEnergy > 0)
                    {
                        player.UpdateEnergy(-1, 0);
                        for (int i = 0; i < actionsUsed; i++)
                        {
                            minibufferList.Add(gameState.miniActionList[0]);
                        }
                        actionsUsed++;
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.GambleHit:
                    //Debug.Log("GambleHit");
                    if (player.playerCurrentEnergy > 4)
                    {
                        player.UpdateEnergy(-5, 0);
                        player.Attack();
                        CalculatePlayerDamage(Random.Range(0,60));
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                default:
                    Debug.LogError("Failed to use proper action when using energy");
                    break;
            }
        }

        /// <summary>
        /// Use a MiniAction based on given param "<paramref name="_miniAction"/>" and define their behaviour
        /// </summary>
        /// <param name="_miniAction"></param>
        public void UseMiniAction(int _miniAction)
        {
            switch (_miniAction)
            {
                case (int)MiniActionEnum.FinisherMini:
                    //Debug.Log("Finisher Mini");
                    if(enemyObjects.Count > 0)
                    {
                        player.Attack();
                        CalculatePlayerDamage(-5);
                    }

                    break;
            }
        }

        /// <summary>
        /// Calculates damage to an enemy based on input dmg given
        /// </summary>
        /// <param name="_atkDmg"></param>
        private void CalculatePlayerDamage(int _atkDmg)
        {
            if (_atkDmg > enemyObjects[0].enemyCurrentDefence)
            {
                // find dif between atk dmg and players shield. (13 attack - 8 shield = 5 leftover)
                int dif = _atkDmg - enemyObjects[0].enemyCurrentDefence;

                // remove shield entirely
                enemyObjects[0].UpdateShield(-enemyObjects[0].enemyCurrentDefence);

                // apply leftover damage
                enemyObjects[0].UpdateHealth(-dif, 0);
            }
            else if (_atkDmg <= enemyObjects[0].enemyCurrentDefence)
            {
                enemyObjects[0].UpdateShield(-_atkDmg);
            }
        }

        /// <summary>
        /// Checks if battle has ended - usually due to <see cref="enemyObjects"/> list being less than 1
        /// </summary>
        public void BattleEndCheck()
        {
            // If enemy objects less than 1, play win sound and open map for player
            if(enemyObjects.Count < 1)
            {
                SfxPlayer.Instance.PlaySound(battleWonClip, 1.0f);

                // Send player stats to gamestate for safekeeping through levels:
                gameState.playerCurrentHP = player.GetPlayerCurrentHP();
                gameState.playerMaxHP = player.GetPlayerMaxHP();

                // Open map and force player to pick a new location
                MapManager.Instance.mapUsable = true;
                GameObject.Find("MapManager").GetComponent<MapManager>().ToggleMap(false);
            }
        }
    }
}

