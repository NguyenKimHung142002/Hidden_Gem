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
[RequireComponent(typeof(GemPool))]

public class GridManager : MonoBehaviour
{
    //[SerializeField, Range(2, 8)] private int gridRange;

    [SerializeField] private List<LevelDetailSO> lOfLevel;
    [SerializeField] private RectTransform mineGridHolder;
    [SerializeField] private RectTransform stoneGridHolder;
    [SerializeField] private RectTransform gemGridHolder;
    [SerializeField] private GameObject stoneBlockPrefab;
    [SerializeField] private int maxAttemptToPlaceGem = 10;
    //[SerializeField] private List<PlaceGemTypeSO> lPlaceGem;
    [SerializeField] private bool wantAutoPlace = true;
    [SerializeField] private CanvasManager canvasManager;
    public UnityEvent<StoneBlock, bool> OnClickedBlockAction;
    private int maxWidth = 8;
    private int minWidth = 2;
    private int attempToPlaceGem = 0;
    private int currentIndex = 0;
    private int currentLevel = 0;
    private float cellSize;
    private bool readyToPlace = false;
    private StoneBlockPool stoneBlockPool;

    private GemPool gemPool;

    private StoneBlock[,] gridArray;
    private List<int> blockStoredIndex;
    private List<GemValue> lAllGeminGrid;
    private List<PlaceGemTypeSO> validGemList;

    private Coroutine coroutinePlaceGem;

    private void Awake()
    {
        stoneBlockPool = gameObject.GetComponent<StoneBlockPool>();
        gemPool = gameObject.GetComponent<GemPool>();

        if (lOfLevel is null) Debug.LogError("List of level cannot null");

    }
    private void Start()
    {
        ResetBoard();
    }

    private void SetUpBoard()
    {
        //Init array
        gridArray = new StoneBlock[lOfLevel[currentLevel].GridRange, lOfLevel[currentLevel].GridRange];
        // set up grid
        SetUpGridSize();
        //load stone block on grid
        LoadStoneBlock();

        //get gem list
        validGemList = new List<PlaceGemTypeSO>(lOfLevel[currentLevel].LPlaceGem);
        validGemList = GetValidGemList();
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
            if (child.gameObject.activeSelf == true)
                gemPool.DeactiveGem(child.gameObject);
        }

