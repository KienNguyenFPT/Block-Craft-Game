using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using System.Linq;
using UnityEngine.UI;
public class MainController : MonoBehaviour
{
    private List<SaveGameData> saveGameDataList = new List<SaveGameData>();
    public List<GameObject> listPanel;
    public TMP_InputField usernameInputField;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "startgame")
        {
            LoadSavedData();
            SortAndDisplaySavedData();
        }
    }
    public void runStart()
    {
        if (usernameInputField != null)
        {
            string inputText = usernameInputField.text;

            if (!string.IsNullOrEmpty(inputText))
            {
                StaticLoadData.userName = inputText.Trim();
                StaticLoadData.IsLoadData = false;
                SceneManager.LoadScene("BlockGame", LoadSceneMode.Single);
            }
        }
                
    }

    public void runHome()
    {
        SceneManager.LoadScene("startgame", LoadSceneMode.Single);
    }
    public void OnLoadButtonClicked(Button clickedButton)
    {
        int clickNumber = int.Parse(clickedButton.name.Split("_").Last()) - 1;
        if (clickNumber >= saveGameDataList.Count) return;
        SaveGameData data = saveGameDataList[clickNumber];
        if (data != null)
        {
            StaticLoadData.IsLoadData = true;
            StaticLoadData.SetDataFromSaveGameData(data);
            SceneManager.LoadScene("BlockGame", LoadSceneMode.Single);
        }
    }

    public void saveGame()
    {
        LoadSavedData();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError("No GameObject with the tag 'Player' found.");
            return;
        }

        GuyInventory inventoryComponent = playerObject.GetComponent<GuyInventory>();
        GuyStats statsComponent = playerObject.GetComponent<GuyStats>();

        if (inventoryComponent == null || statsComponent == null)
        {
            Debug.LogError("GuyInventory or GuyStats component not found on the specified GameObject.");
            return;
        }

        List<Item> inventory = inventoryComponent.inventory;
        List<Weapon> weapons = inventoryComponent.weapons;

        List<ItemToSave> itemsToSave = new List<ItemToSave>();
        foreach (Item item in inventory)
        {
            ItemToSave itemToSave = new ItemToSave(item);
            itemsToSave.Add(itemToSave);
        }

        List<WeaponToSave> weaponsToSave = new List<WeaponToSave>();

        foreach (Weapon weapon in weapons)
        {
            WeaponToSave weaponToSave = new WeaponToSave(weapon);
            weaponsToSave.Add(weaponToSave);
        }

        string userName = statsComponent.userName;
        int level = statsComponent.level;
        int currentHealth = statsComponent.currentHealth;
        int currentMana = statsComponent.currentMana;
        int currentExp = statsComponent.currentExp;
        Vector3 playerPosition = playerObject.transform.position;

        SaveGameData saveData = new SaveGameData(userName, level, currentHealth, currentMana, currentExp, itemsToSave, weaponsToSave, playerPosition, DateTime.UtcNow);

        saveGameDataList.Add(saveData);

        SaveToFile();
    }

    private void SaveToFile()
    {

        string json = JsonHelper.ToJson<SaveGameData>(saveGameDataList.ToArray());

        string filePath = Application.persistentDataPath + "/savegame.json";
        File.WriteAllText(filePath, json);

        Debug.Log("Game saved to: " + filePath);
    }
    public void exit()
    {
        Application.Quit();
    }

    [System.Serializable]
    public class SaveGameData
    {
        public string userName;
        public int level;
        public int currentHealth;
        public int currentMana;
        public int currentExp;
        public List<ItemToSave> inventory;
        public List<WeaponToSave> weapons;
        public Vector3 playerPosition;
        public string dateSave;

        public SaveGameData(string userName, int level, int currentHealth, int currentMana, int currentExp, List<ItemToSave> inventory, List<WeaponToSave> weapons, Vector3 playerPosition, DateTime dateSave)
        {
            this.userName = userName;
            this.level = level;
            this.currentHealth = currentHealth;
            this.currentMana = currentMana;
            this.currentExp = currentExp;
            this.inventory = inventory;
            this.weapons = weapons;
            this.playerPosition = playerPosition;
            this.dateSave = dateSave.ToString("o");
        }
    }

    [System.Serializable]
    public class WeaponToSave
    {
        public string itemName;
        public ItemType itemType;
        public string icon;
        public int amount;
        public string describe;
        public int attackPower;
        public float attackDistance;
        public string prefab;

        public WeaponToSave(Weapon weapon)
        {
            itemName = weapon.itemName;
            itemType = weapon.itemType;
            icon = weapon.icon;
            amount = weapon.amount;
            describe = weapon.describe;
            attackPower = weapon.attackPower;
            attackDistance = weapon.attackDistance;
            prefab = weapon.prefab;
        }
    }

    [System.Serializable]
    public class ItemToSave
    {
        public string itemName;
        public ItemType itemType;
        public string icon;
        public int amount;
        public string describe;
        public string prefab;

        public ItemToSave(Item item)
        {
            itemName = item.itemName;
            itemType = item.itemType;
            icon = item.icon;
            amount = item.amount;
            describe = item.describe;
            prefab = item.prefab;
        }
    }

    private void LoadSavedData()
    {
        string filePath = Application.persistentDataPath + "/savegame.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            saveGameDataList = JsonHelper.FromJson<SaveGameData>(json).ToList();

            Debug.Log("Loaded saved data.");
        }
        else
        {
            Debug.Log("No saved data found.");
        }
    }

    private void SortAndDisplaySavedData()
    {
        saveGameDataList.Sort((data1, data2) =>
        {
            int levelComparison = data2.level.CompareTo(data1.level);
            if (levelComparison == 0)
            {
                int expComparison = data2.currentExp.CompareTo(data1.currentExp);
                if (expComparison == 0)
                {
                    return data2.dateSave.CompareTo(data1.dateSave);
                }
                return expComparison;
            }
            return levelComparison;
        });


        for (int i = 0; i < listPanel.Count && i < saveGameDataList.Count; i++)
        {
            TextMeshProUGUI textElement = listPanel[i].GetComponentInChildren<TextMeshProUGUI>();
            SaveGameData data = saveGameDataList[i];
            textElement.text = $"Name: {data.userName} Level: {data.level} Exp: {data.currentExp} \nDate: {DateTime.Parse(data.dateSave)}";
        }
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}
