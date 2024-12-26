using System.Collections;
using System.Collections.Generic;
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
    [SerializeField, Range(1, 4)]
    private int gemWidth;
    [SerializeField, Range(1, 4)]
    private int gemHeight;
    private float gemSize = 100;
    private RectTransform gemPosition;
    public float GemSize{get; set;}

    public void InitGemInfo ( float gemSize)
    {
        SetSprite();
        RectTransform gemRect = gemPrefabs.GetComponent<RectTransform>();

        if (gemRect is null) return;
       
        // gemRect.pivot = new Vector2(0, 1);

        gemRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, gemWidth * gemSize);
        gemRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, gemHeight * gemSize);

        /// gemRect.anchoredPosition = gemPos.anchoredPosition;
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
    
}