using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemValue : MonoBehaviour
{
    private List<int> lLocationsOfGem  = new List<int>();
    public List<int> LLocationsOfGem {get {return lLocationsOfGem;}}

    
    public void InitGemLocation(int x)
    {
        lLocationsOfGem.Add(x);
    }

    public bool RemoveGemLocation(int value)
    {
        lLocationsOfGem.Remove(value);
        if(lLocationsOfGem.Count == 0)
        {
            Debug.Log("Gem Revealed");
            Destroy(gameObject, 1);
            return true;
        }
        return false;    
    }
}
