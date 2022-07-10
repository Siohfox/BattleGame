using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    // Enums
    public enum TileState { Unused, Used, Active, Selectable };
    public enum TileType { Normal, ChooseActionEvent, ChoosePowerEvent, Shop, Rest, Boss };
    

    // Inspector Variables
    public Color baseColour, offsetColour, activeColour, usedColour, selectableColour, bossColour;
    [SerializeField] private Image image;
    [SerializeField] private GameObject highlight;
    
    // Variables
    private Color savedColor;
    public Vector2 tileLocation;
    private int[] weights;
    private int weightTotal;
    [System.NonSerialized] public TileState currentState;
    [System.NonSerialized] public TileType currentType;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
    }

    private void Start()
    {
        int length = System.Enum.GetValues(typeof(TileType)).Length - 1; // all types minus boss tile
        weights = new int[length];

        // Weighting of each Tile Type, high number means more occurrance
        weights[(int)TileType.Normal] = 4;
        weights[(int)TileType.ChooseActionEvent] = 3;
        weights[(int)TileType.ChoosePowerEvent] = 3;
        weights[(int)TileType.Shop] = 2;
        weights[(int)TileType.Rest] = 20;

        // Set the total weight value so it knows how much weight there is
        weightTotal = 0;
        foreach (int w in weights)
        {
            weightTotal += w;
        }

        // Select random tile type depending on weighted outcome
        TileType randomType = (TileType)RandomWeighted();
        currentType = randomType;

        Debug.Log("My type is: " + currentType.ToString());

        currentState = TileState.Unused;
    }

    /// <summary>
    /// Generates a random value based on the <see cref="weights"/> list
    /// </summary>
    /// <returns>an integer value equal to one of the weights list</returns>
    int RandomWeighted()
    {
        int result, total = 0;
        int randVal = Random.Range(0, weightTotal + 1);
        for(result = 0; result < weights.Length; result ++)
        {
            total += weights[result];
            if (total >= randVal)
            {
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// Initialises tile's look
    /// </summary>
    /// <param name="isOffset"></param>
    public void Init(bool isOffset)
    {
        if (currentState == TileState.Unused)
        {
            image.color = isOffset ? offsetColour : baseColour;
            savedColor = image.color;
        }
    }

    // Tile state getter
    public TileState GetTileState()
    {
        return currentState;
    }

    // Tile type getter
    public TileType GetTileType()
    {
        return currentType;
    }

    /// <summary>
    /// Sets the Tile's type and changes look depending on type.
    /// </summary>
    /// <param name="type"></param>
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

    /// <summary>
    /// Sets tile state
    /// </summary>
    /// <param name="state">The tile state to switch to</param>
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
            else if (currentType == TileType.Rest)
            {
                LoadScene(6);
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
