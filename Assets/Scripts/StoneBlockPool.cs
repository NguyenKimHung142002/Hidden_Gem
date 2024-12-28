using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBlockPool : MonoBehaviour
{
    [SerializeField] private Transform BlockHolderParent;
    private List<GameObject> lStoneBlockPool;

        private void Awake()
    {
        lStoneBlockPool = new List<GameObject>();
    }

    public void SetActiveBlock()
    {

    }

    public GameObject GetStoneBlockFromPool(GameObject prefab, RectTransform parent)
    {
        if (lStoneBlockPool.Count > 0 )
        {
            foreach (GameObject block in lStoneBlockPool)
            {
                if (block.activeSelf == false)
                {
                    block.SetActive(true);
                    return block;
                }
            }
        }
        GameObject newBlock = Instantiate(prefab, parent);
        lStoneBlockPool.Add(newBlock);
        return newBlock;
    }

    public void DebugLog(GameObject a)
    {
        Debug.Log("Everything is ok" + a);
    }



}
