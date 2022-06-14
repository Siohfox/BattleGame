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

    // AudioClips
    private AudioClip mapCrumpleClip;

    // Variables
    bool mapClosable;

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

        // Create a new map child of Map gameobject
        Image newMap = Instantiate(backgroundImage, GameObject.Find("Map").transform.position, Quaternion.identity, GameObject.Find("Map").transform);
        newMap.transform.SetAsFirstSibling();

        // Generate map grid and set it to not be visible when done
        GenerateGrid(newMap);
        mapObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && mapObject.activeSelf == false || Input.GetKeyDown(KeyCode.Escape) && mapObject.activeSelf == false)
        {
            mapObject.SetActive(true);
            MusicPlayer.Instance.PlaySound(mapCrumpleClip, 10);
        }
        else if (Input.GetKeyDown(KeyCode.M) && mapObject.activeSelf == true && mapClosable || Input.GetKeyDown(KeyCode.Escape) && mapObject.activeSelf == true && mapClosable)
        {
            mapObject.SetActive(false);
            MusicPlayer.Instance.PlaySound(mapCrumpleClip, 10);
        } 
    }

    public void ToggleMap(bool _mapClosable)
    {
        if(mapObject.activeSelf == false)
        {
            mapObject.SetActive(true);
            MusicPlayer.Instance.PlaySound(mapCrumpleClip, 10);
            mapClosable = _mapClosable;
        }
        else if (mapObject.activeSelf == true)
        {
            mapObject.SetActive(false);
            MusicPlayer.Instance.PlaySound(mapCrumpleClip, 10);
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

        GetTileAtPosition(new Vector2(0, 0)).SetTileState(Tile.TileState.Active);
        GetTileAtPosition(new Vector2(4, 4)).SetTileState(Tile.TileState.Used);

        CalculateSelectableTiles();
    }

    public void CalculateSelectableTiles()
    {
        Vector2 activeTile = new Vector2(3, 3); // defaulted to 3x3y to check for errors
        foreach (KeyValuePair<Vector2, Tile> dictionaryTile in _tiles)
        {
            if (dictionaryTile.Value.GetTileState() == Tile.TileState.Active)
            {
                activeTile = dictionaryTile.Key;
            }
        }

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

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if(_tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
    }
}
