using System;
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
    [SerializeField, Range(2, 8)] private int gridRange;
    [SerializeField] private int maxCellsize = 200;
    [SerializeField] private int minCellsize = 100;

    [SerializeField] private RectTransform mineGridHolder;
    [SerializeField] private RectTransform stoneGridHolder;
    [SerializeField] private RectTransform gemGridHolder;
    [SerializeField] private GameObject stoneBlockPrefab;
    [SerializeField] private List<PlaceGemTypeSO> lPlaceGem;
    [SerializeField] private bool wantAutoPlace = true;
    public UnityEvent<StoneBlock> OnClickedBlockAction;
    private int maxWidth = 8;
    private int minWidth = 2;

    private float cellSize;
    private StoneBlock[,] gridArray;
    private void Awake()
    {
        //Init array
        gridArray = new StoneBlock[gridRange, gridRange];
        // set up grid
        SetUpGridSize();
        //load stone block on grid
        LoadStoneBlock();
        // random place gem on grid
        StartCoroutine(PlaceAllGemOnList());
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
        //adjust board size
        ResetCellsize();
        //caculate size of cellsize
        float mCellSize = Mathf.Lerp(maxCellsize, minCellsize, (float)(gridRange - minWidth) / (maxWidth - minWidth));
        cellSize = Mathf.Clamp(mCellSize, minCellsize, maxCellsize);

        mineGridHolder.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gridRange * cellSize);
        mineGridHolder.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, gridRange * cellSize);

        stoneGridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
    }

    //load block list
    private void LoadStoneBlock()
    {
        // instatiate stone block button
        for (int row = 0; row < gridRange; row++)
        {
            for (int col = 0; col < gridRange; col++)
            {
                int localCols = col;
                int localRows = row;
                var blockText = Instantiate(stoneBlockPrefab, stoneGridHolder);
                gridArray[localRows, localCols] = blockText.GetComponent<StoneBlock>();
                // create local copy of i and j
                gridArray[localRows, localCols].SetXY(localRows, localCols);
                gridArray[localRows, localCols].SetBlockText($"{localRows}, {localCols}");

                //add listener
                gridArray[localRows, localCols].GetComponent<Button>().onClick.AddListener(() => OnClickedBlockAction?.Invoke(gridArray[localRows, localCols]));
            }
        }

    }

    //check if the grid can put gem
    private bool CheckCanPut(StoneBlock block)
    {

        if (block.Row + lPlaceGem[0].GemHeight > gridRange)
        {
            Debug.LogError("Out of index");
            return false;
        }
        if (block.Col + lPlaceGem[0].GemWidth > gridRange)
        {
            Debug.LogError("Out of index");
            return false;
        }
        for (int row = block.Row; row < block.Row + lPlaceGem[0].GemHeight; row++)
        {
            for (int col = block.Col; col < block.Col + lPlaceGem[0].GemWidth; col++)
            {
                if (gridArray[row, col].CanPut == false)
                    return false;
            }
        }

        return true;
    }
    public void PlaceGem(StoneBlock block)
    {
        //check if the block have gem
        if (CheckCanPut(block) == false)
        {
            Debug.LogError($"Can't put gem here");
            return;
        }
        if (lPlaceGem.Count == 0)
        {
            Debug.LogError("list is empty");
            return;
        };

        RectTransform blockPos = block.gameObject.GetComponent<RectTransform>();

        //set infor for gem then create it 
        lPlaceGem[0].InitGemInfo(blockPos, cellSize);
        GameObject gem = Instantiate(lPlaceGem[0].GetGemPrefabs(), gemGridHolder);


        //load gem size to set status for block road
        for (int row = block.Row; row < block.Row + lPlaceGem[0].GemHeight; row++)
        {
            for (int col = block.Col; col < block.Col + lPlaceGem[0].GemWidth; col++)
            {
                gridArray[row, col].SetCanPutStatus(false);
            }
        }
        return;
    }

    public bool PlaceGemAuto(StoneBlock block, PlaceGemTypeSO placeGem)
    {
        //check if the block have gem
        if (CheckCanPut(block) == false)
        {
            return false;
        }

        RectTransform blockPos = block.gameObject.GetComponent<RectTransform>();

        //set infor for gem then create it 
        placeGem.InitGemInfo(blockPos, cellSize);
        GameObject gem = Instantiate(placeGem.GetGemPrefabs(), gemGridHolder);


        //load gem size to set status for block road
        for (int row = block.Row; row < block.Row + placeGem.GemHeight; row++)
        {
            for (int col = block.Col; col < block.Col + placeGem.GemWidth; col++)
            {
                gridArray[row, col].SetCanPutStatus(false);
            }
        }


        return true;
    }


    private void ResetCellsize()
    {
        if (maxCellsize < 0) maxCellsize = 0;

        if (minCellsize < 0) minCellsize = 0;

        if (minCellsize > maxCellsize) minCellsize = maxCellsize;
    }

    private StoneBlock GetBlockFromIndex(int index)
    {
        int row = index / gridRange;
        int col = index % gridRange;
        return gridArray[row, col];
    }

    public void ShowBlockStatus(StoneBlock block)
    {
        // Debug.Log("BLock: " + block);
        // block.SetBlockText("true");
    }

    private bool PlaceRandomGem(PlaceGemTypeSO placeGem)
    {
        if (placeGem is null) return false;
        List<StoneBlock> selectedBlock = new List<StoneBlock>();
        int totalBlocks = gridRange * gridRange;
        int blocksLeft = totalBlocks;
        bool isPlaced = false;

        while (blocksLeft > 0)
        {
            //Select random stone block 
            int randomIndex = UnityEngine.Random.Range(0, totalBlocks);
            StoneBlock selectBLock = GetBlockFromIndex(randomIndex);

            //nếu đối tượng đã được chọn trước đó thì tiếp tục
            if (selectedBlock.Contains(selectBLock))
            {
                continue;
            }
            isPlaced = PlaceGemAuto(selectBLock, placeGem);
            if (isPlaced == true)
            {

                Debug.Log("Gem Placed");
                break;
            }
            blocksLeft--;

        }

        return isPlaced;
    }

    private IEnumerator PlaceAllGemOnList()
    {
        yield return new WaitForEndOfFrame();
        if (lPlaceGem is null) yield break;
        if (wantAutoPlace is false) yield break;
        foreach (PlaceGemTypeSO gem in lPlaceGem)
        {
            yield return new WaitForEndOfFrame();
            bool checkPlace = PlaceRandomGem(gem);
            if (checkPlace == false)
            {
                Debug.Log("Place fail");
            }
        }
    }
}



