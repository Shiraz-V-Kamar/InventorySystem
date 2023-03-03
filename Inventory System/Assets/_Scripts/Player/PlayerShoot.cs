using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _aimCamera;

    [SerializeField] private float _normalSensitivity = 1f;
    [SerializeField] private float _aimSensitivity = 0.5f;
    [SerializeField] private LayerMask aimLayerMask;

    private PlayerMovementInput _movementInput;
    private InputsManager _inputs;
    private InventoryManager _inventoryManager;
    private LevelManager _levelManager;
    private PlayerHandleItem _playerHandleItem;


    [SerializeField] private Gun _gun;
   
    private bool hasGun;

    public Action<bool> OnAimingAtCamera;
    private void Start()
    {
        _inputs = InputsManager.instance;
        _inventoryManager = InventoryManager.Instance;
        _levelManager = LevelManager.instance;
        _movementInput = GetComponent<PlayerMovementInput>();
        _playerHandleItem = GetComponent<PlayerHandleItem>();

        _playerHandleItem.OnHoldingGun += IsHoldingGun;
    }
    private void OnDisable()
    {
        _playerHandleItem.OnHoldingGun -= IsHoldingGun;
    }

    private void IsHoldingGun(bool obj)
    {
        hasGun = obj;
    }

    private void Update()
    {
        
        ToggleAimAndNormalCamera();

        Shooting();

        Reloading();

       
        
    }

    private void ToggleAimAndNormalCamera()
    {
        Vector3 MouseWorldPos = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimLayerMask))
        {
            if(hit.collider.tag== Helper.CAMERA_TAG)
            {
                OnAimingAtCamera?.Invoke(true);
            }else
            {
                OnAimingAtCamera?.Invoke(false);
            }
            MouseWorldPos = hit.point;
        }

        if (_inputs.AimPressed)
        {
            Vector3 WorldAimTarget = MouseWorldPos;
            WorldAimTarget.y = transform.position.y;
            Vector3 AimDirection = (WorldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, AimDirection, Time.deltaTime * 20f);
            _aimCamera.gameObject.SetActive(true);
            _movementInput.SetSensitivity(_aimSensitivity);
            _movementInput.SetRotateOnMove(false);
        }
        else
        {
            _movementInput.SetRotateOnMove(true);
            _aimCamera.gameObject.SetActive(false);
            _movementInput.SetSensitivity(_normalSensitivity);
        }
    }

    private void Reloading()
    {
        if (_inputs.ReloadPressed && hasGun)
        {
            if (_levelManager.BulletCount < 5)
            {
                _inventoryManager.UseBulletItem();
            }
            _inputs.ReloadPressed = false;
        }
    }

    private void Shooting()
    {

        if (_inputs.ShootPressed)
        {
            _gun.Shoot();
            _inputs.ShootPressed = false;
        }
    }
}
