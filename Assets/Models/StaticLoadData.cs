using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MainController;

public class StaticLoadData : MonoBehaviour
{
    public static string userName;
    public static int level;
    public static int currentHealth;
    public static int currentMana;
    public static int currentExp;
    public static List<ItemToSave> inventory;
    public static List<WeaponToSave> weapons;
    public static Vector3 playerPosition;
    public static DateTime dateSave;
    public static bool IsLoadData;

    public static void SetDataFromSaveGameData(SaveGameData data)
    {
        userName = data.userName;
        level = data.level;
        currentHealth = data.currentHealth;
        currentMana = data.currentMana;
        currentExp = data.currentExp;
        inventory = data.inventory;
        weapons = data.weapons;
        playerPosition = data.playerPosition;
        dateSave = DateTime.Parse(data.dateSave);
    }

}
