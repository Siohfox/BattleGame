using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BG.Turns;

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
            for (int i = 0; i < 2; i++)
            {
                GameObject newEnemy = Instantiate(enemyPrefab.gameObject);
                enemyObjects.Add(newEnemy);
            }

            enemyObjects[0].transform.position += new Vector3(-5, 0, 0);
            enemyObjects[1].transform.position += new Vector3(0, 0, 0);

            UpdateEnergy(5, 5);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void RemoveSelfLol()
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
                    // bite
                    if (playerCurrentEnergy > 0)
                    {
                        Debug.Log("Biting");
                        UpdateEnergy(-1, 0);
                        enemyObjects[0].GetComponent<Enemy>().UpdateHealth(-5, 0);
                    }
                    break;

                case Action.Scratch:
                    // scratch
                    if (playerCurrentEnergy > 0)
                    {
                        UpdateEnergy(-2, 0);
                        enemyObjects[0].GetComponent<Enemy>().UpdateHealth(-10, 0);
                        Debug.Log("Scratching");
                    }

                    break;

                case Action.Defend:
                    // defend
                    Debug.Log("Defending");
                    UpdateEnergy(-1, 0);
                    break;


                default:
                    Debug.LogError("Failed to use proper action when using energy");
                    break;
            }
        }
    }
}

