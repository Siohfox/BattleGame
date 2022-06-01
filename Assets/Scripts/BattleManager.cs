using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BG.Battle 
{
    public class BattleManager : MonoBehaviour
    {
        public enum Action { Bite, Scratch, Defend, Hide, Howl };

        private int playerMaxEnergy = 0;
        private int playerCurrentEnergy = 0;

        [SerializeField] private GameObject playerObject;
        [SerializeField] public List<GameObject> enemyObjects;

        [SerializeField] private Enemy enemyPrefab;

        [SerializeField] private TMP_Text playerEnergyTextValue;

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
            UpdateEnergy(5, 5);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateEnergy(int currentEnergy, int maxEnergy)
        {
            playerCurrentEnergy += currentEnergy;
            playerMaxEnergy += maxEnergy;

            playerEnergyTextValue.text = playerCurrentEnergy.ToString() + "/" + playerMaxEnergy.ToString();
        }

        public void UseEnergy(int action)
        {
            switch ((Action)action)
            {
                case Action.Bite:
                    if (playerCurrentEnergy > 0)
                    {           
                        if(enemyObjects.Count > 0)
                        {
                            Debug.Log("Biting");
                            UpdateEnergy(-1, 0);
                            enemyObjects[0].GetComponent<Enemy>().UpdateHealth(-5, 0);
                        }
                    }
                    break;

                case Action.Scratch:
                    if (playerCurrentEnergy > 1)
                    {
                        
                        if (enemyObjects.Count > 0)
                        {
                            Debug.Log("Scratching");
                            UpdateEnergy(-2, 0);
                            enemyObjects[0].GetComponent<Enemy>().UpdateHealth(-10, 0);
                        }     
                    }

                    break;

                case Action.Defend:
                    if (playerCurrentEnergy > 1)
                    {
                        Debug.Log("Defending");
                        UpdateEnergy(-1, 0);
                    }

                    break;

                case Action.Howl:
                    // lower enemy attack
                    if(playerCurrentEnergy > 1)
                    {

                    }

                    break;

                default:
                    Debug.LogError("Failed to use proper action when using energy");
                    break;
            }
        }
    }
}

