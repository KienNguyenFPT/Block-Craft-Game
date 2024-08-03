using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MainController;


[System.Serializable]
public class Weapon : Item
{
    public int attackPower;
    public float attackDistance;
    public override void Use()
    {
        base.Use();

        EquipWeapon();
    }

    public Weapon(WeaponToSave weaponToSave)
        : base(weaponToSave.itemName, weaponToSave.itemType, weaponToSave.icon, weaponToSave.amount, weaponToSave.describe, weaponToSave.prefab)
    {
        this.attackPower = weaponToSave.attackPower;
        this.attackDistance = weaponToSave.attackDistance;
    }
    private void EquipWeapon()
    {
        GuyAction playerWeaponManager = FindObjectOfType<GuyAction>();
        if (playerWeaponManager != null)
        {
            playerWeaponManager.EquipWeapon(this);
        }
    }
    public void CheckNewWeapon(string name)
    {
        
        switch(name)
        {
            case "dark_sword":
                this.itemType = ItemType.Weapon;
                this.itemName = "Dark Sword";
                this.prefab = "Weapons/Prefabs/dark_sword";
                this.icon = "Inventory_Icon/Weapon/dark_sword_icon";
                this.attackPower = 20;
                this.attackDistance = 0.1f;
                break;
            case "axe":
                this.itemType = ItemType.Weapon;
                this.itemName = "Axe";
                this.prefab = "Weapons/Prefabs/axe";
                this.icon = "Inventory_Icon/Weapon/axe_icon";
                this.attackPower = 10;
                this.attackDistance = 0.15f;
                break;
            case "hammer":
                this.itemType = ItemType.Weapon;
                this.itemName = "Hammer";
                this.prefab = "Weapons/Prefabs/hammer";
                this.icon = "Inventory_Icon/Weapon/hammer_icon";
                this.attackPower = 15;
                this.attackDistance = 0.3f;
                break;
            case "mace":
                this.itemType = ItemType.Weapon;
                this.itemName = "Mace";
                this.prefab = "Weapons/Prefabs/mace";
                this.icon = "Inventory_Icon/Weapon/mace_icon";
                this.attackPower = 25;
                this.attackDistance = 0.1f;
                break;
            case "skull_axe":
                this.itemType = ItemType.Weapon;
                this.itemName = "Skull Sword";
                this.prefab = "Weapons/Prefabs/skull_axe";
                this.icon = "Inventory_Icon/Weapon/skull_axe_icon";
                this.attackPower = 20;
                this.attackDistance = 0.2f;
                break;
        }
    }
}

