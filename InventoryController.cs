using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public Item[] itemList;
    ItemGrid selectedGrid_;
    public ItemGrid selectedGrid
    {
        get => selectedGrid_;
        set
        {
            selectedGrid_ = value;
            highlight.SetParent(selectedGrid);
        }
    }
    [SerializeField] RectTransform rectTransform;
    [SerializeField]Item selectedItem;
    Item overlapItem;
    [SerializeField]InventoryHighlight highlight;
    Item item2Highlight;


    void PlaceItem(Vector2Int tileGridPos)
    {

        if(selectedGrid.PlaceItem(selectedItem, tileGridPos.x, tileGridPos.y,ref overlapItem))
        {
            selectedItem = null;
            if(overlapItem != null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
            }
        }
    }

    Item GetRandomItem()
    {
        int randomIndex = Random.Range(0, itemList.Length);
        var item = Instantiate(itemList[randomIndex], transform.position, Quaternion.identity,selectedGrid.transform);
        return item;
    }

    

    void Insert(Item item)
    {
        if(selectedGrid == null)
        {
            return;
        }

        Vector2Int? pos = selectedGrid.FindSpace(item);
        if(pos == null)
        {
            return;
        }
        
        selectedGrid.PlaceItem(item, pos.Value.x, pos.Value.y);
    }

    void PickUpItem(Vector2Int tileGridPos)
    {
        selectedItem = selectedGrid.PickUp(tileGridPos.x, tileGridPos.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }

    }

    void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPos();
        if(selectedItem == null)
        {
            item2Highlight = selectedGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
            if(item2Highlight != null)
            {
                highlight.gameObject.SetActive(true);
                highlight.SetSize(item2Highlight);
                highlight.SetPosition(selectedGrid, item2Highlight);
            }
            else
            {
                highlight.gameObject.SetActive(false);
            }
        }
        else
        {
            bool isInBoundary = selectedGrid.isInBoundary(positionOnGrid.x, positionOnGrid.y, selectedItem.width, selectedItem.height);
            highlight.gameObject.SetActive(isInBoundary);
            highlight.SetSize(selectedItem);
            highlight.SetPosition(selectedGrid, selectedItem,positionOnGrid.x,positionOnGrid.y);
        }
    }
    private void Update() 
    {
        Vector2 mousePos = Input.mousePosition;

        if (selectedGrid == null)
        {
            highlight.gameObject.SetActive(false);
            return;
        }

        if (selectedItem != null)
        {
            rectTransform.position = mousePos;
            rectTransform.SetParent(selectedGrid.transform);
        }

        HandleHighlight();
        if(Input.GetMouseButtonDown(0))
        {
            var tilePos = GetTileGridPos();
            if(selectedItem == null)
            {
                PickUpItem(tilePos);
            }
            else
            {
                PlaceItem(tilePos);
            }
        }
        if(Input.GetKeyDown(KeyCode.Q) && selectedItem == null)
        {
            selectedItem = GetRandomItem();
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            var item = GetRandomItem();
            Insert(item);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }
    }

    void RotateItem()
    {
        if(selectedItem == null)
        {
            return;
        }
        selectedItem.Rotate();
    }


    Vector2Int GetTileGridPos()
    {
        Vector2 mousePos = Input.mousePosition;
        if (selectedItem != null)
        {
            mousePos.x -= (selectedItem.width - 1) * ItemGrid.interactTileSize.x * 0.5f;
            mousePos.y += (selectedItem.height - 1) * ItemGrid.interactTileSize.y * 0.5f;
        }
        Vector2Int tilePos = selectedGrid.GetGridPosition(mousePos);
        return tilePos;
    }
}
