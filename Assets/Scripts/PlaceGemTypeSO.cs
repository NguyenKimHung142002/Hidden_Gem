using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GemTypes", menuName = "ScriptableObjects/GemTypes")]
public class PlaceGemTypeSO : ScriptableObject
{
    [SerializeField] 
    private string gemType;
    [SerializeField]
    private GameObject gemPrefabs;

    [SerializeField]
    private Sprite gemSprite;
    [SerializeField, Range(1, 5)]
    private int gemWidth;
    [SerializeField, Range(1, 5)]
    private int gemHeight;

    public string GemType { get { return gemType; } }
    public int GemWidth { get { return gemWidth; } }
    public int GemHeight { get { return gemHeight; } }
    public GameObject GemPrefab { get { return gemPrefabs; } }
    public Sprite GemSprite { get { return gemSprite; } }
    public GameObject GetGemPrefabs()
    {
        return gemPrefabs;
    }

    public GameObject CreateGemGameObject(RectTransform parent)
    {
        return Instantiate(gemPrefabs, parent);

    }
}
