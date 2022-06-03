using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

namespace BG.Battle 
{
    public enum Action { Bite, Scratch, Defend, Hide, Howl };

    public class BattleManager : MonoBehaviour
    {
        

        public List<GameObject> enemyObjects;

        public List<Button> atkButtons;

        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private Button actionButtonPrefab;

        [SerializeField] private TMP_Text playerEnergyTextValue;

        [SerializeField] private Player player;

        public List<string> actionList = System.Enum.GetNames(typeof(Action)).ToList();

        // Start is called before the first frame update
        void Start()
        {
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

            // Create a list to store actions that have been used
            List<int> usedActions = new List<int>();

            // Create buttons
            for (int i = 0; i < 3; i++)
            {
                // Select a random action from entire Action enum
                Action randomAction = (Action)Random.Range(0, System.Enum.GetValues(typeof(Action)).Length);

                // Instantiate button under parent transform buttonpos + space them out from left to right
                Button button = Instantiate(actionButtonPrefab, GameObject.Find("ButtonPos").transform.position + new Vector3(i * 550,0,0), Quaternion.identity, GameObject.Find("ButtonPos").transform);

                // For each used action, if random action is a dupe, reroll
                for (int j = 0; j < usedActions.Count; j++)
                {
                    if ((int)randomAction == usedActions[j])
                    {
                        randomAction = (Action)Random.Range(0, System.Enum.GetValues(typeof(Action)).Length);
                    }
                }

                // After the loop, add the action used to the used list so it can't reuse it
                usedActions.Add((int)randomAction);

                // Update button to listen to it's new action + update button text to action name
                button.onClick.AddListener(delegate { UseAction(randomAction); });
                button.GetComponentInChildren<TMP_Text>().text = randomAction.ToString();

                // Add button to a list of buttons so they can be removed and replaced later
                atkButtons.Add(button);
            }
        }

        public void UseAction(Action action)
        {
            switch (action)
            {
                case Action.Bite:
                    if (player.playerCurrentEnergy > 0)
                    {           
                        if(enemyObjects.Count > 0)
                        {
                            Debug.Log("Biting");
                            player.UpdateEnergy(-1, 0);
                            player.Attack();
                            enemyObjects[0].GetComponent<Enemy>().UpdateHealth(-(Random.Range(1,7)), 0);
                        }
                    }
                    else { Debug.Log("Player energy is less than 0"); }
                    break;

                case Action.Scratch:
                    if (player.playerCurrentEnergy > 1)
                    {
                        
                        if (enemyObjects.Count > 0)
                        {
                            Debug.Log("Scratching");
                            player.UpdateEnergy(-2, 0);
                            player.Attack();
                            enemyObjects[0].GetComponent<Enemy>().UpdateHealth(-(Random.Range(4, 16)), 0);
                        }     
                    } 
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case Action.Defend:
                    if (player.playerCurrentEnergy > 0)
                    {
                        Debug.Log("Defending");
                        player.Shield();
                        player.UpdateShield(Random.Range(2,8));
                        player.UpdateEnergy(-1, 0);
                    } 
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case Action.Hide:
                    // lower enemy attack
                    if(player.playerCurrentEnergy > 3)
                    {
                        Debug.Log("Hiding");
                        player.playerState = Player.State.Hidden;
                        if(player.playerState == Player.State.Hidden)
                        {
                            player.playerStateIcon.SetActive(true);
                            player.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                        }
                        player.UpdateEnergy(-4, 0);

                        Debug.Log("Player state is: " + player.playerState);
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case Action.Howl:
                    // lower enemy attack
                    if (player.playerCurrentEnergy > 0)
                    {
                        enemyObjects[0].GetComponent<Enemy>().UpdateAttack(-Random.Range(1, 8));  
                        player.UpdateEnergy(-1, 0);
                    }
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                default:
                    Debug.LogError("Failed to use proper action when using energy");
                    break;
            }
        }
    }
}

