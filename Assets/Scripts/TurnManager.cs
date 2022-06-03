using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BG.Battle;
using UnityEngine.UI;

namespace BG.Turns
{
    public class TurnManager : MonoBehaviour
    {
        enum TurnType { Player, Enemy };

        private bool roundStarted;
        private int roundNumber;
        private TurnType turn;
        private bool enemyTurnDone;
        private bool enemyActionDone;
        private bool playerDoOnce;

        [SerializeField] private TMP_Text roundTextValue;
        [SerializeField] private GameObject endTurnButton;
        [SerializeField] private GameObject atkOptions;
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private Player player;
        

        // Start is called before the first frame update
        void Start()
        {
            roundStarted = false;
            roundNumber = 0;
            turn = TurnType.Player;
            enemyTurnDone = false;
            atkOptions.SetActive(true);

            UpdateTurnText();

            StartNewRound();

            enemyActionDone = false;

            playerDoOnce = true;

        }

        // Update is called once per frame
        void Update()
        {
            if (roundStarted && turn == TurnType.Player)
            {
                PlayerTurn();
            }

            if (roundStarted && turn == TurnType.Enemy)
            {
                if(!enemyActionDone)
                {
                    StartCoroutine(EnemyTurn());
                    enemyActionDone = true;
                }
            }
        }

        public void StartNewRound()
        {
            roundStarted = true;
            roundNumber = 1;
            turn = TurnType.Player;

            UpdateTurnText();
        }

        private void PlayerTurn()
        {
            Debug.Log("Player's Turn!");
            if (enemyActionDone)
            {
                enemyActionDone = false;
            }

            if(playerDoOnce)
            {
                atkOptions.SetActive(true);
                battleManager.CreateBattleButtons();

                player.UpdateShield(-player.playerCurrentShield);

                // Redundant object cleanup
                GameObject[] redundantObjs = GameObject.FindGameObjectsWithTag("Redundant");
                foreach(GameObject obj in redundantObjs)
                {
                    Destroy(obj);
                }

                playerDoOnce = false;
            } 
        }

        IEnumerator EnemyTurn()
        {
            Debug.Log("Enemies' Turn!");


            atkOptions.SetActive(false);
            foreach (Button atkbtn in battleManager.atkButtons)
            {
                Destroy(atkbtn.gameObject);
            }

            // Do enemy actions
            if (battleManager.enemyObjects.Count > 0)
            {
                battleManager.enemyObjects[0].GetComponent<Enemy>().Attack();
                enemyTurnDone = true;
            } else { Debug.LogError("No more enemies to perform actions. They're all dead!"); }

            yield return new WaitForSeconds(2);

            // When complete, end turn & round
            if (enemyTurnDone)
            {
                roundNumber++;
                UpdateTurnText();
                EndTurn();
                enemyTurnDone = false;
                playerDoOnce = true;
            }
        }

        public void EndTurn()
        {
            if(turn == TurnType.Player)
            {
                turn = TurnType.Enemy;
                endTurnButton.SetActive(false);
            }
            else if (turn == TurnType.Enemy)
            {
                turn = TurnType.Player;
                endTurnButton.SetActive(true);
                player.playerState = Player.State.Normal;

                if (player.playerCurrentEnergy < player.playerMaxEnergy)
                {
                    player.UpdateEnergy(player.playerMaxEnergy - player.playerCurrentEnergy, 0);
                }

                for (int i = 0; i < battleManager.enemyObjects.Count; i++)
                {
                    battleManager.enemyObjects[i].GetComponent<Enemy>().UpdateAttack();
                }
                
            }
        }

        private void UpdateTurnText()
        {
            roundTextValue.text = roundNumber.ToString();
        }
    }
}
