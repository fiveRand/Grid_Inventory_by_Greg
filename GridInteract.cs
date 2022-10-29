using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    ItemGrid grid;
    InventoryController controller;

    private void Awake() {
        controller = FindObjectOfType<InventoryController>();
        grid = GetComponent<ItemGrid>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        controller.selectedGrid = grid;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        controller.selectedGrid = null;
    }
}
