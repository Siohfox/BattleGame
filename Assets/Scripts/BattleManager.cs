using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BG.Battle 
{
    public class BattleManager : MonoBehaviour
    {
        public enum Action { Bite, Scratch, Defend, Hide, Howl };

        [SerializeField] public List<GameObject> enemyObjects;

        [SerializeField] private Enemy enemyPrefab;

        [SerializeField] private TMP_Text playerEnergyTextValue;

        [SerializeField] private Player player;

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

        // Update is called once per frame
        void Update()
        {

        }

        public void UseEnergy(int action)
        {
            switch ((Action)action)
            {
                case Action.Bite:
                    if (player.playerCurrentEnergy > 0)
                    {           
                        if(enemyObjects.Count > 0)
                        {
                            Debug.Log("Biting");
                            player.UpdateEnergy(-1, 0);
                            enemyObjects[0].GetComponent<Enemy>().UpdateHealth(-5, 0);
                        }
                    }
                    break;

                case Action.Scratch:
                    if (player.playerCurrentEnergy > 1)
                    {
                        
                        if (enemyObjects.Count > 0)
                        {
                            Debug.Log("Scratching");
                            player.UpdateEnergy(-2, 0);
                            enemyObjects[0].GetComponent<Enemy>().UpdateHealth(-10, 0);
                        }     
                    }

                    break;

                case Action.Defend:
                    if (player.playerCurrentEnergy > 1)
                    {
                        Debug.Log("Defending");
                        player.UpdateEnergy(-1, 0);
                    }

                    break;

                case Action.Howl:
                    // lower enemy attack
                    if(player.playerCurrentEnergy > 1)
                    {
                        player.UpdateEnergy(-1, 0);
                    }

                    break;

                default:
                    Debug.LogError("Failed to use proper action when using energy");
                    break;
            }
        }
    }
}

