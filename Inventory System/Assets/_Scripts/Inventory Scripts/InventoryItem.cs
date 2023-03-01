using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{

    [Header("UI")]
    public Image _itemImage;
    public TextMeshProUGUI CountText;

    [HideInInspector]public ItemScriptableObject Item;
    [HideInInspector] public int Count = 1;


    public void InitializeItems(ItemScriptableObject _item)
    {
        Item = _item;
        _itemImage.sprite = _item.Image;
        RefreshCount();
    }

    public void RefreshCount()
    {
        CountText.text = Count.ToString();

        bool showText = Count > 1;
        CountText.gameObject.SetActive(showText);
    }
}
