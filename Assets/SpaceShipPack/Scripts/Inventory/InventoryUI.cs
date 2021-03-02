using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public InventoryObject inventory;
    public int spaceBetweenItemsX;
    public int spaceBetweenItemsY;
    Dictionary<InventorySlot, RectTransform> itemsDisplayed = new Dictionary<InventorySlot, RectTransform>();
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;
    private bool isInventoryActive;
    private ShipController playerShipController;

    private void Awake()
    {
        itemSlotContainer = transform.Find("itemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");
        // Deactivating inventory UI if active
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
        isInventoryActive = false;

        playerShipController = GameObject.FindGameObjectWithTag("PlayerTag").transform.parent.GetComponent<ShipController>();

    }

    void Update()
    {
        if (Input.GetKeyDown("i")) {
            SwitchInventoryVisibleInvisible();
            UpdateInventoryDisplay();
        }
    }

    public void SwitchInventoryVisibleInvisible()
    {
        if (isInventoryActive)
        {
            // Hide cursor & inventory
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isInventoryActive = false;
        } 
        else 
        {
            // Show cursor & inventory
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            isInventoryActive = true;
        }

        // Setting children active based on isInventoryActive
        foreach (Transform child in transform)
            child.gameObject.SetActive(isInventoryActive);
        

        // Enable or disable player ship movement
        playerShipController.shouldCheckControls = !isInventoryActive;


    }
 
    public void UpdateInventoryDisplay()
    {
        // Init grid values (starting in right upper corner)
        int x = 0;
        int y = 0;
        // TODO: Set max items in inventory and display a scrollbar
        // if we have more than what can be in the UI at once

        // Loop through inventory and check if we already have the item's icon instantiated.
        // If we do, only change the item's name, amount, etc..., then increment the grid's x value.
        // If we don't, instantiate it.
        for (int i = 0; i < inventory.Container.Items.Count; i++)
        {
            if (itemsDisplayed.ContainsKey(inventory.Container.Items[i]))
            {
                // If we already have the item, update its name, image and amount
                RectTransform itemSlotRectTransform = itemsDisplayed[inventory.Container.Items[i]];
                Image itemSlotSprite = itemSlotRectTransform.transform.Find("image").GetComponent<Image>();
                itemSlotSprite.sprite = inventory.Container.Items[i].item.sprite;
                Text itemSlotName = itemSlotRectTransform.transform.Find("name").GetComponent<Text>();
                itemSlotName.text = inventory.Container.Items[i].item.Name;
                Text itemSlotAmount = itemSlotRectTransform.transform.Find("amount").GetComponent<Text>();
                itemSlotAmount.text = inventory.Container.Items[i].amount.ToString();
                x++;
            } 
            else
            {
                // If we don't have this item instantiated in UI, then instantiate it, set it active and position it
                RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
                itemSlotRectTransform.gameObject.SetActive(true);
                itemSlotRectTransform.anchoredPosition = new Vector2(x * spaceBetweenItemsX, y * spaceBetweenItemsY);

                Image itemSlotSprite = itemSlotRectTransform.transform.Find("image").GetComponent<Image>();
                itemSlotSprite.sprite = inventory.Container.Items[i].item.sprite;
                Text itemSlotName = itemSlotRectTransform.transform.Find("name").GetComponent<Text>();
                itemSlotName.text = inventory.Container.Items[i].item.Name;
                Text itemSlotAmount = itemSlotRectTransform.transform.Find("amount").GetComponent<Text>();
                itemSlotAmount.text = inventory.Container.Items[i].amount.ToString();
                
                itemsDisplayed.Add(inventory.Container.Items[i], itemSlotRectTransform);
                x++;
            }
            if (x > 3)
            {
                x = 0;
                y--; // Decrementing, because we are starting in the right upper corner
            }
        }
    }
}
