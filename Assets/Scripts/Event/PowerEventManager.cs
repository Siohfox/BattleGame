using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerEventManager : MonoBehaviour
{
    [SerializeField] private Button powerButtonPrefab;
    [SerializeField] private GameObject powerButtonContainer;
    [SerializeField] private GameObject nextScreenButton;

    // Start is called before the first frame update
    void Start()
    {
        CreatePowerButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreatePowerButton()
    {
        
        Button button = Instantiate(powerButtonPrefab, powerButtonContainer.transform.position, Quaternion.identity, powerButtonContainer.transform);
        

        button.name = "Power Button " + "x";
    }

    private void FinishEvent()
    {
        powerButtonContainer.SetActive(false);
        nextScreenButton.SetActive(true);
    }
}
