using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChestValue : MonoBehaviour
{
    [SerializeField] private ChestRewardSO chestRewardSO;

    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool isClaimed = false;
    [SerializeField] private GameObject chestImageObject;
    [SerializeField] private ParticleSystem chestParticle;
    
    private Image chestImage;
    public bool IsOpen { get { return isOpen; } }
    private CanvasManager canvasManager;
    private void Awake()
    {
        canvasManager = GameObject.FindObjectOfType<CanvasManager>();
        chestImage = chestImageObject.GetComponent<Image>();
        chestParticle.Stop();
        if (isOpen == false)
        {
            chestImage.sprite = chestRewardSO.CloseChest;
        }
        else
        {
            chestImage.sprite = chestRewardSO.OpenChest;
        }
    }
    public void ClaimReward()
    {
        if (isOpen && isClaimed == false && canvasManager)
        {
            canvasManager.SetPickAxeNumber(chestRewardSO.PickAxeReward);
            isClaimed = true;
            chestParticle.Stop();

        }
    }

    public void SetOpenChest()
    {
        isOpen = true;
        chestParticle.Play();
        chestImage.sprite = chestRewardSO.OpenChest;
    }
}
