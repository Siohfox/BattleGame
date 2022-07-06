using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BG.Core;
using System;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    // References to UI elements
    [SerializeField] private TMP_Text bonesText;
    [SerializeField] private TMP_Text tilesExploredText;

    // Start is called before the first frame update
    void Start() 
    {
        try
        {
            bonesText = GameObject.Find("BonesText").GetComponent<TMP_Text>();
            tilesExploredText = GameObject.Find("LevelText").GetComponent<TMP_Text>();
        }
        catch (Exception)
        {
            Debug.LogWarning("Missing UI element references (Refs not found, check names)!");
        }

        UpdateUIElements();
    }

    public void UpdateUIElements()
    {
        bonesText.text = $"Bones: {GameState.Instance.bonesAmount}";
        tilesExploredText.text = $"Explored: {GameState.Instance.tilesExploredAmount}";
    }
}
