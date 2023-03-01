using JetBrains.Annotations;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerHandleItem : MonoBehaviour
{
    InventoryManager _inventoryManager;
    InputsManager _inputs;

    [SerializeField] private GameObject _gunObj;


    private ItemScriptableObject currentItem;
    private int ItemDropCount;

    [SerializeField]private ItemType currentItemType;
    [SerializeField] private Transform _prefabHolder;
    [SerializeField] private Transform _dropItemSpawnPos;
    [SerializeField] private GameObject[] _objPrefab;
    [SerializeField]private float spawnRadius = 5f;

    [SerializeField] private float _scatterForce;
    private void Start()
    {
        _inventoryManager = InventoryManager.Instance;
        _inputs = InputsManager.instance;
        _inventoryManager.OnSelectedSlotChanged += SelectTheItem;
    }
    private void OnDisable()
    {
        _inventoryManager.OnSelectedSlotChanged -= SelectTheItem;
    }
    private void SelectTheItem()
    {
        GetSelectedItem();
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
    public void GetSelectedItem()
    {
        currentItem = _inventoryManager.GetSelectedItem();
        if (currentItem != null)
        { 
            if (currentItem.Type == ItemType.Gun)
            {
                _gunObj.SetActive(true);
            }
            else
            {
                _gunObj.SetActive(false);
            }
        }
    }

    public void DropSelectedItem()
    {
        GetSelectedItem();
        
        int ItemCount = _inventoryManager.DropSelectedItem();
        if (ItemCount > 0)
        {
            ItemDropCount = ItemCount;

            currentItemType = currentItem.Type;
            switch (currentItemType)
            {
                case ItemType.Gun:
                    {
                        ScatterDroppedItem(_objPrefab[0], ItemDropCount);
                        break;
                    }
                case ItemType.Bullets:
                    {
                        ScatterDroppedItem(_objPrefab[1], ItemDropCount);
                        break;
                    }
                case ItemType.MedicKit:
                    {
                        ScatterDroppedItem(_objPrefab[2], ItemDropCount);
                        break;
                    }
            }
        }
       

       
    }

    private void ScatterDroppedItem(GameObject Prefab, int SpawnCount)
    {
        for (int i = 0; i < SpawnCount; i++)
        {
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
