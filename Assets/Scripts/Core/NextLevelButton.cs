using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelButton : MonoBehaviour
{
    private MapManager mapManager;

    // Start is called before the first frame update
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();

        GetComponent<Button>().onClick.AddListener(MapThingy);
    }

    private void MapThingy()
    {
        mapManager.ToggleMap(false);
        MapManager.Instance.CalculateSelectableTiles();
    }
}
