using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public enum TileState { Unused, Used, Active, Selectable };
    public enum TileType { Normal, ChooseActionEvent, ChoosePowerEvent, Shop, Boss };

    public Color baseColour, offsetColour, activeColour, usedColour, selectableColour, bossColour;
    [SerializeField] private Image image;
    [SerializeField] private GameObject highlight;

    private Color savedColor;

    public Vector2 tileLocation;
    float chooseWeightPercent;

    public TileState currentState;
    public TileType currentType;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
    }

    private void Start()
    {
        chooseWeightPercent = Random.Range(0.0f, 100.0f); //0-100
        float length = System.Enum.GetValues(typeof(TileType)).Length;

        if (chooseWeightPercent < 100.0f / length)
        {
            Debug.Log(1);
        }
        else if(chooseWeightPercent < 100.0f / length * 2)
        {
            Debug.Log(2);
        }
        else if (chooseWeightPercent < 100.0f / length * 3)
        {
            Debug.Log(3);
        }
        else if (chooseWeightPercent < 100.0f / length * 4)
        {
            Debug.Log(4);
        }
        else if (chooseWeightPercent < 100.0f / length * 5)
        {
            Debug.Log(5);
        }
        else
        {
            Debug.LogError("Something went wrong");
        }

        TileType randomType = (TileType)Random.Range(0, 4);
        currentType = randomType;
        currentState = TileState.Unused;
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
            savedColor = image.color;
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
            if (currentType != TileType.Boss)
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
                LoadScene(2);
            }
            else if (currentType == TileType.ChooseActionEvent)
            {
                LoadScene(3);
            }
            else if (currentType == TileType.ChoosePowerEvent)
            {
                LoadScene(4);
            }
            else if (currentType == TileType.Boss)
            {
                LoadScene(2);
            }
            else if (currentType == TileType.Shop)
            {
                LoadScene(5);
            }

            // Calculate new selectable tiles around new active tile
            //MapManager.Instance.CalculateSelectableTiles();        
        } 
    }

    private void LoadScene(int sceneIndex)
    {
        MapManager.Instance.ToggleMap(true);
        LevelLoader.Instance.LoadScene(sceneIndex);
    }
}
