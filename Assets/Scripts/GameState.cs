using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BG.Turns;

namespace BG.Core
{
    public class GameState : MonoBehaviour
    {
        [SerializeField] private TMP_Text goldTextValue;
        [SerializeField] private TMP_Text levelTextValue;

        [SerializeField] private TurnManager turnManager;

        private int goldAmount;
        private int levelAmount;

        // Start is called before the first frame update
        void Start()
        {

            goldAmount = 0;
            levelAmount = 0;

            goldTextValue.text = goldAmount.ToString();
            levelTextValue.text = levelAmount.ToString();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                turnManager.StartNewRound();
            }
        }
    }
}
