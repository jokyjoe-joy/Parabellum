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
    public GameObject itemTooltipPrefab;
    GameObject currentItemTooltip;
    public float tooltipOffsetX = 150;

    private void Awake()
    {
        itemSlotContainer = transform.Find("itemSlotContainer");

        // Deactivating inventory UI if active
        foreach (Transform child in transform) child.gameObject.SetActive(false);
        isInventoryActive = false;
        playerShipController = GameObject.FindGameObjectWithTag("PlayerTag").transform.parent.GetComponent<ShipController>();
    
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            inventory.GetSlots[i].parent = this;
            inventory.GetSlots[i].OnAfterUpdate += OnSlotUpdate;
        }

        // Check for setup errors
        if (this.GetComponent<DynamicInterface>() != null) 
        {
            itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");
            Button slotButton = itemSlotTemplate.GetComponent<Button>();
            if (slotButton.colors.normalColor == slotButton.colors.highlightedColor)
                Debug.LogWarning(string.Concat(slotButton.gameObject.name, "'s button's normal color and highlighted color are the same. Slot highlighting on cursor enter may not work as intended."));
        }

        StaticInterface IStatic = this.GetComponent<StaticInterface>();
        if (IStatic != null)
        {
            foreach (GameObject slot in IStatic.slots)
            {
                Button slotButton = slot.GetComponent<Button>();
                if (slotButton.colors.normalColor == slotButton.colors.highlightedColor)
                    Debug.LogWarning(string.Concat(slotButton.gameObject.name, "'s button's normal color and highlighted color are the same. Slot highlighting on cursor enter may not work as intended."));
            }
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
            if (currentItemTooltip != null) Destroy(currentItemTooltip);
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

        if (this.GetComponent<DynamicInterface>() != null)
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

        Image objImage = obj.GetComponent<Image>();
        Button objButton = obj.GetComponent<Button>();
        objImage.color = objButton.colors.highlightedColor;


        // If there is an item in the slot, display a UI tooltip for it.
        if (slotsOnInterface[obj].ItemObject != null && currentItemTooltip == null)
        {
            // Put the tooltip a little bit next to the cursor, so you can see the selected slot.
            Vector3 positionToInstantiate = obj.transform.position;
            positionToInstantiate.x += tooltipOffsetX;

            currentItemTooltip = Instantiate(itemTooltipPrefab, positionToInstantiate, Quaternion.identity);
            // Need to be transform.parent, so that it is over other interface
            currentItemTooltip.transform.SetParent(transform.parent); 

            if (!currentItemTooltip.GetComponent<RectTransform>().IsFullyVisibleFrom(Camera.main))
            {
                // TODO: Fix this and place tooltip so that it is visible.
                Debug.Log("not visible");
            }

            Text name = currentItemTooltip.transform.Find("name").GetComponent<Text>();
            Text description = currentItemTooltip.transform.Find("description").GetComponent<Text>();

            name.text = slotsOnInterface[obj].ItemObject.itemName;
            description.text = slotsOnInterface[obj].ItemObject.itemDescription;

            // Set etc1 & etc2 texts if weapon to damage and range
            if (slotsOnInterface[obj].ItemObject.type == ItemType.Weapon)
            {
                Text etc1 = currentItemTooltip.transform.Find("etc1").GetComponent<Text>();
                Text etc2 = currentItemTooltip.transform.Find("etc2").GetComponent<Text>();
                etc1.text = "Damage: " + slotsOnInterface[obj].ItemObject.prefabToEquip.GetComponent<Gun>().damageAmount.ToString();
                etc2.text = "Range: " + slotsOnInterface[obj].ItemObject.prefabToEquip.GetComponent<Gun>().maxRange.ToString();
            }
        }
    }
    public void OnExit(GameObject obj)
    {
        MouseData.slotHoveredOver = null;
        Image objImage = obj.GetComponent<Image>();
        Button objButton = obj.GetComponent<Button>();
        objImage.color = objButton.colors.normalColor;

        if (currentItemTooltip != null) Destroy(currentItemTooltip);
        // Need to set it to null, as without it you can't select two items right after each other.
        currentItemTooltip = null;
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
 
public static class RendererExtensions
{
    /// <summary>
    /// Counts the bounding box corners of the given RectTransform that are visible from the given Camera in screen space.
    /// </summary>
    /// <returns>The amount of bounding box corners that are visible from the Camera.</returns>
    /// <param name="rectTransform">Rect transform.</param>
    /// <param name="camera">Camera.</param>
    private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height); // Screen space bounds (assumes camera renders across the entire screen)
        Vector3[] objectCorners = new Vector3[4];
        rectTransform.GetWorldCorners(objectCorners);
 
        int visibleCorners = 0;
        Vector3 tempScreenSpaceCorner; // Cached
        for (var i = 0; i < objectCorners.Length; i++) // For each corner in rectTransform
        {
            tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]); // Transform world space position of corner to screen space
            if (screenBounds.Contains(tempScreenSpaceCorner)) // If the corner is inside the screen
            {
                visibleCorners++;
            }
        }
        return visibleCorners;
    }
 
    /// <summary>
    /// Determines if this RectTransform is fully visible from the specified camera.
    /// Works by checking if each bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
    /// </summary>
    /// <returns><c>true</c> if is fully visible from the specified camera; otherwise, <c>false</c>.</returns>
    /// <param name="rectTransform">Rect transform.</param>
    /// <param name="camera">Camera.</param>
    public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        return CountCornersVisibleFrom(rectTransform, camera) == 4; // True if all 4 corners are visible
    }
 
    /// <summary>
    /// Determines if this RectTransform is at least partially visible from the specified camera.
    /// Works by checking if any bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
    /// </summary>
    /// <returns><c>true</c> if is at least partially visible from the specified camera; otherwise, <c>false</c>.</returns>
    /// <param name="rectTransform">Rect transform.</param>
    /// <param name="camera">Camera.</param>
    public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        return CountCornersVisibleFrom(rectTransform, camera) > 0; // True if any corners are visible
    }
}