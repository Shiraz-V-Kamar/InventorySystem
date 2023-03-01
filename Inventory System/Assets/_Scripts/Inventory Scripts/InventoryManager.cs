using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;


    private int MaxStackedItems = 5;

    [SerializeField] private InventorySlot[] _inventorySlots;

    [SerializeField] private GameObject _inventoryItemPrefab;

    private int _selectedSlot = -1;
    InputsManager _inputs;
    private float _itemUseTimeoutDelta;
    [SerializeField] private float _itemUseTimeout;
    public Action OnSelectedSlotChanged;
    private void Awake()
    {
        Instance = this;
    }

    private void ChangeActiveSlotToNumKey()
    {
        if (_inputs.Slot1) ChangeSelectedSlot(0);
        else if (_inputs.Slot2) ChangeSelectedSlot(1);
        else if (_inputs.Slot3) ChangeSelectedSlot(2);
        else if (_inputs.Slot4) ChangeSelectedSlot(3);
        else if (_inputs.Slot5) ChangeSelectedSlot(4);
        else if (_inputs.Slot6) ChangeSelectedSlot(5);
        else if (_inputs.Slot7) ChangeSelectedSlot(6);
        else return;
    }
    private void Start()
    {
        _inputs = InputsManager.instance;
        ChangeSelectedSlot(0);
    }

    private void Update()
    {
        ChangeActiveSlotToNumKey();
    }
    public bool AddItem(ItemScriptableObject item)
    {
        //Checking each slot to see if the item collected can be stacked or not
        foreach (var slot in _inventorySlots)
        {
            InventoryItem slotItem = slot.GetComponentInChildren<InventoryItem>();
            if (slotItem != null && slotItem.Item == item && slotItem.Count < MaxStackedItems && item.IsStackable == true)
            {
                slotItem.Count++;
                slotItem.RefreshCount();
                return true;
            }
        }

        //Find an empty slot to store the Item
        foreach (var slot in _inventorySlots)
        {
            InventoryItem slotItem = slot.GetComponentInChildren<InventoryItem>();
            if (slotItem == null)
            {
                SpawnNewItem(item, slot.transform);
                return true;
            }
        }
        return false;
    }
    void ChangeSelectedSlot(int Value)
    {
        if (_selectedSlot >= 0)
        {
            _inventorySlots[_selectedSlot].Deselect();
        }
        _inventorySlots[Value].Select();
        _selectedSlot = Value;
        OnSelectedSlotChanged?.Invoke();
    }
    public void SpawnNewItem(ItemScriptableObject item, Transform InventorySlot)
    {
        GameObject newItem = Instantiate(_inventoryItemPrefab, InventorySlot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitializeItems(item);
    }

    public ItemScriptableObject GetSelectedItem()
    {
        InventorySlot slot = _inventorySlots[_selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot != null)
        {
            ItemScriptableObject item = itemInSlot.Item;
            return item;
        }
        return null;
    }

    public void UseSelectedItem()
    {
        InventorySlot slot = _inventorySlots[_selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            ItemScriptableObject item = itemInSlot.Item;

            // MedicKit is the only Consumable thus reduces it amount on keypress
            if (item.Type == ItemType.MedicKit)
            {
                itemInSlot.Count--;
                if (itemInSlot.Count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
                _inputs.UseItemPressed = false;
            }


        }
    }

    public int DropSelectedItem()
    {
        _inputs.DropItemPressed = false;
        InventorySlot slot = _inventorySlots[_selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            ItemScriptableObject item = itemInSlot.Item;
            Destroy(itemInSlot.gameObject);
            return itemInSlot.Count;
        }
        else
            return 0;

    }
}
