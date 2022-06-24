using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public enum TileState { Unused, Used, Active, Selectable };
    public enum TileType { Normal, Boss };

    public Color baseColour, offsetColour, activeColour, usedColour, selectableColour, bossColour;
    [SerializeField] private Image image;
    [SerializeField] private GameObject highlight;

    private Color savedColor;

    public Vector2 tileLocation;

    public TileState currentState;
    public TileType currentType;

    private int randomScene;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
    }

    private void Start()
    {
        currentType = TileType.Normal;
        currentState = TileState.Unused;
        randomScene = Random.Range(1, 3); // 1 or 2

        Debug.Log("Making tile");
    }

    public void Init(bool isOffset)
    {
        if (currentState == TileState.Unused)
        {
            image.color = isOffset ? offsetColour : baseColour;
            savedColor = image.color;
        }
    }

    public TileState GetTileState()
    {
        return currentState;
    }

    public TileType GetTileType()
    {
        return currentType;
    }

    public void SetTileType(TileType type)
    {
        currentType = type;

        if(currentType == TileType.Normal)
        {
            currentType = TileType.Normal;
        }
        if (currentType == TileType.Boss)
        {
            currentType = TileType.Boss;
            image.color = bossColour;
        }
    }

    public void SetTileState(TileState state)
    {
        currentState = state;

        if(currentState == TileState.Active)
        {
            currentState = TileState.Active;
            GetComponent<Button>().onClick.RemoveAllListeners();
            image.color = activeColour;
        }
        if(currentState == TileState.Selectable)
        {
            // 1 - Set state to selectable & set colour
            currentState = TileState.Selectable;
            if (currentType == TileType.Normal)
            {
                image.color = selectableColour;
            }
            else if (currentType == TileType.Boss)
            {
                image.color = bossColour;
            }

            // Add load scene listener
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(CalculateNextMapTiles);
        }
        if (currentState == TileState.Unused)
        {
            image.color = savedColor;
            currentState = TileState.Unused;
            GetComponent<Button>().onClick.RemoveAllListeners();
        }
        if (currentState == TileState.Used)
        {
            currentState = TileState.Used;
            GetComponent<Button>().onClick.RemoveAllListeners();
            image.color = usedColour;
        }
    }

    /// <summary>
    /// Calculates the next available choices for the player to go to in the map
    /// </summary>
    public void CalculateNextMapTiles()
    {
        if (MapManager.Instance.mapUsable == true)
        {
            // Scuffed solution -- for some reason first tile won't be set to used
            //MapManager.Instance.GetTileAtPosition(new Vector2(0, 0)).SetTileState(TileState.Used);

            // Remove any active tiles
            foreach (var dictionaryTile in MapManager.Instance._tiles)
            {
                if (dictionaryTile.Value.GetTileState() == TileState.Active)
                {
                    dictionaryTile.Value.SetTileState(TileState.Used);
                }
            }

            

            // Reset any selectable tiles before making new ones
            foreach (var dictionaryTile in MapManager.Instance._tiles)
            {
                if (dictionaryTile.Value.GetTileState() == TileState.Selectable)
                {
                    dictionaryTile.Value.SetTileState(TileState.Unused);
                }
            }

            // Set chosen tile as active
            SetTileState(TileState.Active);

            // Load new scene
            if (currentType == TileType.Normal)
            {

                LoadRandomSceneThing();
            }
            else if (currentType == TileType.Boss)
            {
                LoadBossScene();
            }

            // Calculate new selectable tiles around new active tile
            //MapManager.Instance.CalculateSelectableTiles();        
        } 
    }

    private void LoadBossScene()
    {
        Debug.Log("Loading boss scene");
        MapManager.Instance.ToggleMap(true);
        LevelLoader.Instance.LoadScene(2);
    }

    private void LoadRandomSceneThing()
    {
        Debug.Log("Loading normal scene");
        MapManager.Instance.ToggleMap(true);
        LevelLoader.Instance.LoadScene(randomScene);
    }


    public void RandomThingy()
    {
        foreach (var dictionaryTile in MapManager.Instance._tiles)
        {
            if (dictionaryTile.Value.GetTileType() == TileType.Boss)
            {
                Debug.Log("TILE " + dictionaryTile.Value.name + " is a boss tile still!!");
            }
        }
    }
}
