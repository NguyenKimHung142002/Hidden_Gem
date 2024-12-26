using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField, Range(2, 8)] private int width;
    [SerializeField] private int maxCellsize = 200;
    [SerializeField] private int minCellsize = 100;

    [SerializeField] private RectTransform mineGridHolder;
    [SerializeField] private RectTransform stoneGridHolder;
    [SerializeField] private RectTransform gemGridHolder;
    [SerializeField] private GameObject stoneBlockPrefab;
    [SerializeField] private List<PlaceGemTypeSO> lPlaceGem;
    public UnityEvent<StoneBlock> OnClickedBlockAction;
    private int maxWidth = 8;
    private int minWidth = 2;

    private float cellSize;
    private StoneBlock[,] gridArray;


    private void Start()
    {
        gridArray = new StoneBlock[width, width];
        SetUpGridSize();
        LoadStoneBlock();


    }
    private void Update()
    {
        SetUpGridSize();
    }

    //auto resize block and block holder
    private void SetUpGridSize()
    {
        GridLayoutGroup stoneGridLayoutGroup = stoneGridHolder.GetComponent<GridLayoutGroup>();
        if (stoneGridLayoutGroup is null)
        {
            Debug.LogError("Missing GridLayoutGroup");
            return;
        }
        ResetCellsize();
        float mCellSize = Mathf.Lerp(maxCellsize, minCellsize, (float)(width - minWidth) / (maxWidth - minWidth));
        cellSize = Mathf.Clamp(mCellSize, minCellsize, maxCellsize);

        mineGridHolder.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * cellSize);
        mineGridHolder.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, width * cellSize);

        stoneGridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);

    }

    private void LoadStoneBlock()
    {

        // instatiate stone block button
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var blockText = Instantiate(stoneBlockPrefab, stoneGridHolder);
                gridArray[i, j] = blockText.GetComponent<StoneBlock>();
                // create local copy of i and j
                int a = i;
                int b = j;

                gridArray[i, j].SetBlockText($"{i}, {j}");

                //add listener
                gridArray[i, j].GetComponent<Button>().onClick.AddListener(() => OnClickedBlockAction?.Invoke(gridArray[a, b]));


            }

        }


    }

    public void PlaceGem(StoneBlock block)
    {
        if (lPlaceGem.Count == 0) return;
        RectTransform blockPos = block.gameObject.GetComponent<RectTransform>();

        //set infor for gem then create it 
        lPlaceGem[0].InitGemInfo(cellSize);
        GameObject gem = Instantiate(lPlaceGem[0].GetGemPrefabs(),blockPos.position, quaternion.identity,gemGridHolder);
        



    }


    private void ResetCellsize()
    {
        if (maxCellsize < 0) maxCellsize = 0;

        if (minCellsize < 0) minCellsize = 0;

        if (minCellsize > maxCellsize) minCellsize = maxCellsize;
    }


    public void ShowBlockStatus(StoneBlock block)
    {
        // Debug.Log("BLock: " + block);
        block.SetBlockText("true");
    }

}
