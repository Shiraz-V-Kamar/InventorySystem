using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;


    private int MaxStackedItems = 5;

    [SerializeField] private InventorySlot[] _inventorySlots;

    [SerializeField] private GameObject _inventoryItemPrefab;

    private int _selectedSlot = 1;
    
    InputsManager _inputs;
    LevelManager _levelManager;

    private bool hasGun = false;


    public Action OnSelectedSlotChanged;
    public Action<ItemType, int> OnItemDropped;
    private bool hasBullet;

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
        _levelManager = LevelManager.instance;

    }

    private void Update()
    {
        ChangeActiveSlotToNumKey();
    }
    public bool AddItem(ItemScriptableObject item)
    {

        for (int i = 1; i < _inventorySlots.Length; i++)
        {
            InventoryItem itemInSlot = _inventorySlots[i].GetComponentInChildren<InventoryItem>();

            //Checking each slot to see if the item collected can be stacked or not
            if (itemInSlot != null && itemInSlot.Item == item && itemInSlot.Count < MaxStackedItems && item.IsStackable == true)
            {
                itemInSlot.Count++;
                itemInSlot.RefreshCount();
                return true;
            }

            //For items which is not stackable and is not unique store them in empty slot
            if (itemInSlot == null && item.IsUnique == false)
            {
                SpawnNewItem(item, _inventorySlots[i].transform);
                return true;
            }

            if (itemInSlot != null && item.Type == ItemType.Bullets)
            {
                hasBullet = true;   
            }
        }
    

        //checking if first slot has unique item, spawn if there is none
        Transform firstSlot = _inventorySlots[0].transform;
        InventoryItem itemInSlotOne = _inventorySlots[0].GetComponentInChildren<InventoryItem>();
        if (itemInSlotOne == null)
        {

            SpawnNewItem(item, firstSlot);

            ChangeSelectedSlot(0);
            OnSelectedSlotChanged?.Invoke();
            return true;


        }
        else
        {
            hasGun = true;
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
        InventoryItem itemInSlot = newItem.GetComponent<InventoryItem>();
        itemInSlot.InitializeItems(item);
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
            }
            else if(item.Type == ItemType.Bullets && _levelManager.BulletCount<5 && hasGun)
            {
                _levelManager.SetBulletToMax();
                itemInSlot.Count--;
                if (itemInSlot.Count <= 0)
                {
                    hasBullet = false;
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }
                _inputs.UseItemPressed = false;
        }
    }

    public void UseBulletItem()
    {
        // Checking if there is any bullet available then reduce itemCount by 1 and increase bulletCount 
        foreach (var slot in _inventorySlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.Item.Type == ItemType.Bullets && itemInSlot.Count > 0)
            {
                itemInSlot.Count--;
                itemInSlot.RefreshCount();
                _levelManager.SetBulletToMax();

                if (itemInSlot.Count == 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                break;
            }
        }
    }

    public void DropSelectedItem()
    {
        _inputs.DropItemPressed = false;
        InventorySlot slot = _inventorySlots[_selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            ItemScriptableObject item = itemInSlot.Item;
            int count = itemInSlot.Count;

            OnItemDropped?.Invoke(item.Type, count);
            if (item.Type == ItemType.Gun)
            {
                hasGun = false;
            }
            DestroyImmediate(itemInSlot.gameObject);
            ChangeSelectedSlot(_selectedSlot);
        }
    }
}
