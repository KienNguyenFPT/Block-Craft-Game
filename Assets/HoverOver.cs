using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MainController;

public class HoverOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject hoverPanel;
    public GameObject Guy;
    public GameObject WeaponsList;
    public GameObject InventoryList;
    private GuyInventory inventory;
    private GuyAction guyAction;

    private void Start()
    {
        inventory = Guy.GetComponent<GuyInventory>();
        guyAction = Guy.GetComponent<GuyAction>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject hoveredObject = eventData.pointerEnter;

        if (hoveredObject != null)
        {
            Transform parentTransform = hoveredObject.transform.parent;

            if (parentTransform != null &&
               (parentTransform.name == "Weapon" || parentTransform.name == "Inventory"))
            {
                string[] nameParts = hoveredObject.name.Split('_');
                if (int.TryParse(nameParts.Last(), out int index))
                {
                    if (parentTransform.name == "Weapon" && inventory != null && index >= 0 && index < inventory.weapons.Count)
                    {
                        Weapon weapon = inventory.weapons[index];

                        if (weapon != null)
                        {
                            hoverPanel.transform.Find("Avt").GetComponent<Image>().sprite = Resources.Load<Sprite>(weapon.icon);
                            hoverPanel.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = weapon.itemName;
                            hoverPanel.transform.Find("Content").GetComponent<TextMeshProUGUI>().text = "Type: " + weapon.itemType + "\nAttack Power: " + weapon.attackPower + "\nAttack Distance: " + weapon.attackDistance + "\nDescribe: " + weapon.describe;
                            hoverPanel.SetActive(true);
                        }
                    }
                    else if (parentTransform.name == "Inventory" && inventory != null && index >= 0 && index < inventory.inventory.Count)
                    {
                        Item item = inventory.inventory[index];

                        if (item != null)
                        {
                            hoverPanel.transform.Find("Avt").GetComponent<Image>().sprite = Resources.Load<Sprite>(item.icon);
                            hoverPanel.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
                            hoverPanel.transform.Find("Content").GetComponent<TextMeshProUGUI>().text = "Type: " + item.itemType + "\nAmount: " + item.amount + "\nDescribe: " + item.describe;
                            hoverPanel.SetActive(true);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("GameObject is not of type Weapon.");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverPanel.SetActive(false);
    }
    public void OnWeaponsButtonClicked(Button clickedButton)
    {
        int clickNumber = int.Parse(clickedButton.name.Split("_").Last()) - 1;
    }

    public void OnInventoryButtonClicked(Button clickedButton)
    {
        int clickNumber = int.Parse(clickedButton.name.Split("_").Last());
        if (clickNumber >= 0 && clickNumber < inventory.inventory.Count)
        {
            Item item = inventory.inventory[clickNumber];

            if (item.amount > 0 && item.itemType == ItemType.Block && item != guyAction.currentBlock)
            {
                guyAction.currentBlock = item;
                HandleItemSwitch(clickNumber);
                Debug.Log("Current block set to: " + guyAction.currentBlock.itemName);
            }else if (item == guyAction.currentBlock)
            {
                Color overlayColorDis = new Color(212 / 255f, 212 / 255f, 212 / 255f, 1.0f);
                Image childOverlay = WeaponsList.GetComponentsInChildren<Image>()[clickNumber + 1];
                changeColorOverlay(childOverlay, overlayColorDis);
                guyAction.currentBlock = null;
            }
            else
            {
                guyAction.currentBlock = null;
                Debug.LogWarning("Selected item is not a valid block or amount is zero.");
            }
        }
        else
        {
            Debug.LogWarning("Invalid inventory slot selected.");
        }
    }

    void HandleItemSwitch(int itemClick)
    {
        Color overlayColor = new Color(226 / 255f, 84 / 255f, 84 / 255f, 1.0f);
        Color overlayColorDis = new Color(212 / 255f, 212 / 255f, 212 / 255f, 1.0f);
        for (int i = 0; i <= 12; i++)
        {
            Image childOverlay = WeaponsList.GetComponentsInChildren<Image>()[i + 1];
            if (i != itemClick)
            {
                changeColorOverlay(childOverlay, overlayColorDis);
            }
            else if (inventory.inventory[i] != null)
            {
                changeColorOverlay(childOverlay, overlayColor);
            }
        }
    }

    void changeColorOverlay(Image overlayImage, Color color)
    {
        if (overlayImage != null)
        {
            overlayImage.color = color;
        }
    }
}
