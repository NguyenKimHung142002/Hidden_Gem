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
    //[SerializeField, Range(2, 8)] private int gridRange;

    [SerializeField] private List<LevelDetailSO> lOfLevel;
    [SerializeField] private RectTransform mineGridHolder;
    [SerializeField] private RectTransform stoneGridHolder;
    [SerializeField] private RectTransform gemGridHolder;
    [SerializeField] private GameObject stoneBlockPrefab;
    //[SerializeField] private List<PlaceGemTypeSO> lPlaceGem;
    [SerializeField] private bool wantAutoPlace = true;
    public UnityEvent<StoneBlock> OnClickedBlockAction;
    private int maxWidth = 8;
    private int minWidth = 2;
    private int attempToPlaceGem = 0;
    private StoneBlockPool stoneBlockPool;
    private float cellSize;
    private StoneBlock[,] gridArray;
    private List<int> blockStoredIndex;
    private List<GemValue> lAllGeminGrid;
    private int currentIndex = 0;
    private int currentLevel = 0;
    private Coroutine coroutinePlaceGem;

    private void Awake()
    {
        stoneBlockPool = gameObject.GetComponent<StoneBlockPool>();
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
                gridArray[localRows, localCols].GetComponent<Button>().onClick.AddListener(() => OnClickedBlockAction?.Invoke(gridArray[localRows, localCols]));
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
        GameObject newGem = placeGem.CreateGemGameObject(gemGridHolder);
        GemValue gemValue = newGem.GetComponent<GemValue>();
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
    private bool PlaceRandomGem(PlaceGemTypeSO placeGem)
    {
        if (placeGem is null) return false;

        List<int> availabeIndex = new List<int>();

        int totalBlocks = lOfLevel[currentLevel].GridRange * lOfLevel[currentLevel].GridRange;
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
        if (lOfLevel[currentLevel].LPlaceGem is null) yield break;
        if (CheckIfGemListIsValid() is false)
        {
            yield break;
        }
        if (wantAutoPlace is false) yield break;
        foreach (PlaceGemTypeSO gem in lOfLevel[currentLevel].LPlaceGem)
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
        //reset list
        lAllGeminGrid = new List<GemValue>();
        //call place gem corroutine
        coroutinePlaceGem = StartCoroutine(IEPlaceAllGemOnList());
    }

    private bool CheckIfGemListIsValid()
    {
        int totalSize = 0;
        foreach (PlaceGemTypeSO gem in lOfLevel[currentLevel].LPlaceGem)
        {
            if (gem.GemWidth > lOfLevel[currentLevel].GridRange || gem.GemHeight > lOfLevel[currentLevel].GridRange)
            {
                Debug.LogError($"Gem width = {gem.GemWidth}, Gem Heigh = {gem.GemHeight} Grid Range = {lOfLevel[currentLevel].GridRange}");
                return false;
            }
            totalSize = totalSize + gem.GemWidth + gem.GemHeight - 1;
        }

        if (totalSize > lOfLevel[currentLevel].GridRange * lOfLevel[currentLevel].GridRange)

        {
            Debug.LogError("Total size: " + totalSize);
            return false;
        }
        return true;
    }

    public void CheckAllGemLocation(StoneBlock block)
    {
        if(block.StartMining()) return;
        foreach (GemValue gem in lAllGeminGrid)
        {
            if (gem.LLocationsOfGem.Contains(block.Id))
            {
                bool isListEmpty = gem.RemoveGemLocation(block.Id);
                if (isListEmpty == true)
                {
                    lAllGeminGrid.Remove(gem);
                }
                break;
            }
        }
    }
}



