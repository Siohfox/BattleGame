using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BG.Battle;

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

        [SerializeField] private TMP_Text roundTextValue;
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
                EnemyTurn();
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
            atkOptions.SetActive(true);
        }

        private void EnemyTurn()
        {
            Debug.Log("Enemies' Turn!");
            atkOptions.SetActive(false);
            enemyActionDone = false;

            // Do enemy actions
            if (!enemyActionDone)
            {
                if(battleManager.enemyObjects.Count > 0)
                {
                    battleManager.enemyObjects[0].GetComponent<Enemy>().Attack();
                    enemyTurnDone = true;
                }
            }
            

            if (Input.GetKeyDown(KeyCode.P))
            {
                enemyTurnDone = true;
            }


            // When complete, end turn & round
            if (enemyTurnDone)
            {
                roundNumber++;
                UpdateTurnText();
                EndTurn();
                enemyTurnDone = false;
            }
        }

        public void EndTurn()
        {
            if(turn == TurnType.Player)
            {
                turn = TurnType.Enemy;
            }
            else if (turn == TurnType.Enemy)
            {
                turn = TurnType.Player;

                if(player.playerCurrentEnergy < player.playerMaxEnergy)
                {
                    player.UpdateEnergy(player.playerMaxEnergy - player.playerCurrentEnergy, 0);
                }
            }
        }

        private void UpdateTurnText()
        {
            roundTextValue.text = roundNumber.ToString();
        }
    }
}
