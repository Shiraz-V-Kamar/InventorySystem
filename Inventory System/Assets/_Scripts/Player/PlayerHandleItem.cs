using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerHandleItem : MonoBehaviour
{

    [SerializeField]private GameObject _gunObj;
    [SerializeField]private int _itemDropCount;
    [SerializeField]private Transform _prefabHolder;
    [SerializeField]private Transform _dropItemSpawnPos;
    [SerializeField]private GameObject[] _objPrefab;

    [SerializeField] private float _scatterForce;
    [SerializeField]private float spawnRadius = 5f;

    [HideInInspector]public ItemScriptableObject currentItem;
    private ItemType CurrentItemType;

    // Scripts
    InventoryManager _inventoryManager;
    InputsManager _inputs;
    LevelManager _levelManager;
    AudioManager _audioManager;

    // Delegates
    public Action<bool> OnHoldingGun;
    private void Start()
    {
        _audioManager = AudioManager.instance;
        _inventoryManager = InventoryManager.Instance;
        _inputs = InputsManager.instance;
        _levelManager = LevelManager.instance;

        _inventoryManager.OnSelectedSlotChanged += SelectTheItem;
        _inventoryManager.OnItemDropped += ItemDroped;
    }
    private void OnDisable()
    {
        _inventoryManager.OnSelectedSlotChanged -= SelectTheItem;
        _inventoryManager.OnItemDropped -= ItemDroped;
    }

    private void Update()
    {
        if (_inputs.UseItemPressed)
        {
            _inventoryManager.UseSelectedItem();
        }
        if (_inputs.DropItemPressed)
        {
            DropSelectedItem();
        }
    }
    private void ItemDroped(ItemType type, int dropCount)
    {
        CurrentItemType = type;
        _itemDropCount = dropCount;
        GetSelectedItem();
    }
    private void SelectTheItem()
    {
        GetSelectedItem();
    }
    public void GetSelectedItem()
    {
        // Allows the player to equip gun which is primary items
        currentItem = _inventoryManager.GetSelectedItem();
        if (currentItem != null)
        { 
            if (currentItem.Type == ItemType.Gun)
            {
                _gunObj.SetActive(true);
                OnHoldingGun?.Invoke(true);
            }
            else
            {
                _gunObj.SetActive(false);
                OnHoldingGun?.Invoke(false);
            }

            // if the player has not loaded bullets and gun 
            if(currentItem.Type == ItemType.Bullets && _levelManager.BulletCount==0 && _inventoryManager.hasGun)
            {
                _inventoryManager.UseBulletItem();
            }
        }else
        {
             OnHoldingGun?.Invoke(false);
            _gunObj.SetActive(false);
        }
    }

    public void DropSelectedItem()
    {   
        // Checks selected item type and instantiares respective prefabs
        bool canDropItem = _inventoryManager.DropSelectedItem();

        if (_itemDropCount > 0 && canDropItem)
        {
            switch (CurrentItemType)
            {
                case ItemType.Gun:
                    {
                        ScatterDroppedItem(_objPrefab[0], _itemDropCount);
                        break;
                    }
                case ItemType.Bullets:
                    {
                        ScatterDroppedItem(_objPrefab[1], _itemDropCount);
                        break;
                    }
                case ItemType.MedicKit:
                    {
                        ScatterDroppedItem(_objPrefab[2], _itemDropCount);
                        break;
                    }
            }
        }
    }

    private void ScatterDroppedItem(GameObject Prefab, int SpawnCount)
    {
        // Instantiate items, according to the number of items dropped
        for (int i = 0; i < SpawnCount; i++)
        {
            _audioManager.PlaySound(Helper.DROP_ITEMS);
            Vector3 randomDir = _dropItemSpawnPos.position + Random.insideUnitSphere * spawnRadius;
            if (randomDir.y < transform.position.y) // check if spawn position is below player
            {
                randomDir.y = transform.position.y; // adjust y-coordinate to player's y-coordinate
            }
            Quaternion spawnRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            Vector3 randSpawnPos = new Vector3(Random.Range(_dropItemSpawnPos.position.x - i,_dropItemSpawnPos.position.x + i),
                                    _dropItemSpawnPos.position.y, 
                                    Random.Range(_dropItemSpawnPos.position.z - i, _dropItemSpawnPos.position.z + i));
            GameObject spawnedObj = Instantiate(Prefab, randSpawnPos, spawnRotation, _prefabHolder);
            spawnedObj.GetComponent<BoxCollider>().isTrigger = false;
            spawnedObj.GetComponent<Rigidbody>().AddForce(randomDir * _scatterForce,ForceMode.Impulse);
        }
    }

    
}
