using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGrid : MonoBehaviour
{
    RectTransform canvasRectTransform;
    Canvas canvas;
    CanvasScaler canvasScaler;
    public static Vector2 tileSize = new Vector2(32, 32);
    public static Vector2 interactTileSize;

    [SerializeField] Vector2Int gridSize;
    Item[,] itemSlots;

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();

    [SerializeField]RectTransform rectTransform;
    private void Awake() {
        
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        canvasScaler = GetComponentInParent<CanvasScaler>();
        rectTransform = GetComponent<RectTransform>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        interactTileSize = new Vector2(tileSize.x * canvasRectTransform.localScale.x, tileSize.y * canvasRectTransform.localScale.y);

        Initialize(gridSize.x, gridSize.y);
    }

    private void OnValidate() {
        if(canvasScaler == null)
        {
            canvasScaler = GetComponentInParent<CanvasScaler>();
        }
        if(canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            
        }
        CanvasCheck();
    }

    public Vector2Int? FindSpace(Item item)
    {

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x;x++)
            {
                if(isInBoundary(x,y,item.width,item.height) && isValid(x,y,item.width,item.height))
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }

    void CanvasCheck()
    {
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            Debug.LogError($"Set {this.name}.Canvas renderMode to ScreenSpaceOverlay.");
        }
        if(canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            Debug.LogError($"Set {this.name}.Canvas ScaleMode to ScaleWithScreenSize, also set Match to 0.5 ");
            Debug.LogError($"also, set Match to 0.5 and Reference PPU to 100 ");
        }
    }

    void Initialize(int width,int height)
    {
        itemSlots = new Item[width, height];

        Vector2 size = new Vector2(width * tileSize.x, height * tileSize.y);
        rectTransform.sizeDelta = size;
    }

    public Item PickUp(int x,int y)
    {
        if(!isInBoundary(x,y))
        {
            return null;
        }

        var item = itemSlots[x, y];


        if(item == null)
        {
            return null;
        }

        Clear(item);

        return item;
    }

    void Clear(Item item)
    {
        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                int iX = x + item.GridPosX;
                int iY = y + item.GridPosY;

                itemSlots[iX, iY] = null;
            }
        }
    }

    public Vector2Int GetGridPosition(Vector2 mousePosition)
    {
        

        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

        tileGridPosition.x = (int)((positionOnTheGrid.x) / interactTileSize.x) ;
        tileGridPosition.y = (int)((positionOnTheGrid.y) / interactTileSize.y);


        return tileGridPosition;
    }

    public Item GetItem(int x,int y)
    {
        return itemSlots[x, y];
    }

    public bool PlaceItem(Item item,int x,int y,ref Item overlapItem)
    {
        if(isInBoundary(x,y,item.width,item.height) == false)
        {
            return false;
        }

        if(OverlapCheck(x,y,item.width,item.height,ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if(overlapItem != null)
        {
            Clear(overlapItem);
        }

        PlaceItem(item, x, y);
        return true;
    }

    public void PlaceItem(Item item,int x,int y)
    {
        item.rectTransform.SetParent(this.rectTransform);
        item.rectTransform.anchorMax = new Vector2(0, 1);
        item.rectTransform.anchorMin = new Vector2(0, 1);
        item.rectTransform.pivot = Vector2.one * 0.5f;

        for (int itemX = 0; itemX < item.width; itemX++)
        {
            for (int itemY = 0; itemY < item.height; itemY++)
            {
                itemSlots[x + itemX, y + itemY] = item;
            }
        }
        item.GridPosX = x;
        item.GridPosY = y;
        Vector2 position = CalculaterPositionOnGrid(item, x, y);
        item.rectTransform.localPosition = position;
    }
    public Vector2 CalculaterPositionOnGrid(Item item,int x,int y)
    {
        float posX = x * tileSize.x + tileSize.x * item.width * 0.5f;
        float posY = y * tileSize.y + tileSize.y * item.height * 0.5f;

        return new Vector2(posX, -posY);
    }

    bool OverlapCheck(int posX,int posY,int width,int height,ref Item overLapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int iX = posX + x; int iY = posY + y;
                if(itemSlots[iX,iY] != null)
                {
                    if(overLapItem == null)
                    {
                        overLapItem = itemSlots[iX, iY];
                    }
                    else
                    {
                        if(overLapItem != itemSlots[iX,iY])
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    public bool isInBoundary(int x,int y)
    {
        return (x < 0 || y < 0 || x >= gridSize.x || y >= gridSize.y) == false;
    }

    bool isValid(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int iX = posX + x; int iY = posY + y;
                if (itemSlots[iX, iY] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool isInBoundary(int x,int y,int width,int height)
    {
        if(!isInBoundary(x,y))
        {
            return false;
        }
        x += width -1;
        y += height - 1;
        if (!isInBoundary(x, y))
        {
            return false;
        }
        return true;
    }
}
