using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ChestRewards", menuName = "ScriptableObjects/ChestRewards")]
public class ChestRewardSO : ScriptableObject
{
    [SerializeField] private string chestType;
    [SerializeField] private Sprite closeChest;
    [SerializeField] private Sprite openChest;
    [SerializeField] private int pickAxeReward = 50;


    public Sprite CloseChest {get {return closeChest;}}
    public Sprite OpenChest {get {return openChest;}}
    public int PickAxeReward {get {return pickAxeReward;}}

}
