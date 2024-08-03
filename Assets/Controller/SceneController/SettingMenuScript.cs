using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingMenuScript : MonoBehaviour
{
    public TMP_Dropdown graphicsDropdown;

    public void changeGraphicsQuantity()
    {
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
