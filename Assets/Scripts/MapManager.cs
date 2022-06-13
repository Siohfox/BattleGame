using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private int width, height;

    [SerializeField] private GameObject mapObject;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private float offsetX, offsetY, tileSpacingX, tileSpacingY;

    private AudioClip mapCrumpleClip;

    public static MapManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

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
        mapCrumpleClip = Resources.Load<AudioClip>("Sounds/MapCrumple");

        mapObject = transform.GetChild(0).transform.GetChild(0).gameObject;

        Image newMap = Instantiate(backgroundImage, GameObject.Find("Map").transform.position, Quaternion.identity, GameObject.Find("Map").transform);
        newMap.transform.SetAsFirstSibling();

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
        else if (Input.GetKeyDown(KeyCode.M) && mapObject.activeSelf == true || Input.GetKeyDown(KeyCode.Escape) && mapObject.activeSelf == true)
        {
            mapObject.SetActive(false);
            MusicPlayer.Instance.PlaySound(mapCrumpleClip, 10);
        } 
    }

    public void ToggleMap()
    {
        if(mapObject.activeSelf == false)
        {
            mapObject.SetActive(true);
            MusicPlayer.Instance.PlaySound(mapCrumpleClip, 10);
        }
        else if (mapObject.activeSelf == true)
        {
            mapObject.SetActive(false);
            MusicPlayer.Instance.PlaySound(mapCrumpleClip, 10);
        }
    }

    void GenerateGrid(Image map)
    {
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

            }
        }
    }
}
