using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class StoneBlock : MonoBehaviour
{
    private int id;
    private int row;
    private int col;
    private TextMeshProUGUI blockText;
    [SerializeField] private List<Sprite> lBlockTexture;

    private int health;
    private bool canPut = true;
    private Image blockImage;
    public bool CanPut { get { return canPut; } }
    public int Row { get { return row; } }
    public int Col { get { return col; } }
    public int Id { get { return id; } set { id = value; } }
    public int Health { get { return health; } }
    public void SetXY(int w, int h)
    {
        this.row = w;
        this.col = h;
    }
    //check if block have gem
    public void SetCanPutStatus(bool status)
    {
        canPut = status;
        // if (canPut is false)
        // {
        //     SetBlockText("False");
        // }
    }
    private void Awake()
    {
        health = lBlockTexture.Count;
        blockText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        blockImage = gameObject.GetComponent<Image>();
        if (blockText is null)
        {
            Debug.LogError("Block Text missing");
        }

        RectTransform rectT = gameObject.GetComponent<RectTransform>();
        rectT.pivot = new Vector2(0, 1);
    }

    public void SetBlockText(string text)
    {
        if (blockText is null) return;
        blockText.text = text;
    }
    private void ChangeBlockTexture()
    {
        if (blockImage.gameObject.activeSelf == false)
            blockImage.enabled = true;
        blockImage.sprite = lBlockTexture[health - 1];
    }
    public bool StartMining()
    {
        health--;
        if (health == 0)
        {
            if (blockImage.gameObject.activeSelf == true)
                blockImage.enabled = false;
            return false;
        }
        else if (health > 0)
        {      
            ChangeBlockTexture();
        }
        return true;
    }
    public void ResetComponent()
    {
        row = 0;
        col = 0;
        id = 0;
        canPut = true;
        SetCanPutStatus( true);
        health = lBlockTexture.Count;
        ChangeBlockTexture();
    }

}
