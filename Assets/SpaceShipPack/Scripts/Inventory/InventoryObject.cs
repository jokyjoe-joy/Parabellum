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
    public ItemDatabaseObject database; // private, so JSON won't save it
    public Inventory Container;

/* 
    private void OnEnable()
    {
        // Load database
        #if UNITY_EDITOR
        database = (ItemDatabaseObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/ItemDatabase.asset", typeof(ItemDatabaseObject));
        #else
        database = Resources.Load<ItemDatabaseObject>("ItemDatabase");
        #endif
    } */

    public void AddItem(Item _item, int _amount)
    {
        // This is mostly a workaround if stackable is set wrong.
        if (_item.buffs.Length > 0)
        {
            Container.Items.Add(new InventorySlot(_item.Id, _item, _amount));
            return;
        }

        // Loop through inventory and check if we have item
        // If we already have the item and it is stackable, add the amount to it.
        for (int i = 0 ; i < Container.Items.Count; i++)
        {
            if (Container.Items[i].item.Id == _item.Id) 
            {
                if (_item.stackable) 
                {
                    Container.Items[i].AddAmount(_amount);
                    return;
                };
            }
        }

        // If we don't have the item or we do and it is not stackable, add it to inventory
        Container.Items.Add(new InventorySlot(_item.Id, _item, _amount));
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

    public void Clear() {
        Container = new Inventory();
    }
}

[System.Serializable]
public class Inventory
{
    public List<InventorySlot> Items = new List<InventorySlot>();
}

[System.Serializable]
public class InventorySlot {
    public int ID;
    public Item item;
    public int amount;
    public InventorySlot(int _id, Item _item, int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }
}