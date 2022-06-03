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
        

        [SerializeField] public List<GameObject> enemyObjects;

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

            CreateBattleButtons();
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(actionList.Count());
        }


        // >>> PERHAPS ISSUE IS SOMETHING TO DO WITH THE BUTTON PREFAB INSTANTIATION!! <<<<
        public void CreateBattleButtons()
        {
            

            // Create buttons
            for (int i = 0; i < 3; i++)
            {
                // Select a random action from entire action enum
                Action randomAction = (Action)Random.Range(0, System.Enum.GetValues(typeof(Action)).Length);

                // Init last action to be an unreachable number so it doesn't just skip the first select
                Action lastAction = (Action)1000;

                // Instantiate button under parent transform buttonpos + space them out
                Button button = Instantiate(actionButtonPrefab, GameObject.Find("ButtonPos").transform.position + new Vector3(i * 250,0,0), Quaternion.identity, GameObject.Find("ButtonPos").transform);

                // While the action is the same as last, reroll until a new one is found ---- NOTE: MAKE THIS A LIST TO REMOVE FROM LATER
                while (randomAction == lastAction)
                {
                    //Debug.Log("Random action is: " + randomAction.ToString() + "Rerolling...");
                    randomAction = (Action)Random.Range(0, System.Enum.GetValues(typeof(Action)).Length);
                }

                // Last action becomes random action
                lastAction = randomAction;

                // Update button to listen to it's new action + update button text to action name
                button.onClick.AddListener(delegate { UseAction(randomAction); });
                button.GetComponentInChildren<TMP_Text>().text = randomAction.ToString();
            }

        }

        public void UseAction(Action action)
        {
            Debug.Log("Action passed in: " + action.ToString());
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
                            enemyObjects[0].GetComponent<Enemy>().UpdateHealth(-5, 0);
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
                            enemyObjects[0].GetComponent<Enemy>().UpdateHealth(-10, 0);
                        }     
                    } 
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case Action.Defend:
                    if (player.playerCurrentEnergy > 0)
                    {
                        Debug.Log("Defending");
                        player.Shield();
                        player.UpdateEnergy(-1, 0);
                    } 
                    else { Debug.Log("Player energy is less than 0"); }

                    break;

                case Action.Howl:
                    // lower enemy attack
                    if(player.playerCurrentEnergy > 1)
                    {
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

