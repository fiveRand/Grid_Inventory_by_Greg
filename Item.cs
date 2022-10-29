using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Item : MonoBehaviour
{
    public ItemData data;
    public int GridPosX, GridPosY;

    public int width
    {
        get
        {
            return (!rotated) ? data.x : data.y;
        }
    }

    public int height
    {
        get
        {
            return (!rotated) ? data.y : data.x;
        }
    }
    public RectTransform rectTransform;
    public bool rotated = false;
    Image image;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponentInChildren<Image>();

        image.raycastTarget = false;
        image.maskable = false;
        Set(data);
    }

    internal void Set(ItemData data)
    {
        image.sprite = data.sprite;

        float x = data.x * ItemGrid.tileSize.x;
        float y = data.y * ItemGrid.tileSize.y;
        rectTransform.sizeDelta = new Vector2(x, y);
    }

    internal void Rotate()
    {
        rotated = !rotated;
        float angle = (rotated == true) ? 90f : 0f;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
