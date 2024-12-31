using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemValue : MonoBehaviour
{
    private string gemValueType;
    private List<int> lLocationsOfGem = new List<int>();
    private bool isDynamite = false;
    public bool IsDynamite { get { return isDynamite; } }
    public List<int> LLocationsOfGem { get { return lLocationsOfGem; } }

    public string GemValueType { get { return gemValueType; } }

    public void SetGemvalueType(string type, bool isDynamiteSO)
    {
        gemValueType = type;
        if (isDynamiteSO == true)
        {
            isDynamite = true;
        }
    }
    public void ResetListLocation()
    {
        lLocationsOfGem = new List<int>();
        isDynamite = false;
    }
    public void InitGemLocation(int x)
    {
        lLocationsOfGem.Add(x);
    }

    public bool CheckGemLocation(int value)
    {
        lLocationsOfGem.Remove(value);
        if (lLocationsOfGem.Count == 0)
        {
            Debug.Log("Gem Revealed");
            return true;
        }
        return false;
    }
}
