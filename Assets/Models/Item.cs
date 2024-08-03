using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MainController;

public enum ItemType
{
    HealthPotion,
    ManaPotion,
    Weapon,
    Armor,
    Block,
}

[System.Serializable]
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public string icon;
    public int amount;
    public string describe;
    public string prefab;
    public virtual void Use()
    {
        Debug.Log("Using item: " + itemName);
    }
    public Item(string itemName, ItemType itemType, string icon, int amount, string describe, string prefab)
    {
        this.itemName = itemName;
        this.itemType = itemType;
        this.icon = icon;
        this.amount = amount;
        this.describe = describe;
        this.prefab = prefab;
    }

    public void CheckNewItem(string name)
    {
        switch (name)
        {
            case "health_potion":
                this.itemType = ItemType.HealthPotion;
                this.itemName = "Health Potion";
                this.prefab = "Items/Prefabs/health_potion";
                this.icon = "Inventory_Icon/Item/health_potion_icon";
                this.amount = 1;
                this.describe = "Restores 50 health points.";
                break;
            case "mana_potion":
                this.itemType = ItemType.ManaPotion;
                this.itemName = "Mana Potion";
                this.prefab = "Items/Prefabs/mana_potion";
                this.icon = "Inventory_Icon/Item/mana_potion_icon";
                this.amount = 1;
                this.describe = "Restores 30 mana points.";
                break;
            case "BlackBlock":
                this.itemType = ItemType.Block;
                this.itemName = "Black Block";
                this.prefab = "Blocks/BlackBlock";
                this.icon = "Inventory_Icon/Block/BlackIcon";
                this.amount = 1;
                this.describe = "A solid black block.";
                break;
            case "WhiteBlock":
                this.itemType = ItemType.Block;
                this.itemName = "White Block";
                this.prefab = "Blocks/WhiteBlock";
                this.icon = "Inventory_Icon/Block/WhiteIcon";
                this.amount = 1;
                this.describe = "A solid white block.";
                break;
            case "SoilBlock":
                this.itemType = ItemType.Block;
                this.itemName = "Soil Block";
                this.prefab = "Blocks/SoilBlock";
                this.icon = "Inventory_Icon/Block/SoilIcon";
                this.amount = 1;
                this.describe = "A block of soil.";
                break;
            case "RockBlock":
                this.itemType = ItemType.Block;
                this.itemName = "Rock Block";
                this.prefab = "Blocks/RockBlock";
                this.icon = "Inventory_Icon/Block/RockIcon.png";
                this.amount = 1;
                this.describe = "A rock block.";
                break;
            default:
                Debug.LogWarning("Unknown item: " + name);
                break;
        }
    }

}

