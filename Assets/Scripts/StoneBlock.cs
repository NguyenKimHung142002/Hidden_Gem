using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RectTransform))]
public class StoneBlock : MonoBehaviour
{
    private int id;
    private int row;
    private int col;
    [SerializeField] private TextMeshProUGUI blockText;
    [SerializeField] private GameObject texture;
    [SerializeField] private bool isActive = true;
    private bool canPut = true;
    public bool CanPut {get {return canPut;}}
    public int Row {get {return row;}}
    public int Col {get{return col;}}
    public int Id {get{return id;} set{id = value;}}
    public void SetXY (int w, int h)
    {
        this.row = w;
        this.col = h;
    }
    //check if block have gem
    public void SetCanPutStatus (bool status)
    {
        canPut = status;
        if (canPut is false)
        {
            SetBlockText ("False");
        }
    }
    private void Awake()
    {
        blockText = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        if (blockText is null)
        {
            Debug.LogError("Block Text missing");
        }

        RectTransform rectT = gameObject.GetComponent<RectTransform>();
        rectT.pivot = new Vector2(0,1);
    }

    public void SetBlockText(string text)
    {
        if (blockText is null) return;
        blockText.text = text;
    }

    public void ResetComponent()
    {
        row = 0;
        col = 0;
        id = 0;
        isActive = true;
        canPut = true;
    }

}
