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

    public void AddItem(Item _item, int _amount)
    {
        // This is mostly a workaround if stackable is set wrong.
        if (_item.buffs.Length > 0)
        {
            SetFirstEmptySlot(_item, _amount);
            return;
        }

        // Loop through inventory and check if we have item
        // If we already have the item and it is stackable, add the amount to it.
        for (int i = 0 ; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].ID == _item.Id) 
            {
                if (_item.stackable) 
                {
                    Container.Items[i].AddAmount(_amount);
                    return;
                };
            }
        }

        // If we don't have the item or we do and it is not stackable, add it to inventory
        SetFirstEmptySlot(_item, _amount);
    }

    public InventorySlot SetFirstEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].ID <= -1)
            {
                Container.Items[i].UpdateSlot(_item.Id, _item, _amount);
                return Container.Items[i];
            }
        }
        // TODO: If inventory is full
        return null;
    }

    public void MoveItem(InventorySlot item1, InventorySlot item2)
    {
        InventorySlot temp = new InventorySlot(item2.ID, item2.item, item2.amount);
        item2.UpdateSlot(item1.ID, item1.item, item1.amount);
        item1.UpdateSlot(temp.ID, temp.item, temp.amount);
    }

    public void RemoveItem(Item _item)
    {
        // TODO: Drop it in real world
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item == _item)
            {
                Container.Items[i].UpdateSlot(-1, null, 0);
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
            Items[i].UpdateSlot(-1, new Item(), 0);
        }
    }
}

[System.Serializable]
public class InventorySlot {
    public ItemType[] AllowedItems = new ItemType[0];
    public UserInterface parent;
    public int ID = -1;
    public Item item;
    public int amount;
    public InventorySlot(int _id, Item _item, int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }
    public void UpdateSlot(int _id, Item _item, int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }
    public InventorySlot()
    {
        ID = -1;
        item = null;
        amount = 0;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }

    public bool CanPlaceInSlot(ItemObject _item)
    {
        if (AllowedItems.Length <= 0)
            return true;
        
        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if (_item.type == AllowedItems[i]) return true;
        }
        return false;
    }
}