        canvasManager.ResetStoredGem();

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
            if (child.gameObject.activeSelf == true)
                gemPool.DeactiveGem(child.gameObject);
        }
        canvasManager.ResetStoredGem();
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

        //caculate size of cellsize
        float mCellSize = Mathf.Lerp(lOfLevel[currentLevel].MaxCellsize, lOfLevel[currentLevel].MinCellsize, (float)(lOfLevel[currentLevel].GridRange - minWidth) / (maxWidth - minWidth));
        cellSize = Mathf.Clamp(mCellSize, lOfLevel[currentLevel].MinCellsize, lOfLevel[currentLevel].MaxCellsize);

        mineGridHolder.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lOfLevel[currentLevel].GridRange * cellSize);
        mineGridHolder.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lOfLevel[currentLevel].GridRange * cellSize);

        stoneGridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
    }

    //load block list
    private void LoadStoneBlock()
    {
        //init blockStoredIndex

        blockStoredIndex = new List<int>();
        currentIndex = 0;
        // instatiate stone block button
        for (int row = 0; row < lOfLevel[currentLevel].GridRange; row++)
        {
            for (int col = 0; col < lOfLevel[currentLevel].GridRange; col++)
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
                gridArray[localRows, localCols].GetComponent<Button>().onClick.RemoveAllListeners();
                gridArray[localRows, localCols].GetComponent<Button>().onClick.AddListener(() => OnClickedBlockAction?.Invoke(gridArray[localRows, localCols], false));
            }
        }

    }

    //check if the grid can put gem
    private bool CheckCanPut(StoneBlock block, PlaceGemTypeSO placeGem)
    {

        if (block.Row + placeGem.GemHeight > lOfLevel[currentLevel].GridRange)
        {
            return false;
        }
        if (block.Col + placeGem.GemWidth > lOfLevel[currentLevel].GridRange)
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

    public bool PlaceAGem(StoneBlock block, PlaceGemTypeSO placeGem)
    {
        //check if the block have gem
        if (CheckCanPut(block, placeGem) == false)
        {
            return false;
        }

        RectTransform blockPos = block.gameObject.GetComponent<RectTransform>();

        //set infor for gem then create it 
        // placeGem.InitGemInfo(blockPos, cellSize);
        GameObject newGem = gemPool.GetStoneBlockFromPool(placeGem, gemGridHolder, blockPos, cellSize);
        GemValue gemValue = newGem.GetComponent<GemValue>();

        gemValue.SetGemvalueType(placeGem.GemType, placeGem.IsDynamite);

        //load gem size to set status for block road
        for (int row = block.Row; row < block.Row + placeGem.GemHeight; row++)
        {
            for (int col = block.Col; col < block.Col + placeGem.GemWidth; col++)
            {
                gridArray[row, col].SetCanPutStatus(false);

                if (gemValue is not null)
                {
                    gemValue.InitGemLocation(gridArray[row, col].Id);
                }
                else
                {
                    Debug.LogError("Gem value is not available");
                }
            }
        }

        lAllGeminGrid.Add(gemValue);

        return true;
    }


    private StoneBlock GetBlockFromIndex(int index)
    {
        int row = index / lOfLevel[currentLevel].GridRange;
        int col = index % lOfLevel[currentLevel].GridRange;
        return gridArray[row, col];
    }

    //random place gem on board
    private bool PlaceRandomGemList(PlaceGemTypeSO placeGem)
    {
        if (placeGem is null) return false;

        List<int> availabeIndex = new List<int>();

        int totalBlocks = lOfLevel[currentLevel].GridRange * lOfLevel[currentLevel].GridRange;
        for (int i = 0; i < totalBlocks; i++)
        {
            availabeIndex.Add(i);
        }

        bool isPlaced = false;


        while (availabeIndex.Count > 0)
        {
            //Select random stone block 
            int randomIndex = UnityEngine.Random.Range(0, availabeIndex.Count);

            StoneBlock selectBLock = GetBlockFromIndex(randomIndex);

            isPlaced = PlaceAGem(selectBLock, placeGem);
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
        yield return new WaitForEndOfFrame();
        if (lOfLevel[currentLevel].LPlaceGem is null) yield break;

        if (wantAutoPlace is false) yield break;

        foreach (PlaceGemTypeSO gem in validGemList)
        {

            bool checkPlace = PlaceRandomGemList(gem);
            if (checkPlace == false)
            {
                attempToPlaceGem++;
                ResetGem();
                yield break;
            }
        }

        // auto place successful
        attempToPlaceGem = 0;
        if (canvasManager is not null)
        {
            //dont get dymanite as gem remove it from 2 list
            for (int i = validGemList.Count - 1; i >= 0; i--)
            {
                if (validGemList[i].IsDynamite)
                {
                    validGemList.Remove(validGemList[i]);
                }
            }

            canvasManager.LoadStoredGemPrefabs(validGemList);
        }
        else
        {
            Debug.Log("Canvas Manager cannot be null");
        }
        readyToPlace = true;
    }

    //place gem fuction
    public void PlaceALlGemOnList()
    {
        readyToPlace = false;
        if (coroutinePlaceGem is not null)
        {
            StopCoroutine(coroutinePlaceGem);
        }
        //reset list
        lAllGeminGrid = new List<GemValue>();
        //call place gem corroutine
        coroutinePlaceGem = StartCoroutine(IEPlaceAllGemOnList());
    }
    //check xem số lượng gem có phù hợp ko, nếu quá thì sẽ tự động xoá bớt
    private List<PlaceGemTypeSO> GetValidGemList()
    {
        bool isInvalid = true;
        int totalSize = 0;
        while (isInvalid)
        {
            //gem if gem size higher than grid range, if higher remove gem
            foreach (PlaceGemTypeSO gem in validGemList)
            {
                if (gem.GemWidth > lOfLevel[currentLevel].GridRange || gem.GemHeight > lOfLevel[currentLevel].GridRange)
                {
                    Debug.LogError($"The current gem List is = {validGemList.Count} invalid and should be re move one");
                    validGemList.Remove(gem);
                    Debug.LogError($"So new Number of gem in List = {validGemList.Count}");
                    break;
                }
                totalSize = totalSize + gem.GemWidth + gem.GemHeight - 1;
            }

            // check total size of gem compare to grid, if size > grid remove last
            if (totalSize > lOfLevel[currentLevel].GridRange * lOfLevel[currentLevel].GridRange)
            {
                totalSize = 0;
                Debug.LogError($"totalSize The current gem List is = {validGemList.Count} invalid and should be re move one");
                validGemList.RemoveAt(validGemList.Count - 1);
                Debug.LogError($"So new Number of gem in List = {validGemList.Count}");
                continue;
            }
            isInvalid = false;
        }


        return validGemList;
    }

    public void CheckIfBlockHasGem(StoneBlock block, bool IsDynamite = false)
    {
        if (readyToPlace is false) return;
        if (block.Health <= 0) return;
        if (block.Health > 0 && canvasManager.NumberOfPickaxe >= 0)
        {

            if (IsDynamite == false)
            {
                //reduce placaxe number
                bool mine = canvasManager.SetPickAxeNumber(-1);
                if (mine is false)
                {
                    return;
                }
            }
            bool mineAll = block.StartMining();
            Debug.Log(mineAll);
            if (mineAll == false)
            {
                return;
            }
        }

        if (block.CanPut == true) return;

        foreach (GemValue gem in lAllGeminGrid)
        {
            if (gem.LLocationsOfGem.Contains(block.Id))
            {
                if (gem.IsDynamite == true)
                {
                    DynamiteExpore(block);
                }
                bool isListEmpty = gem.CheckGemLocation(block.Id);
                if (isListEmpty == true)
                {
                    StartCoroutine(IfGemIsFound(gem));
                }
                break;
            }
        }
    }

    private IEnumerator IfGemIsFound(GemValue gem)
    {
        yield return new WaitForSeconds(1);
        gemPool.DeactiveGem(gem.gameObject);
        canvasManager.RevealHiddenGem(gem);

        lAllGeminGrid.Remove(gem);
        bool listEmpty = false;
        
        
        if (lAllGeminGrid.Count == 0)
        {
            listEmpty = true;
        }
        else
        {
            foreach (GemValue gemInGrid in lAllGeminGrid)
            {
                listEmpty = true; 
                if (gemInGrid.IsDynamite == false)
                {
                    listEmpty = false;
                    break;

                }
            }
        }

        if (listEmpty == true)
        {
            currentLevel++;
            canvasManager.OpenChestReward();
            ResetBoard();
        }
    }

    private void DynamiteExpore(StoneBlock block)
    {
        block.PlayExplored();
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int blockX = block.Row + dx;
                int blockY = block.Col + dy;

                if (blockX >= 0 && blockX < lOfLevel[currentLevel].GridRange && blockY >= 0 && blockY < lOfLevel[currentLevel].GridRange)
                {
                    CheckIfBlockHasGem(gridArray[blockX, blockY], true);
                }
            }
        }
    }
}



