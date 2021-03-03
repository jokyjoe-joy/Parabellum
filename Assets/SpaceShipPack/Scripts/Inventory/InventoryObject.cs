using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string savePath = "/inventory.dat";
    public ItemDatabaseObject database;
    public Inventory Container;

    private void OnEnable()
    {
        // Load database
        #if UNITY_EDITOR
        database = (ItemDatabaseObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/ItemDatabase.asset", typeof(ItemDatabaseObject));
        #else
        database = Resources.Load<ItemDatabaseObject>("ItemDatabase");
        #endif
    }

    public bool AddItem(Item _item, int _amount)
    {
        if (EmptySlotCount <= 0) return false;
        InventorySlot slot = FindItemOnInventory(_item);

        if (!database.GetItem[_item.Id].stackable || slot == null)
        {
            SetFirstEmptySlot(_item, _amount);
            return true;
        }
        slot.AddAmount(_amount);
        return true;
    }

    public InventorySlot FindItemOnInventory(Item _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.Id == _item.Id)
            {
                return Container.Items[i];
            }
        }
        return null;
    }

    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < Container.Items.Length; i++)
            {
                if (Container.Items[i].item.Id <= -1) counter++;
            }
            return counter;
        }
    }

    public InventorySlot SetFirstEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.Id <= -1)
            {
                Container.Items[i].UpdateSlot(_item, _amount);
                return Container.Items[i];
            }
        }
        // TODO: If inventory is full
        return null;
    }

    public void SwapItems(InventorySlot item1, InventorySlot item2)
    {
        if (item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject))
        {
            InventorySlot temp = new InventorySlot(item2.item, item2.amount);
            item2.UpdateSlot(item1.item, item1.amount);
            item1.UpdateSlot(temp.item, temp.amount);
        }

    }

    public void RemoveItem(Item _item)
    {
        // TODO: Drop it in real world
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item == _item)
            {
                Container.Items[i].UpdateSlot(null, 0);
            }
        }
    }

    public void Save()
    {
        // Turn this whole InventoryObject to JSON and save it to savePath.
        //string saveData = JsonUtility.ToJson(this, true);
        string saveData = JsonUtility.ToJson(Container, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        bf.Serialize(file, saveData);
        file.Close();
    }

    public void Load() 
    {
        // Check if we already have a save, if we do, then overwrite this InventoryObject based on that.
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath))) 
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), Container);
            file.Close();
        }
    }

    [ContextMenu("Clear")]
    public void Clear() {
        Container.Clear();
    }
}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[24];
    public void Clear()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            // TODO: Rather use RemoveItem()? 
            Items[i].UpdateSlot(new Item(), 0);
        }
    }
}

[System.Serializable]
public class InventorySlot {
    public ItemType[] AllowedItems = new ItemType[0];
    [System.NonSerialized] public UserInterface parent;
    public Item item;
    public int amount;

    public ItemObject ItemObject
    {
        get
        {
            if (item.Id >= 0)
            {
                return parent.inventory.database.GetItem[item.Id];
            }
            return null;
        }
    }

    public InventorySlot(Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
    public void UpdateSlot(Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
    public InventorySlot()
    {
        item = new Item();
        amount = 0;
    }
    public void RemoveItem()
    {
        item = new Item();
        amount = 0;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }

    public bool CanPlaceInSlot(ItemObject _itemObject)
    {
        if (AllowedItems.Length <= 0 || _itemObject == null || _itemObject.data.Id < 0)
            return true;
        
        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if (_itemObject.type == AllowedItems[i]) return true;
        }
        return false;
    }
}