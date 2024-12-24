using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int cellSize;
    [SerializeField] private RectTransform stoneGridHolder;
    [SerializeField] private Transform stoneGridPrefab;
    private int [,] gridArray;

    private void Start()
    {
        gridArray = new int [width, height];
        SetUpGridSize ();
    }

    private void SetUpGridSize ()
    {
        stoneGridHolder.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, width * cellSize);
        stoneGridHolder.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, height * cellSize);
        GridLayoutGroup stoneGridLayoutGroup = stoneGridHolder.GetComponent<GridLayoutGroup>();
        if (stoneGridLayoutGroup)
        {
            stoneGridLayoutGroup.cellSize = new Vector2 (cellSize, cellSize);
        }     
    }
}
