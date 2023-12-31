using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo_UpgradesSkills : MonoBehaviour
{
    // Dragged & dropped the "Item Image" child game object of "item" prefab here. I will then use its ".sprite" to assign it to the sprite from "CreateItem_SO" inside ShopManager.cs
    // This "Image" type is actually the COMPONENT attached to "Item Image" child game object
    public Image itemImg;
    
    public TextMeshProUGUI itemNameTxt;
    public TextMeshProUGUI descriptionTxt;
    public Image[] upgradeBarsList; // list of the item's upgrade bars
    public TextMeshProUGUI nextUpgradeInfoTxt;
    public TextMeshProUGUI costTxt;
}
