using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHighlight : MonoBehaviour
{
    public RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetSize(Item item)
    {
        float x = item.width * ItemGrid.tileSize.x;
        float y = item.height * ItemGrid.tileSize.y;
        rectTransform.sizeDelta = new Vector2(x, y);
    }

    public void SetParent(ItemGrid grid)
    {
        if(grid == null)
            return;
        rectTransform.SetParent(grid.transform);
    }
    public void SetPosition(ItemGrid targetGrid,Item item)
    {
        rectTransform.SetParent(targetGrid.transform);
        Vector2 pos = targetGrid.CalculaterPositionOnGrid(item, item.GridPosX, item.GridPosY);
        rectTransform.localPosition = pos;
    }
    public void SetPosition(ItemGrid targetGrid, Item item,int x,int y)
    {
        Vector2 pos = targetGrid.CalculaterPositionOnGrid(item, x, y);
        rectTransform.localPosition = pos;
    }
    
}
