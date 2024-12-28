using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GemTypes", menuName = "ScriptableObjects/GemTypes")]
public class PlaceGemTypeSO : ScriptableObject
{
    public string gemType;
    [SerializeField]
    private GameObject gemPrefabs;
    [SerializeField]
    private Sprite gemSprite;
    [SerializeField, Range(1, 5)]
    private int gemWidth;
    [SerializeField, Range(1, 5)]
    private int gemHeight;
    private RectTransform gemPosition;
    private GameObject gemGameObject;
    private List<int> lLocationsOfGem ;
    public List<int> LLocationsOfGem {get {return lLocationsOfGem;}}

    public int GemWidth { get { return gemWidth; } }
    public int GemHeight { get { return gemHeight; } }
    public void InitGemInfo(RectTransform blockPos, float gemSize)
    {
        SetSprite();
        RectTransform gemRect = gemPrefabs.GetComponent<RectTransform>();

        if (gemRect is null) return;

        gemRect.pivot = new Vector2(0, 1);

        gemRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gemWidth * gemSize);
        gemRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, gemHeight * gemSize);

        gemRect.anchoredPosition = blockPos.anchoredPosition;   

        lLocationsOfGem = new List<int>();
    }

    public void SetSprite()
    {
        Image gemImage = gemPrefabs.GetComponent<Image>();
        if (gemImage is null) return;

        gemImage.sprite = gemSprite;
    }

    public GameObject GetGemPrefabs()
    {
        return gemPrefabs;
    }

    public void InitGemLocation(int x)
    {
        lLocationsOfGem.Add(x);
    }

    public void RemoveGemLocation(int value)
    {
        Debug.LogError("Part of gem found at " + value);
        lLocationsOfGem.Remove(value);
        if(lLocationsOfGem.Count == 0)
        {
            Debug.Log("Gem Revealed");
            gemGameObject.SetActive(false);
        }
            
    }

    public void CreateGemGameObject(RectTransform parent)
    {
        gemGameObject  = Instantiate(gemPrefabs, parent);

    }
}
