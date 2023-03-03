using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [Header("UI")]
    public Image _itemImage;
    public TextMeshProUGUI CountText;
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI SlotId;

    public GameObject SlotIconHolder;

    [HideInInspector] public ItemScriptableObject Item;
    [HideInInspector] public int Count = 1;

    
    public void InitializeItems(ItemScriptableObject _item)
    {
        // Sets the values from the item into the slot information in inventory
        InventorySlot parentSlot = transform.parent.GetComponent<InventorySlot>();

        Item = _item;
        _itemImage.sprite = _item.Image;
        ItemName.text = _item.Name;
        
        if (parentSlot != null)
            SlotId.text = parentSlot.slotId.ToString();
        RefreshCount();
    }

    public void RefreshCount()
    {
        //updating the item count and sloticon
        CountText.text = Count.ToString();

        bool showText = Count > 1;
        CountText.gameObject.SetActive(showText);
        SlotIconHolder.SetActive(showText); 
    }
}
