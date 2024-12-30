using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBlockPool : MonoBehaviour
{
    private List<GameObject> lStoneBlockPool = new List<GameObject>();

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

}
