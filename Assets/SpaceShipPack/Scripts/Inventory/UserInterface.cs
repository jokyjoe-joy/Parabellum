using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public abstract class UserInterface : MonoBehaviour
{
    public InventoryObject inventory;
    public Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
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
        foreach (Transform child in transform) 
        {
            child.gameObject.SetActive(false);
        }
        isInventoryActive = false;
        playerShipController = GameObject.FindGameObjectWithTag("PlayerTag").transform.parent.GetComponent<ShipController>();
    
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            inventory.GetSlots[i].parent = this;
            inventory.GetSlots[i].OnAfterUpdate += OnSlotUpdate;
        }
    }

    private void Start() 
    {

        CreateSlots();
        
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    private void OnSlotUpdate(InventorySlot _slot)
    {
        Text text = _slot.slotDisplay.transform.Find("amount").GetComponent<Text>();
        Image image = _slot.slotDisplay.transform.Find("image").GetComponent<Image>();

        // If has an item
        if (_slot.item.Id >= 0)
        {
            image.sprite = _slot.ItemObject.itemSprite;
            text.text = _slot.amount == 1 ? "" : _slot.amount.ToString();
            
            foreach (Transform child in _slot.slotDisplay.transform)
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

    void Update()
    {
        // TODO: This shouldn't be here
        if (Input.GetKeyDown("i")) {
            SwitchInventoryVisibleInvisible();
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
            // TODO: is the funtion below doing anything?
            slotsOnInterface.UpdateSlotDisplay();
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
        MouseData.slotHoveredOver = obj;
    }
    public void OnExit(GameObject obj)
    {
        MouseData.slotHoveredOver = null;
    }

    public void OnEnterInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = obj.GetComponent<UserInterface>();
    }

    public void OnExitInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }

    public void OnDragStart(GameObject obj)
    {
        MouseData.tempItemBeingDragged = CreateTempItem(obj);
    }

    public GameObject CreateTempItem(GameObject obj)
    {
        GameObject tempItem = null;
        if (slotsOnInterface[obj].item.Id >= 0)
        {
            tempItem = new GameObject();
            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            tempItem.transform.SetParent(transform.parent);
            var img = tempItem.AddComponent<Image>();
            img.sprite = slotsOnInterface[obj].ItemObject.itemSprite;
            img.raycastTarget = false;
        }
        return tempItem;

    }

    public void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempItemBeingDragged);
        if (MouseData.interfaceMouseIsOver == null)
        {
            slotsOnInterface[obj].RemoveItem();
            return;
        }
        if (MouseData.slotHoveredOver)
        {
            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
            inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
        }
    }
    public void OnDrag(GameObject obj)
    {
        if (MouseData.tempItemBeingDragged != null)
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
    }
}

public static class MouseData
{   
    public static UserInterface interfaceMouseIsOver;
    public static GameObject tempItemBeingDragged;
    public static GameObject slotHoveredOver;
}

public static class ExtensionMethods
{
    public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
    {
        foreach (KeyValuePair<GameObject, InventorySlot> _slot in _slotsOnInterface)
        {
        }
    }
}