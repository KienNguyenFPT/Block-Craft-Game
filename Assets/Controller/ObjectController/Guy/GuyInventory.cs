using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GuyInventory : MonoBehaviour
{
    public List<Item> inventory = new List<Item>();
    public List<Weapon> weapons = new List<Weapon>();
    public List<Image> inventoryImages;
    public List<Image> weaponImages;

    public GuyInventory()
    {
        inventory = new List<Item>();
        weapons = new List<Weapon>();
    }
    public void AddItem(string itemName, ItemType type)
    {
        string name = itemName.Split("(").First();
        Item existingItem = inventory.Find(item => Regex.Replace(item.itemName, @"\s+", "") == name && item.itemType == type);
        if (existingItem != null)
        {
            existingItem.amount += 1;
            Debug.Log("Increased item amount in inventory: " + name);
        }
        else
        {
            Item newItem = ScriptableObject.CreateInstance<Item>();
            newItem.CheckNewItem(name);

            inventory.Add(newItem);
            UpdateInventoryImage(inventory.Count - 1, newItem.icon, inventoryImages);
            Debug.Log("Item added to inventory: " + name);
        }

        Debug.Log("Item added to inventory: " + name);
    }

    public void AddItem(Item item)
    {
        inventory.Add(item);
        UpdateInventoryImage(inventory.Count - 1, item.icon, inventoryImages);
        Debug.Log("Weapon added to inventory: " + item.itemName);
    }

    public void AddWeapon(string itemName)
    {
        Item existingItem = weapons.Find(item => item.itemName == itemName);
        if (existingItem != null)
        {
            return;
        }
        Weapon newWeapon = ScriptableObject.CreateInstance<Weapon>();

        newWeapon.CheckNewWeapon(itemName);

        weapons.Add(newWeapon);
        UpdateInventoryImage(weapons.Count - 1, newWeapon.icon, weaponImages);
        Debug.Log("Weapon added to inventory: " + newWeapon.itemName);
    }

    public void AddWeapon(Weapon weapon)
    {
        weapons.Add(weapon);
        UpdateInventoryImage(weapons.Count - 1, weapon.icon, weaponImages);
        Debug.Log("Weapon added to inventory: " + weapon.itemName);
    }

    public Weapon GetWeapon(int index)
    {
        if (index >= 0 && index < weapons.Count)
        {
            return weapons[index];
        }
        return null;
    }

    public void UpdateInventoryImage(int index, string newSprite, List<Image> images)
    {
        if (index >= 0 && index < images.Count && images[index] != null)
        {
            images[index].sprite = Resources.Load<Sprite>(newSprite);
        }
        else
        {
            Debug.LogError("Invalid index or Image is not assigned.");
        }
    }

}
