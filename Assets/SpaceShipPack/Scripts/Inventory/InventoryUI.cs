using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public InventoryObject inventory;
    public int spaceBetweenItemsX;
    public int NUMBER_OF_COLUMNS;
    public int spaceBetweenItemsY;
    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;

    private void Awake() {
        itemSlotContainer = transform.Find("itemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay() {
        int x = 0;
        int y = 0;
        // TODO: Can i use foreach here?
        for (int i = 0; i < inventory.inventoryContainer.Count; i++) {
            // Activate and position itemslot.
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);
            itemSlotRectTransform.anchoredPosition = new Vector2(x * spaceBetweenItemsX, y * spaceBetweenItemsY);
            // 
            Image itemSlotSprite = itemSlotRectTransform.transform.Find("image").GetComponent<Image>();
            itemSlotSprite.sprite = inventory.inventoryContainer[i].item.itemSprite;
            Text itemSlotName = itemSlotRectTransform.transform.Find("name").GetComponent<Text>();
            itemSlotName.text = inventory.inventoryContainer[i].item.itemName;
            Text itemSlotAmount = itemSlotRectTransform.transform.Find("amount").GetComponent<Text>();
            itemSlotAmount.text = inventory.inventoryContainer[i].amount.ToString();
            x++;
        }
        if (x > 4) {
            x = 0;
            y++;
        }
    }
}
