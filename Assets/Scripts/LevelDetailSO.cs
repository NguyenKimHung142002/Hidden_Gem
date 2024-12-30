using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDetails", menuName = "ScriptableObjects/LevelDetails")]
public class LevelDetailSO : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField, Range(2, 8)] private int gridRange;
    [SerializeField] private List<PlaceGemTypeSO> lPlaceGem;
    [SerializeField] private int pickAxeReward = 50;

    [SerializeField] private int maxCellsize = 200;
    [SerializeField] private int minCellsize = 100;
    public int GridRange { get { return gridRange; } }
    public int MaxCellsize { get { return maxCellsize; } }
    public int MinCellsize { get { return minCellsize; } }
    public List<PlaceGemTypeSO> LPlaceGem { get { return lPlaceGem; } }

    private void Awake()
    {
        if (maxCellsize < 0) maxCellsize = 0;

        if (minCellsize < 0) minCellsize = 0;

        if (minCellsize > maxCellsize) minCellsize = maxCellsize;
    }

}
