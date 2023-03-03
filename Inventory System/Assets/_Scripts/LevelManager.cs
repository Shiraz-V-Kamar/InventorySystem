using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private float _camerasDestroyedCount;
    private float _camerasToDestroyedCount;

    public int BulletCount { get; private set; }
    private int MaxBulletCount = 5;

    private bool _isHoldingGun; 
    private bool _isGameOver;
    private bool isInventoryOpen;
    private bool isGamePaused;
    private bool _gameWon;
    private bool _changeCrosshairColor;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _bulletCountText;
    [SerializeField] private TextMeshProUGUI _camerasToDestroyedCountText;
    [SerializeField] private TextMeshProUGUI _gameOverCameraDestroyedCountText;
    [SerializeField] private TextMeshProUGUI _currentItemText;
    [SerializeField] private TextMeshProUGUI _helperText;
    [SerializeField] private Image _crosshairImage;
    [SerializeField] private Image _currentItemImage;
    [SerializeField] private Sprite _noItemImage;

    [Header("All Panels")]
    [SerializeField] private GameObject[] _allPanel;

    [Header("Scripts")]
    [SerializeField] PlayerHandleItem _playerHandlItem;
    [SerializeField] PlayerShoot _playerShoot;
    InventoryManager _inventoryManager;
    InputsManager _inputs;

    [SerializeField] private GameObject[] _cameraObjs;

    [Header("Items")]
    [SerializeField]private BulletScriptableObject bulletScriptableObject;
    [SerializeField]private WeaponScriptableObject weaponScriptableObject;
    [SerializeField]private MedicKitScriptableObject medicKitScriptableObject;
    public enum GameStates
    {
        InGame,
        Pause,
        Inventory,
        GameOver,
        GameWon
    }

    public ItemType _currentItemType;
    public GameStates CurrentState;

    public static LevelManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _inputs = InputsManager.instance;
        _inventoryManager = InventoryManager.Instance;

        //reseting timescale after Gamover / GameWon / Pause 
        Time.timeScale = 1;

        _playerHandlItem.OnHoldingGun += PlayerIsHoldingGun;
        _playerShoot.OnAimingAtCamera += ChangeCrosshairColor;

        // Enum helps changing between panels easier
        CurrentState = GameStates.InGame;
        _camerasToDestroyedCount = _cameraObjs.Length;
    }
    private void OnDisable()
    {
        _playerHandlItem.OnHoldingGun -= PlayerIsHoldingGun;
        _playerShoot.OnAimingAtCamera -= ChangeCrosshairColor;
    }
    private void Update()
    {
        SetUITexts();
        EnableAndDisableCrosshair();

        // Game Win Condition
        if (_camerasToDestroyedCount < 1)
        {
            GameCompleted();
        }

        // Player Inputs
        if (!_isGameOver && !_gameWon)
        {

            if (_inputs.OpenInventoryPressed)
            {
                OpenInventory();
                _inputs.OpenInventoryPressed = false;
            }

            if (_inputs.PausePressed)
            {
                PauseGame();
                _inputs.PausePressed = false;
            }
            Cursor.lockState = CursorLockMode.Locked;
            _inputs.cursorInputForLook = true;
        }
        else if (_isGameOver || _gameWon )
        {
            _inputs.cursorInputForLook = false;
            Cursor.lockState = CursorLockMode.None;
        }

        ToggleUIPanel(CurrentState);

        // helps the player to see what the current item is 
        SetCurrentItemImage();
    }

    private void SetCurrentItemImage()
    {
        // Depending on the itemtype the image,name changes
        ItemScriptableObject currentItem= _inventoryManager.GetSelectedItem();
        if (currentItem != null)
        {
            _currentItemType = currentItem.Type;

            switch (_currentItemType)
            {
                case ItemType.Gun:
                    {
                        _currentItemImage.sprite = weaponScriptableObject.Image;
                        _currentItemText.text = weaponScriptableObject.Name;
                        break;
                    }
                case ItemType.Bullets:
                    {
                        _currentItemImage.sprite = bulletScriptableObject.Image;
                        _currentItemText.text = bulletScriptableObject.Name;
                        break;
                    }
                case ItemType.MedicKit:
                    {
                        _currentItemImage.sprite = medicKitScriptableObject.Image;
                        _currentItemText.text = medicKitScriptableObject.Name;
                        break;
                    }
            }
        }
        else
        {
            _currentItemImage.sprite = _noItemImage;
            _currentItemText.text = "";
        }
    }

    private void PlayerIsHoldingGun(bool obj)
    {
        _isHoldingGun = obj;
    }

    private void ChangeCrosshairColor(bool obj)
    {
        _changeCrosshairColor = obj;
    }

    public void SetBulletToMax()
    {
        BulletCount = MaxBulletCount;
    }

    public void DecrementBulletCount()
    {
        BulletCount--;
    }

    public void AddCameraDestroyedCount()
    {
        _camerasDestroyedCount++;
        _camerasToDestroyedCount--;
    }


    private void ToggleUIPanel(GameStates state)
    {
        // Disables all the panels
        foreach (var panel in _allPanel)
        {
            panel.SetActive(false);
        }

        // Enables the appropriate panel
        if (state == GameStates.Pause)
        {
            _allPanel[(int)state].SetActive(isGamePaused);
            _inputs.cursorInputForLook = false;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else if (state == GameStates.Inventory)
        {
            _allPanel[(int)state].SetActive(isInventoryOpen);
        }
        else
        {
            _allPanel[(int)state].SetActive(true);
        }
    }


    private void PauseGame()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            CurrentState = GameStates.Pause;
        }
        else
        {
            Time.timeScale = 1;
            CurrentState = GameStates.InGame;
        }

    }

    private void SetUITexts()
    {
        _bulletCountText.text = BulletCount.ToString();
        if(BulletCount<1)
        {
            _bulletCountText.color = Color.red;
            _helperText.text = "Reload";
        }
        else
        {
            _bulletCountText.color = Color.white;
            _helperText.text = "";
        }
       
        _camerasToDestroyedCountText.text = _camerasToDestroyedCount.ToString();
        _gameOverCameraDestroyedCountText.text = _camerasDestroyedCount.ToString();
    }

    private void EnableAndDisableCrosshair()
    {
        if (_isHoldingGun)
        {
            if (_changeCrosshairColor)
            {
                _crosshairImage.color = Color.red;
            }
            else
            {
                _crosshairImage.color = Color.white;
            }
            _crosshairImage.enabled = true;
        }
        else
        {
            _crosshairImage.enabled = false;
        }
    }

    private void OpenInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        if (isInventoryOpen)
        {
            CurrentState = GameStates.Inventory;
        }
        else
        {
            CurrentState = GameStates.InGame;
        }
    }
    private void GameCompleted()
    {
        _gameWon = true;
        CurrentState = GameStates.GameWon;
        Time.timeScale = 0;
    }
    public void GameOver()
    {
        CurrentState = GameStates.GameOver;
        _isGameOver = true;
        Time.timeScale = 0;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
