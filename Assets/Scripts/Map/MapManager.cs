using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    // Singleton
    public static MapManager Instance { get; private set; }

    // Objects
    [SerializeField] private GameObject mapObject;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Image backgroundImage;

    // Map variables
    [SerializeField] private int width, height;
    [SerializeField] private float offsetX, offsetY, tileSpacingX, tileSpacingY;
    Vector2 activeTile = new Vector2(3, 3); // defaulted to 3x3y to check for errors

    // AudioClips
    private AudioClip mapCrumpleClip;

    // Variables
    public bool mapClosable;
    public bool mapUsable;

    // Dictionaries and lists
    public Dictionary<Vector2, Tile> _tiles;

    private void Awake()
    {
        // Singleton instance
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        // Make sure there can only ever be one map at a time
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Map");
        if (obj.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        // Load dependencies 
        mapCrumpleClip = Resources.Load<AudioClip>("Sounds/MapCrumple");
        mapObject = transform.GetChild(0).transform.GetChild(0).gameObject;

        // Set variables
        mapClosable = true;
        mapUsable = true;

        // Create a new map child of Map gameobject
        Image newMap = Instantiate(backgroundImage, GameObject.Find("Map").transform.position, Quaternion.identity, GameObject.Find("Map").transform);
        newMap.transform.SetAsFirstSibling();

        // Generate map grid and set it to not be visible when done
        GenerateGrid(newMap);

        StartCoroutine(StupidShit());
        
    }

    IEnumerator StupidShit()
    {
        yield return new WaitForSeconds(0.001f);
        mapObject.SetActive(false);
        SetTileTypes();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && mapObject.activeSelf == false || Input.GetKeyDown(KeyCode.Escape) && mapObject.activeSelf == false)
        {
            mapObject.SetActive(true);
            SfxPlayer.Instance.PlaySound(mapCrumpleClip, 1.0f);
        }
        else if (Input.GetKeyDown(KeyCode.M) && mapObject.activeSelf == true && mapClosable || Input.GetKeyDown(KeyCode.Escape) && mapObject.activeSelf == true && mapClosable)
        {
            mapObject.SetActive(false);
            SfxPlayer.Instance.PlaySound(mapCrumpleClip, 1.0f);
        } 
    }

    public void ToggleMap(bool _mapClosable)
    {
        if(mapObject.activeSelf == false)
        {
            mapObject.SetActive(true);
            if (SfxPlayer.Instance == null)
            {
                Debug.LogWarning("SFX player is missing!");
            }
            else
            {
                SfxPlayer.Instance.PlaySound(mapCrumpleClip, 1.0f);
            }
            mapClosable = _mapClosable;
        }
        else if (mapObject.activeSelf == true)
        {
            mapObject.SetActive(false);
            if (SfxPlayer.Instance == null)
            {
                Debug.LogWarning("SFX player is missing!");
            }
            else
            {
                SfxPlayer.Instance.PlaySound(mapCrumpleClip, 1.0f);
            }
        }
    }

    void GenerateGrid(Image map)
    {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.transform.SetParent(GameObject.Find("Tiles").transform);
                spawnedTile.transform.position = map.transform.GetChild(0).transform.position 
                + new Vector3(spawnedTile.GetComponent<Image>().rectTransform.rect.width/2 , -spawnedTile.GetComponent<Image>().rectTransform.rect.height / 2) // 
                + new Vector3(offsetX, -offsetY) //offset from top left to give border room
                + new Vector3(x * spawnedTile.GetComponent<Image>().rectTransform.rect.width, -(y * spawnedTile.GetComponent<Image>().rectTransform.rect.height))
                + new Vector3(x * tileSpacingX, -y * tileSpacingY);
                spawnedTile.name = $"Tile {x} {y}";

                var isOffset = (x % 2 == 0 && y % 2 != 0 || x % 2 != 0 && y % 2 == 0);

                spawnedTile.Init(isOffset);

                _tiles[new Vector2  (x, y)] = spawnedTile;
                spawnedTile.tileLocation = new Vector2(x,y);
            }
        }

        SetTileTypes();
        FigureOutActiveTile();
    }

    public void CalculateSelectableTiles()
    {

        FigureOutActiveTile();

        foreach (KeyValuePair<Vector2, Tile> dictionaryTile in _tiles)
        {
            //Debug.Log(dictionaryTile.Value.GetTileState().ToString()); // This debug shows how many tiles are in which state

            if (dictionaryTile.Value.GetTileState() != Tile.TileState.Used)
            {
                // Right side
                if (dictionaryTile.Key.x > activeTile.x)
                {
                    if (dictionaryTile.Key.x < activeTile.x + 2)
                    {
                        if (dictionaryTile.Key.y < activeTile.y + 2 && dictionaryTile.Key.y > activeTile.y - 2)
                        {
                            dictionaryTile.Value.SetTileState(Tile.TileState.Selectable);
                        }
                    }
                }

                // Left side
                if (dictionaryTile.Key.x < activeTile.x)
                {
                    if (dictionaryTile.Key.x > activeTile.x - 2)
                    {
                        if (dictionaryTile.Key.y < activeTile.y + 2 && dictionaryTile.Key.y > activeTile.y - 2)
                        {
                            dictionaryTile.Value.SetTileState(Tile.TileState.Selectable);
                        }
                    }
                }

                // Bottom side
                if (dictionaryTile.Key.y > activeTile.y)
                {
                    if (dictionaryTile.Key.y < activeTile.y + 2)
                    {
                        if (dictionaryTile.Key.x < activeTile.x + 2 && dictionaryTile.Key.x > activeTile.x - 2)
                        {
                            dictionaryTile.Value.SetTileState(Tile.TileState.Selectable);
                        }
                    }
                }

                // Top side
                if (dictionaryTile.Key.y < activeTile.y)
                {
                    if (dictionaryTile.Key.y > activeTile.y - 2)
                    {
                        if (dictionaryTile.Key.x < activeTile.x + 2 && dictionaryTile.Key.x > activeTile.x - 2)
                        {
                            dictionaryTile.Value.SetTileState(Tile.TileState.Selectable);
                        }
                    }
                }
            }
        }
    }


    public void FigureOutActiveTile()
    {
        foreach (KeyValuePair<Vector2, Tile> dictionaryTile in _tiles)
        {
            if (dictionaryTile.Value.GetTileState() == Tile.TileState.Active)
            {
                activeTile = dictionaryTile.Key;
            }
        }
    }

    /// <summary>
    /// Gets tile at a given Vector2 position and returns it if found, else return null
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Tile GetTileAtPosition(Vector2 pos)
    {
        if(_tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
    }

    public Tile GetActiveTile()
    {
        return GetTileAtPosition(activeTile);
    }

    public void GetAllTileTypes()
    {
        foreach (KeyValuePair<Vector2, Tile> dictionaryTile in _tiles)
        {
            Debug.Log("Tile type = " + dictionaryTile.Value.currentType);
        }

        
    }

    public void SetTileTypes()
    {
        // Set the tiles that will always be the same
        GetTileAtPosition(new Vector2(0, 0)).SetTileState(Tile.TileState.Active);

        GetTileAtPosition(new Vector2(1, 1)).SetTileType(Tile.TileType.Normal);
        GetTileAtPosition(new Vector2(0, 1)).SetTileType(Tile.TileType.Normal);
        GetTileAtPosition(new Vector2(1, 0)).SetTileType(Tile.TileType.Normal);

        GetTileAtPosition(new Vector2(width-1, height-1)).SetTileType(Tile.TileType.Boss);
        //GetTileAtPosition(new Vector2(width - 1, height - 1)).GetComponent<Image>().sprite = GetTileAtPosition(new Vector2(width - 1, height - 1)).GetTileTypeSprite();
    }
}
