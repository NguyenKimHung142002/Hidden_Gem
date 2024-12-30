using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoredGemValue : MonoBehaviour
{
    [SerializeField] private Image hiddenGem;
    [SerializeField] private Image revealGem;
    private string gemType;
    private bool isRevealed = false;

    public string StoredGemType {get { return gemType;}}
    public bool IsRevealed {get { return isRevealed;}}
    public void AssignStoredGemValue(PlaceGemTypeSO gemTypeSO)
    {
        gemType = gemTypeSO.GemType;
        hiddenGem.sprite = gemTypeSO.GemSprite;
        revealGem.sprite = gemTypeSO.GemSprite;
    }

    public void RevealStoredGem()
    {
        isRevealed = true;
        revealGem.gameObject.SetActive(isRevealed);
    }
}
