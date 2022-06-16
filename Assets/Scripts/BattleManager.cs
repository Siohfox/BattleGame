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
        public List<GameObject> enemyObjects;
        public List<Button> atkButtons;
        private List<Action> bufferList;
        private List<MiniAction> minibufferList;
        private bool waitForBuffer;
        private bool waitForMiniBuffer;
        public int actionsUsed;

        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private Button actionButtonPrefab;
        [SerializeField] private TMP_Text playerEnergyTextValue;

        [SerializeField] private Player player;
        private GameState gameState;

        public int playerDefenceBonus;

        AudioClip battleWonClip;

        // Start is called before the first frame update
        void Start()
        {
            battleWonClip = Resources.Load<AudioClip>("Sounds/BattleWon");

            gameState = GameObject.Find("GameState").GetComponent<GameState>();

            bufferList = new List<Action>();
            minibufferList = new List<MiniAction>();

            waitForBuffer = false;
            waitForMiniBuffer = false;
            playerDefenceBonus = 1;
            actionsUsed = 0;

            // Make a bunch of enemies
            int amountOfEnemiesToMake = 1;
            for (int i = 0; i < amountOfEnemiesToMake; i++)
            {
                GameObject newEnemy = Instantiate(enemyPrefab.gameObject);
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

            int randomAction = Random.Range(0, gameState.actionsLearnt.Count);

            //Debug.Log("actions learnt count: " + gameState.actionsLearnt.Count);

            for (int j = 0; j < _usedActions.Count; j++)
            {
                while (randomAction == _usedActions[j]) //if random action is equal to a used action, reroll
                {
                    randomAction = Random.Range(0, gameState.actionsLearnt.Count);
                    //Debug.Log("Random action equal to used action " + gameState.actionsLearnt[_usedActions[j]].Name + "... rerolling");
                }
            }

            _usedActions.Add(randomAction);

            // Update button to listen to it's new action + update button text to action name
            button.onClick.AddListener(delegate { BufferAction(gameState.actionsLearnt[randomAction]); });
            button.GetComponentInChildren<TMP_Text>().text = gameState.actionsLearnt[randomAction].Name;

            // Add button to a list of buttons so they can be removed and replaced later
            atkButtons.Add(button);
        }

        private void Update()
        {
            if (bufferList.Count > 0 && !waitForBuffer)
            {
                StartCoroutine(ExecuteAction());
            }

            if(minibufferList.Count > 0 && !waitForMiniBuffer)
            {
                StartCoroutine(ExecuteMiniAction());
            }
        }

        private void BufferAction(Action _action)
        {
            if (enemyObjects.Count > 0)
            {
                bufferList.Add(_action);
            }
        }

        IEnumerator ExecuteAction()
        {
            waitForBuffer = true;
            UseAction(bufferList[0].Index);
            yield return new WaitForSeconds(bufferList[0].SpeedTime);        
            bufferList.RemoveAt(0);
            waitForBuffer = false;
        }

        IEnumerator ExecuteMiniAction()
        {
            waitForMiniBuffer = true;
            UseMiniAction(minibufferList[0].Index);
            yield return new WaitForSeconds(minibufferList[0].SpeedTime);
            minibufferList.RemoveAt(0);
            waitForMiniBuffer = false;
        }

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
                            enemyObjects[0].GetComponent<Enemy>().UpdateHealth(-(Random.Range(1,7)), 0);
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
                            enemyObjects[0].GetComponent<Enemy>().UpdateHealth(-(Random.Range(4, 16)), 0);
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
                    // lower enemy attack
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
                    // lower enemy attack
                    //Debug.Log("Howling");
                    if (player.playerCurrentEnergy > 0)
                    {
                        enemyObjects[0].GetComponent<Enemy>().UpdateAttack(-Random.Range(1, 8));  
                        player.UpdateEnergy(-1, 0);
                        actionsUsed++;
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case (int)ActionEnum.Sprint:
                    // lower enemy attack
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
                    // lower enemy attack
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

                default:
                    Debug.LogError("Failed to use proper action when using energy");
                    break;
            }
        }

        public void UseMiniAction(int _miniAction)
        {
            switch (_miniAction)
            {
                case (int)MiniActionEnum.FinisherMini:
                    //Debug.Log("Finisher Mini");
                    if(enemyObjects.Count > 0)
                    {
                        player.Attack();
                        enemyObjects[0].GetComponent<Enemy>().UpdateHealth(-6, 0);
                    }

                    break;
            }
        }

        public void BattleEndCheck()
        {
            Debug.Log("Checking for battle end.. enemy count: " + enemyObjects.Count);
            if(enemyObjects.Count < 1)
            {
                SfxPlayer.Instance.PlaySound(battleWonClip, 1.0f);
                GameObject.Find("MapManager").GetComponent<MapManager>().ToggleMap(false);
            }
        }
    }
}

