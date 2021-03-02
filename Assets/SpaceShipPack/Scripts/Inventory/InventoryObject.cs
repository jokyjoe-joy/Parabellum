using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject, ISerializationCallbackReceiver
{
    public string savePath = "/inventory.dat";
    private ItemDatabaseObject database; // private, so JSON won't save it

    private void OnEnable()
    {
        // Load database
        #if UNITY_EDITOR
        database = (ItemDatabaseObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/ItemDatabase.asset", typeof(ItemDatabaseObject));
        #else
        database = Resources.Load<ItemDatabaseObject>("ItemDatabase");
        #endif
    }

    public List<InventorySlot> inventoryContainer = new List<InventorySlot>();
    public void AddItem(ItemObject _item, int _amount)
    {
        // Loop through inventory and check if we have item
        // If we already have the item and it is stackable, add the amount to it.
        for (int i = 0 ; i < inventoryContainer.Count; i++)
        {
            if (inventoryContainer[i].item == _item) 
            {
                if (_item.stackable) 
                {
                    inventoryContainer[i].AddAmount(_amount);
                    return;
                };
            }
        }

        // If we don't have the item or we do and it is not stackable, add it to inventory
        inventoryContainer.Add(new InventorySlot(database.GetId[_item], _item, _amount));
    }

    public void Save()
    {
        // Turn this whole InventoryObject to JSON and save it to savePath.
        string saveData = JsonUtility.ToJson(this, true);
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
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
        }
    }

    public void OnAfterDeserialize()
    {
        // TODO: Why are we doing this?
        for (int i = 0; i < inventoryContainer.Count; i++)
            inventoryContainer[i].item = database.GetItem[inventoryContainer[i].ID];
    }

    public void OnBeforeSerialize()
    {

    }

}

[System.Serializable]
public class InventorySlot {
    public int ID;
    public ItemObject item;
    public int amount;
    public InventorySlot(int _id, ItemObject _item, int _amount)
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