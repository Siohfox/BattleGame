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
        enum EnemyActionDecision { Attack, Defend, Other };

        private bool roundStarted;
        private int roundNumber;
        private TurnType turn;
        private EnemyActionDecision enemyActionDecision;
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
            enemyActionDecision = EnemyActionDecision.Attack;

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

            // Set all player attack buttons off so they can't click them during enemy turn
            // Then destroy them so they can be replaced with new ones next player turn
            atkOptions.SetActive(false);
            foreach (Button atkbtn in battleManager.atkButtons)
            {
                Destroy(atkbtn.gameObject);
            }

            // Do enemy actions
            if (battleManager.enemyObjects.Count > 0)
            {
                for (int i = 0; i < battleManager.enemyObjects.Count; i++)
                {
                    if (enemyActionDecision == EnemyActionDecision.Attack)
                    {
                        battleManager.enemyObjects[i].GetComponent<Enemy>().Attack();
                    }
                    else if (enemyActionDecision == EnemyActionDecision.Defend)
                    {
                        battleManager.enemyObjects[i].GetComponent<Enemy>().Defend();
                    }
                    else if (enemyActionDecision == EnemyActionDecision.Other)
                    {
                        Debug.Log("Enemy using some other action");
                    }
                    else
                    {
                        Debug.LogError("Enemy decision out of range error");
                    }
                }
                
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
            // Set turn to enemy if it is player's turn and disable the end turn button
            if(turn == TurnType.Player)
            {
                turn = TurnType.Enemy;
                endTurnButton.SetActive(false);

                for (int i = 0; i < battleManager.enemyObjects.Count; i++)
                {
                    battleManager.enemyObjects[i].GetComponent<Enemy>().enemyCurrentDefence = 0;
                    battleManager.enemyObjects[i].GetComponent<Enemy>().enemyShieldTextValue.text = battleManager.enemyObjects[i].GetComponent<Enemy>().enemyCurrentDefence.ToString();
                }
            }
            else if (turn == TurnType.Enemy)
            {
                turn = TurnType.Player;
                // Reset all player changes:
                endTurnButton.SetActive(true);
                player.playerState[0] = Player.State.Normal;
                player.playerStateIcon.SetActive(false);
                player.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                battleManager.playerDefenceBonus = 1;

                // Reset player energy
                if (player.playerCurrentEnergy < player.playerMaxEnergy)
                {
                    player.UpdateEnergy(player.playerMaxEnergy - player.playerCurrentEnergy, 0);
                }

                // If player is energized, give them one extra energy
                if(player.playerState[1] == Player.State.Energized)
                {
                    player.UpdateEnergy(1,0);
                }

                enemyActionDecision = (EnemyActionDecision)Random.Range(0,2);

                // Update enemies action decision
                for (int i = 0; i < battleManager.enemyObjects.Count; i++)
                {
                    if(enemyActionDecision == EnemyActionDecision.Attack)
                    {
                        battleManager.enemyObjects[i].GetComponent<Enemy>().UpdateAttack(0);
                    }
                    else if(enemyActionDecision == EnemyActionDecision.Defend)
                    {
                        battleManager.enemyObjects[i].GetComponent<Enemy>().UpdateDefence(0);;
                    }
                    else if(enemyActionDecision == EnemyActionDecision.Other)
                    {
                        Debug.Log("Enemy using some other action");
                    }
                    else
                    {
                        Debug.LogError("Enemy decision out of range error");
                    }
                }

                battleManager.actionsUsed = 0;
            }
        }

        private void UpdateTurnText()
        {
            roundTextValue.text = "Round: " + roundNumber.ToString();
        }
    }
}
