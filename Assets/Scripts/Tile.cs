using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColour, offsetColour;
    [SerializeField] private Image image;
    [SerializeField] private GameObject highlight;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
    }

    public void Init(bool isOffset)
    {
        image.color = isOffset ? offsetColour : baseColour;
    }

    private void OnMouseEnter()
    {
        Debug.Log("mousey");
        highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }
}
