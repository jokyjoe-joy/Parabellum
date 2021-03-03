using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public abstract class UserInterface : MonoBehaviour
{
    public InventoryObject inventory;
    public Dictionary<GameObject, InventorySlot> itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
    protected Transform itemSlotContainer;
    protected Transform itemSlotTemplate;
    private bool isInventoryActive;
    private ShipController playerShipController;

    private void Awake()
    {
        itemSlotContainer = transform.Find("itemSlotContainer");
        // FIXME: This is not the way
        if (this.name == "Inventory") itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");
        // Deactivating inventory UI if active
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
        isInventoryActive = false;

        playerShipController = GameObject.FindGameObjectWithTag("PlayerTag").transform.parent.GetComponent<ShipController>();

    }

    private void Start() {
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            inventory.Container.Items[i].parent = this;
        }
        CreateSlots();
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    void Update()
    {
        if (Input.GetKeyDown("i")) {
            SwitchInventoryVisibleInvisible();
        }
        if (isInventoryActive)
        {
            UpdateSlots();
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
            UpdateSlots();
        }

        // Setting children active based on isInventoryActive
        foreach (Transform child in transform)
            child.gameObject.SetActive(isInventoryActive);
        // FIXME: this is not the way
        if (this.name == "Inventory")
        {
            foreach (Transform child in itemSlotContainer)
                child.gameObject.SetActive(isInventoryActive);
            itemSlotTemplate.gameObject.SetActive(false);

        }

        // Enable or disable player ship movement
        playerShipController.shouldCheckControls = !isInventoryActive;
    }

    public void UpdateSlots()
    {
        foreach (KeyValuePair<GameObject, InventorySlot> _slot in itemsDisplayed)
        {
            Text text = _slot.Key.transform.Find("amount").GetComponent<Text>();
            Image image = _slot.Key.transform.Find("image").GetComponent<Image>();

            // If has an item
            if (_slot.Value.ID >= 0)
            {
                image.sprite = inventory.database.GetItem[_slot.Value.item.Id].itemSprite;
                text.text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString();
                
                foreach (Transform child in _slot.Key.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            else
            {
                image.gameObject.SetActive(false);
                text.gameObject.SetActive(false);
            }
            
        }
    }
 
    public abstract void CreateSlots();

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }


    public void OnEnter(GameObject obj)
    {
        playerShipController.mouseItem.hoverObj = obj;
        if (itemsDisplayed.ContainsKey(obj))
            playerShipController.mouseItem.hoverItem = itemsDisplayed[obj];
    }
    public void OnExit(GameObject obj)
    {
        playerShipController.mouseItem.hoverObj = null;
        playerShipController.mouseItem.hoverItem = null;
    }

    public void OnEnterInterface(GameObject obj)
    {
        playerShipController.mouseItem.ui = obj.GetComponent<UserInterface>();
    }

    public void OnExitInterface(GameObject obj)
    {
        playerShipController.mouseItem.ui = null;
    }

    public void OnDragStart(GameObject obj)
    {
        var mouseObject = new GameObject();
        var rt = mouseObject.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        mouseObject.transform.SetParent(transform.parent);

        if (itemsDisplayed[obj].ID >= 0)
        {
            var img = mouseObject.AddComponent<Image>();
            img.sprite = inventory.database.GetItem[itemsDisplayed[obj].ID].itemSprite;
            img.raycastTarget = false;
        }
        playerShipController.mouseItem.obj = mouseObject;
        playerShipController.mouseItem.item = itemsDisplayed[obj];
    }
    public void OnDragEnd(GameObject obj)
    {
        var itemOnMouse = playerShipController.mouseItem;
        var mouseHoverItem = itemOnMouse.hoverItem;
        var mouseHoverObj = itemOnMouse.hoverObj;
        var GetItemObject = inventory.database.GetItem;

        if (itemOnMouse.ui != null)
        {
            if (mouseHoverObj)
                if (mouseHoverItem.CanPlaceInSlot(GetItemObject[itemsDisplayed[obj].ID]) && (mouseHoverItem.item.Id <= -1 || (mouseHoverItem.item.Id >= 0 && itemsDisplayed[obj].CanPlaceInSlot(GetItemObject[mouseHoverItem.item.Id]))))
                    inventory.MoveItem(itemsDisplayed[obj], mouseHoverItem.parent.itemsDisplayed[itemOnMouse.hoverObj]);
        }
        else
        {
            inventory.RemoveItem(itemsDisplayed[obj].item);
        }
        Destroy(itemOnMouse.obj);
        itemOnMouse.item = null;
    }
    public void OnDrag(GameObject obj)
    {
        if (playerShipController.mouseItem.obj != null)
        {
            playerShipController.mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }
}

public class MouseItem
{   
    public UserInterface ui;
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverObj;
}