using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(StoneBlockPool))]
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
    private int attempToPlaceGem = 0;
    private StoneBlockPool stoneBlockPool;
    private float cellSize;
    private StoneBlock[,] gridArray;
    private List<int> blockStoredIndex;
    private int currentIndex = 0;
    private Coroutine coroutinePlaceGem;

    private void Awake()
    {
        stoneBlockPool = gameObject.GetComponent<StoneBlockPool>();


    }
    private void Start()
    {
        ResetBoard();
    }

    private void SetUpBoard()
    {
        //Init array
        gridArray = new StoneBlock[gridRange, gridRange];
        // set up grid
        SetUpGridSize();
        //load stone block on grid
        LoadStoneBlock();
        // random place gem on grid
        PlaceALlGemOnList();
    }


    public void ResetBoard()
    {
        foreach (Transform child in stoneGridHolder.transform)
        {
            GameObject childGameObject = child.gameObject;
            childGameObject.SetActive(false);
            childGameObject.GetComponent<StoneBlock>().ResetComponent();
        }

        foreach (Transform child in gemGridHolder.transform)
        {
            Destroy(child.gameObject);
        }

        SetUpBoard();
    }

    public void ResetGem()
    {
        foreach (StoneBlock block in gridArray)
        {
            if (block.CanPut == false)
            {
                block.SetCanPutStatus(true);
            }
        }

        foreach (Transform child in gemGridHolder.transform)
        {
            Destroy(child.gameObject);
        }

        // random place gem on grid
        PlaceALlGemOnList();
    }

    //auto resize block and block holder
    private void SetUpGridSize()
    {
        GridLayoutGroup stoneGridLayoutGroup = stoneGridHolder.GetComponent<GridLayoutGroup>();
        if (stoneBlockPool is null) return;
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
        //init blockStoredIndex

        blockStoredIndex = new List<int>();
        currentIndex = 0;
        // instatiate stone block button
        for (int row = 0; row < gridRange; row++)
        {
            for (int col = 0; col < gridRange; col++)
            {
                int localCols = col;
                int localRows = row;
                //GameObject blockText = Instantiate(stoneBlockPrefab, stoneGridHolder);
                GameObject blockText = stoneBlockPool.GetStoneBlockFromPool(stoneBlockPrefab, stoneGridHolder);

                gridArray[localRows, localCols] = blockText.GetComponent<StoneBlock>();
                // create local copy of i and j
                gridArray[localRows, localCols].SetXY(localRows, localCols);
                gridArray[localRows, localCols].SetBlockText($"{localRows}, {localCols}");

                //addIndex
                blockStoredIndex.Add(currentIndex);
                gridArray[localRows, localCols].Id = currentIndex;
                currentIndex++;

                //add listener
                gridArray[localRows, localCols].GetComponent<Button>().onClick.AddListener(() => OnClickedBlockAction?.Invoke(gridArray[localRows, localCols]));
            }
        }

    }

    //check if the grid can put gem
    private bool CheckCanPut(StoneBlock block, PlaceGemTypeSO placeGem)
    {

        if (block.Row + placeGem.GemHeight > gridRange)
        {
            return false;
        }
        if (block.Col + placeGem.GemWidth > gridRange)
        {
            return false;
        }
        for (int row = block.Row; row < block.Row + placeGem.GemHeight; row++)
        {
            for (int col = block.Col; col < block.Col + placeGem.GemWidth; col++)
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
        if (CheckCanPut(block, lPlaceGem[0]) == false)
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
        if (CheckCanPut(block, placeGem) == false)
        {
            return false;
        }

        RectTransform blockPos = block.gameObject.GetComponent<RectTransform>();

        //set infor for gem then create it 
        placeGem.InitGemInfo(blockPos, cellSize);

        //load gem size to set status for block road
        for (int row = block.Row; row < block.Row + placeGem.GemHeight; row++)
        {
            for (int col = block.Col; col < block.Col + placeGem.GemWidth; col++)
            {
                gridArray[row, col].SetCanPutStatus(false);
                placeGem.InitGemLocation(gridArray[row, col].Id);

            }
        }
        placeGem.CreateGemGameObject(gemGridHolder);
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

    //random place gem on board
    private bool PlaceRandomGem(PlaceGemTypeSO placeGem)
    {
        if (placeGem is null) return false;

        List<int> availabeIndex = new List<int>();

        int totalBlocks = gridRange * gridRange;
        for (int i = 0; i < totalBlocks; i++)
        {
            availabeIndex.Add(i);
        }

        bool isPlaced = false;

        // if attemp to place gem is higher than 5, we won't do it auto anymore
        if (attempToPlaceGem > 5)
        {
            for (int i = 0; i <= availabeIndex.Count; i++)
            {
                StoneBlock selectBLock = GetBlockFromIndex(i);

                isPlaced = PlaceGemAuto(selectBLock, placeGem);
                if (isPlaced == true)
                {
                    Debug.Log("Gem Placed");
                    break;
                }
            }
        }
        else

            while (availabeIndex.Count > 0)
            {
                //Select random stone block 
                int randomIndex = UnityEngine.Random.Range(0, availabeIndex.Count);

                StoneBlock selectBLock = GetBlockFromIndex(randomIndex);

                isPlaced = PlaceGemAuto(selectBLock, placeGem);
                if (isPlaced == true)
                {
                    break;
                }

                availabeIndex.RemoveAt(randomIndex);
            }

        return isPlaced;
    }

    // coroutine to place gem
    private IEnumerator IEPlaceAllGemOnList()
    {
        yield return new WaitForEndOfFrame();
        if (lPlaceGem is null) yield break;
        if (CheckIfGemListIsValid() is false)
        {
            yield break;
        }
        if (wantAutoPlace is false) yield break;
        foreach (PlaceGemTypeSO gem in lPlaceGem)
        {
            yield return new WaitForEndOfFrame();
            bool checkPlace = PlaceRandomGem(gem);
            if (checkPlace == false)
            {
                // if 1 gem in list can place, reset gem and place again
                attempToPlaceGem++;
                ResetGem();
                yield break;
            }
        }
        attempToPlaceGem = 0;

    }

    //place gem fuction
    public void PlaceALlGemOnList()
    {
        if (coroutinePlaceGem is not null)
        {
            StopCoroutine(coroutinePlaceGem);
        }

        coroutinePlaceGem = StartCoroutine(IEPlaceAllGemOnList());
    }

    private bool CheckIfGemListIsValid()
    {
        int totalSize = 0;
        foreach (PlaceGemTypeSO gem in lPlaceGem)
        {
            if (gem.GemWidth > gridRange || gem.GemHeight > gridRange)
            {
                Debug.LogError($"Gem width = {gem.GemWidth}, Gem Heigh = {gem.GemHeight} Grid Range = {gridRange}");
                return false;
            }
            totalSize = totalSize + gem.GemWidth + gem.GemHeight - 1;
        }

        if (totalSize > gridRange * gridRange)

        {
            Debug.LogError("Total size: " + totalSize);
            return false;
        }
        return true;
    }

    public void CheckAllGemLocation(StoneBlock block)
    {
        
        foreach (PlaceGemTypeSO gem in lPlaceGem)
        {
            if (gem.LLocationsOfGem.Contains(block.Id))
            {
                gem.RemoveGemLocation(block.Id);
            }
        }
    }
}



