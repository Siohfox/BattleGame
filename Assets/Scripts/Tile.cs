using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public enum TileState { Unused, Used, Active, Selectable };

    public Color baseColour, offsetColour, activeColour, usedColour, selectableColour;
    [SerializeField] private Image image;
    [SerializeField] private GameObject highlight;

    public TileState currentState;

    private int randomScene;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
    }

    private void Start()
    {
        currentState = TileState.Unused;
        randomScene = Random.Range(1, 3); // 1 or 2
    }

    public void Init(bool isOffset)
    {
        if (currentState == TileState.Unused)
        {
            image.color = isOffset ? offsetColour : baseColour;
        }
    }

    public TileState GetTileState()
    {
        return currentState;
    }

    public void SetTileState(TileState state)
    {
        currentState = state;

        if(currentState == TileState.Active)
        {
            image.color = activeColour;
        }
        if(currentState == TileState.Selectable)
        {
            image.color = selectableColour;
            GetComponent<Button>().onClick.AddListener(LoadRandomSceneThing);

        }
        if (currentState == TileState.Unused)
        {
            
        }
        if (currentState == TileState.Used)
        {
            image.color = usedColour;
        }
    }

    private void LoadRandomSceneThing()
    {
        Debug.Log("Loading random scene thing");
        LevelLoader.Instance.LoadScene(randomScene);
        MapManager.Instance.ToggleMap();
    }
}
