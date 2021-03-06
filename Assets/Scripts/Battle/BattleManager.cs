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
        private int bonesRewarded;

        // References
        [SerializeField] private Enemy[] enemyPrefabs;
        [SerializeField] private Button actionButtonPrefab;
        [SerializeField] private Button bonesRewardButton;
        [SerializeField] private TMP_Text playerEnergyTextValue;
        [SerializeField] private GameObject rewardsMenu;
        [SerializeField] private GameObject atkOptions;
        [SerializeField] private Player player;
        private GameState gameState;

        // Audio clips
        AudioClip battleWonClip;

        // Start is called before the first frame update
        void Start()
        {
            // Disable map from being usable, only look at-able
            // This will be enabled true again when the battle ends
            MapManager.Instance.mapUsable = false;
            MapManager.Instance.mapClosable = true;
            MapManager.Instance.FigureOutActiveTile();

            // Find references
            battleWonClip = Resources.Load<AudioClip>("Sounds/BattleWon");
            gameState = GameObject.Find("GameState").GetComponent<GameState>();

            // Initialise lists
            bufferList = new List<Action>();
            minibufferList = new List<MiniAction>();

            // Set variables
            waitForBuffer = false;
            waitForMiniBuffer = false;
            rewardsMenu.SetActive(false);
            playerDefenceBonus = 1;
            actionsUsed = 0;

            if(MapManager.Instance.GetActiveTile().GetTileType() == Tile.TileType.Normal)
            {
                bonesRewarded = Random.Range(30, 60);
            }
            else if(MapManager.Instance.GetActiveTile().GetTileType() == Tile.TileType.Boss)
            {
                bonesRewarded = Random.Range(100, 130);
            }

            // Make a bunch of enemies
            int amountOfEnemiesToMake = 1;
            for (int i = 0; i < amountOfEnemiesToMake; i++)
            {
                if (MapManager.Instance.GetActiveTile().GetTileType() == Tile.TileType.Boss)
                {
                    Enemy newEnemy = Instantiate(enemyPrefabs[1]);
                    enemyObjects.Add(newEnemy);
                }
                else if(MapManager.Instance.GetActiveTile().GetTileType() == Tile.TileType.Normal)
                {
                    Enemy newEnemy = Instantiate(enemyPrefabs[0]);
                    enemyObjects.Add(newEnemy);
                }
                else
                {
                    Debug.LogError("Could not find the correct enemy to spawn");
                }
                
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

            CheckIfUseableEnergy();
        }

        /// <summary>
        /// Creates battle buttons. It's important to note this void function must be separate from the intial for loop. Not sure why this is.
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
            button.name = $"Button-{gameState.actionsLearnt[randomAction].Name}";
            button.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = gameState.actionsLearnt[randomAction].EnergyCost.ToString();
            if(gameState.actionsLearnt[randomAction].Rarity == Rarity.Common)
            {
                button.image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else if (gameState.actionsLearnt[randomAction].Rarity == Rarity.Uncommon)
            {
                button.image.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
            }
            else if (gameState.actionsLearnt[randomAction].Rarity == Rarity.Rare)
            {
                button.image.color = new Color(0.0f, 0.2f, 1.0f, 1.0f);
            }
            else if (gameState.actionsLearnt[randomAction].Rarity == Rarity.SuperRare)
            {
                button.image.color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
            }
            else if (gameState.actionsLearnt[randomAction].Rarity == Rarity.Legendary)
            {
                button.image.color = new Color(1.0f, 0.5f, 0.0f, 1.0f);
            }


            // Add button to a list of buttons so they can be removed and replaced later
            atkButtons.Add(button);
        }

        /// <summary>
        /// Checks the energy number given to the button
        /// if this number is less than player energy, colour red, else green
        /// </summary>
        private void CheckIfUseableEnergy()
        {
            foreach (Button atkButton in atkButtons)
            {
                if (System.Convert.ToInt32(atkButton.gameObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text)  <= player.playerCurrentEnergy)
                {
                    atkButton.gameObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().color = Color.green;
                }
                else
                {
                    atkButton.gameObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().color = Color.red;
                }
            }
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
                    if (player.playerCurrentEnergy >= gameState.actionList[(int)ActionEnum.Bite].EnergyCost)
                    {           
                        if(enemyObjects.Count > 0)
                        {
                            //Debug.Log("Biting");
                            player.UpdateEnergy(-gameState.actionList[(int)ActionEnum.Bite].EnergyCost , 0);
                            player.Attack();
                            CalculatePlayerDamage(Random.Range(3, 8));
                        }
                        actionsUsed++;
                    }
                    else { Debug.Log("Player energy is less than 0"); }
                    break;

                case (int)ActionEnum.Scratch:
                    if (player.playerCurrentEnergy >= gameState.actionList[(int)ActionEnum.Scratch].EnergyCost)
                    {
                        if (enemyObjects.Count > 0)
                        {
                            //Debug.Log("Scratching");
                            player.UpdateEnergy(-gameState.actionList[(int)ActionEnum.Scratch].EnergyCost, 0);
                            player.Attack();
                            CalculatePlayerDamage(Random.Range(4, 16));
                        }
                        actionsUsed++;
                    } 
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Defend:
                    if (player.playerCurrentEnergy >= gameState.actionList[(int)ActionEnum.Defend].EnergyCost)
                    {
                        //Debug.Log("Defending");
                        player.Shield();
                        player.UpdateShield(Random.Range(2 * playerDefenceBonus, 8 * playerDefenceBonus));
                        player.UpdateEnergy(-gameState.actionList[(int)ActionEnum.Defend].EnergyCost, 0);
                        actionsUsed++;
                    } 
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Hide:
                    if(player.playerCurrentEnergy >= gameState.actionList[(int)ActionEnum.Hide].EnergyCost)
                    {
                        //Debug.Log("Hiding");
                        player.playerState[0] = Player.State.Hidden;
                        if(player.playerState[0] == Player.State.Hidden)
                        {
                            player.playerStateIcon.SetActive(true);
                            player.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                        }
                        player.UpdateEnergy(-gameState.actionList[(int)ActionEnum.Hide].EnergyCost, 0);
                        actionsUsed++;
                        //Debug.Log("Player state is: " + player.playerState[0]);
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Howl:
                    //Debug.Log("Howling");
                    if (player.playerCurrentEnergy >= gameState.actionList[(int)ActionEnum.Howl].EnergyCost)
                    {
                        enemyObjects[0].UpdateAttack(-Random.Range(1, 8));
                        player.UpdateEnergy(-gameState.actionList[(int)ActionEnum.Howl].EnergyCost, 0);
                        SfxPlayer.Instance.PlaySound(Resources.Load<AudioClip>("Sounds/Howl"), 1.0f);
                        actionsUsed++;
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Sprint:
                    //Debug.Log("Sprinting");
                    if (player.playerCurrentEnergy >= gameState.actionList[(int)ActionEnum.Sprint].EnergyCost)
                    {
                        playerDefenceBonus = 2;
                        player.UpdateEnergy(-gameState.actionList[(int)ActionEnum.Sprint].EnergyCost, 0);
                        actionsUsed++;
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Prepare:
                    //Debug.Log("Preparing");
                    if (player.playerCurrentEnergy >= gameState.actionList[(int)ActionEnum.Prepare].EnergyCost)
                    {
                        player.playerState[1] = Player.State.Energized;
                        if(player.playerState[1] == Player.State.Energized)
                        {
                            // Set icon thingy
                            player.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
                            actionsUsed++;
                        }
                        player.UpdateEnergy(-gameState.actionList[(int)ActionEnum.Prepare].EnergyCost, 0);
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Finisher:
                    //Debug.Log("Finisher");
                    if (player.playerCurrentEnergy >= gameState.actionList[(int)ActionEnum.Finisher].EnergyCost)
                    {
                        player.UpdateEnergy(-gameState.actionList[(int)ActionEnum.Finisher].EnergyCost, 0);
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
                    if (player.playerCurrentEnergy >= gameState.actionList[(int)ActionEnum.GambleHit].EnergyCost)
                    {
                        player.UpdateEnergy(-gameState.actionList[(int)ActionEnum.GambleHit].EnergyCost, 0);
                        player.Attack();
                        CalculatePlayerDamage(Random.Range(0,60));
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Fortitude:
                    //Debug.Log("GambleHit");
                    if (player.playerCurrentEnergy >= gameState.actionList[(int)ActionEnum.Fortitude].EnergyCost)
                    {
                        player.UpdateEnergy(-gameState.actionList[(int)ActionEnum.Fortitude].EnergyCost, 0);
                        player.Shield();
                        player.UpdateHealth(2, 0);
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Execute:
                    //Debug.Log("GambleHit");
                    if (player.playerCurrentEnergy >= gameState.actionList[(int)ActionEnum.Execute].EnergyCost)
                    {
                        player.UpdateEnergy(-gameState.actionList[(int)ActionEnum.Execute].EnergyCost, 0);
                        player.Attack();

                        int zero = 2; // base damage
                        int one = enemyObjects[0].GetMaxHealth() - enemyObjects[0].GetCurrentHealth(); // 15
                        int two = Mathf.RoundToInt(one / 3); // 33% of total damage done
                        int three = zero + two;
                        int dmgtoDeal = three;

                        CalculatePlayerDamage(dmgtoDeal);
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                default:
                    Debug.LogError("Failed to use proper action when using energy");
                    break;
            }

            CheckIfUseableEnergy();
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
                        CalculatePlayerDamage(5);
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
                SfxPlayer.Instance.PlaySound(Resources.Load<AudioClip>("Sounds/Clang"), 1.0f);
                enemyObjects[0].UpdateShield(-_atkDmg);
            }
        }

        /// <summary>
        /// Checks if battle has ended - usually due to <see cref="enemyObjects"/> list being less than 1
        /// </summary>
        public void BattleEndCheck()
        {
            Debug.Log("Checking battle end");

            // If enemy objects less than 1, play win sound and open map for player
            if(enemyObjects.Count < 1)
            {
                // Play win sound
                SfxPlayer.Instance.PlaySound(battleWonClip, 1.0f);

                // Turn usable buttons off
                atkOptions.SetActive(false);
                GameObject.Find("EndTurnButton").SetActive(false);

                StartCoroutine(BattleEndWait());
            }
        }

        IEnumerator BattleEndWait()
        {
            yield return new WaitForSeconds(2.0f);

            // Open victory rewards
            rewardsMenu.SetActive(true);
            bonesRewardButton.GetComponentInChildren<TMP_Text>().text = bonesRewarded.ToString();

            // Send player stats to gamestate for safekeeping through levels:
            gameState.playerCurrentHP = player.GetPlayerCurrentHP();
            gameState.playerMaxHP = player.GetPlayerMaxHP();

            MapManager.Instance.CalculateSelectableTiles();
        }

        /// <summary>
        /// Adds bones reward amount
        /// </summary>
        public void AddBonesReward()
        {
            SfxPlayer.Instance.PlaySound(Resources.Load<AudioClip>("Sounds/BoneCollect"), 1.0f);
            GameState.Instance.AddBones(bonesRewarded);

            Destroy(bonesRewardButton.gameObject);
        }

        /// <summary>
        /// Opens the map and does not allow player to close 
        /// Best for the next button to continue onwards
        /// </summary>
        public void ContinueMap()
        {
            MapManager.Instance.mapUsable = true;
            GameObject.Find("MapManager").GetComponent<MapManager>().ToggleMap(false);
        }
    }
}

