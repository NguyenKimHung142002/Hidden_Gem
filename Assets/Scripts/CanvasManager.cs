using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{

    [SerializeField] private Transform storedGemParent;
    [SerializeField] private Transform chestRewardsParent;
    [SerializeField] private GameObject storedGemPrefab;
    
    [SerializeField] private int numberOfPickaxe = 100;
    [SerializeField] private TextMeshProUGUI numberOfPickaxeTextObject;
    [SerializeField] private GameObject addPickAxePopUp;
    [SerializeField] private List<ChestValue> lChestRewards = new List<ChestValue>();
    public int NumberOfPickaxe { get { return numberOfPickaxe; } }
    private List<StoredGemValue> lStoredGemValue;

    private void Start()
    {
        SetPickAxeNumber();
        if (chestRewardsParent is not null)
        {

            foreach(Transform child in chestRewardsParent)
            {
  
                ChestValue chest = child.gameObject.GetComponent<ChestValue>();
                if (chest is not null)
                {

                    lChestRewards.Add(chest);
                }
            }
        }
    }

    public void OpenChestReward()
    {
        foreach (ChestValue chest in lChestRewards)
        {
            if (chest.IsOpen == false)
            {
                chest.SetOpenChest();
                break;
            }
        }
    }
    // Start is called before the first frame update
    public void LoadStoredGemPrefabs(List<PlaceGemTypeSO> validGemList)
    {
        lStoredGemValue = new List<StoredGemValue>();

        if (validGemList is null) return;
        for (int i = 0; i < validGemList.Count; i++)
        {
            GameObject newStoredGem = Instantiate(storedGemPrefab, storedGemParent);
            StoredGemValue newStoredGemValue = newStoredGem.GetComponent<StoredGemValue>();
            if (newStoredGemValue is not null)
            {
                newStoredGemValue.AssignStoredGemValue(validGemList[i]);
            }

            lStoredGemValue.Add(newStoredGemValue);
        }
    }
    public void SetPickAxeNumber(int number = 0)
    {
        numberOfPickaxe += number;
        if (numberOfPickaxe < 0 )
        {
            numberOfPickaxe = 0;
            addPickAxePopUp.SetActive(true);
        }
        numberOfPickaxeTextObject.text = numberOfPickaxe.ToString();

    }
    public void RevealHiddenGem(GemValue gem)
    {
        foreach (StoredGemValue storedGem in lStoredGemValue)
        {
            if (storedGem.StoredGemType == gem.GemValueType && storedGem.IsRevealed == false)
            {
                lStoredGemValue.Remove(storedGem);
                storedGem.RevealStoredGem();
                break;
            }
        }
    }

    public void ResetStoredGem()
    {
        foreach (Transform child in storedGemParent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
