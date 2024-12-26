using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RectTransform))]
public class StoneBlock : MonoBehaviour
{
    private int x;
    private int y;
    [SerializeField] private TextMeshProUGUI blockText;
    [SerializeField] private GameObject texture;
    [SerializeField] private bool isActive = true;
    private bool canPut = true;

    public StoneBlock (int x, int y)
    {
        this.x = x;
        this.y = y;
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
    public void UpdateDebugText(string text)
    {

    }
}
