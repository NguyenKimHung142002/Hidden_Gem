using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemPool : MonoBehaviour
{
    private List<GameObject> lgemPool = new List<GameObject>();

    public GameObject GetStoneBlockFromPool(PlaceGemTypeSO gemInfo, RectTransform parent, RectTransform pos, float cellsize)
    {
        if (lgemPool.Count > 0)
        {
            foreach (GameObject gem in lgemPool)
            {
                gem.SetActive(true);
                lgemPool.Remove(gem);
                InitGemInfo(gem, gemInfo, pos, cellsize);
                ResetGemLocation(gem);
                return gem;
            }
        }
        GameObject newGem = Instantiate(gemInfo.GemPrefab, parent);
        InitGemInfo(newGem, gemInfo, pos, cellsize);
        ResetGemLocation(newGem);
        return newGem;
    }
    public void ResetGemLocation(GameObject gemObject)
    {
        GemValue gemValue = gemObject.GetComponent<GemValue>();
        if (gemValue is not null)
        {
            gemValue.ResetListLocation();
        }
    }
    public void DeactiveGem(GameObject gem)
    {
        ResetGemLocation(gem);
        gem.SetActive(false);
        lgemPool.Add(gem);
    }


    public void InitGemInfo(GameObject prefab, PlaceGemTypeSO gemInfo, RectTransform blockPos, float gemSize)
    {
        SetSprite(prefab, gemInfo);
        RectTransform gemRect = prefab.GetComponent<RectTransform>();

        gemRect.pivot = new Vector2(0, 1);

        gemRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gemInfo.GemWidth * gemSize);
        gemRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, gemInfo.GemHeight * gemSize);

        gemRect.anchoredPosition = blockPos.anchoredPosition;

    }

    public void SetSprite(GameObject prefab, PlaceGemTypeSO gemInfo)
    {
        Image gemImage = prefab.GetComponent<Image>();
        if (gemImage is null) return;

        gemImage.sprite = gemInfo.GemSprite;
    }


}
