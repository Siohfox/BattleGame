using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private int width, height;

    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Image backgroundImage; 

    private void Start()
    {
        Image newMap = Instantiate(backgroundImage, GameObject.Find("Map").transform.position, Quaternion.identity, GameObject.Find("Map").transform);
        newMap.transform.SetAsFirstSibling();

        GenerateGrid(newMap);
    }

    void GenerateGrid(Image map)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity, GameObject.Find("Tiles").transform);
                spawnedTile.transform.position = map.transform.GetChild(0).transform.position + new Vector3(50, 50);
                spawnedTile.name = $"Tile {x} {y}";
            }
        }
    }
}
