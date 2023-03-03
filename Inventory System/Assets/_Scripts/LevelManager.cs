using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public int BulletCount { get; private set; }
    private int MaxBulletCount = 5;

    private bool _isHoldingGun;

    [Header("UI Elements")]
    [SerializeField]private TextMeshProUGUI _bulletCountText;
    [SerializeField]private TextMeshProUGUI _cameraDestroyedCountText;
    [SerializeField]private TextMeshProUGUI _gameOverCameraDestroyedCountText;
    [SerializeField] private Image _crosshairImage;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _inGamePanel;

    [SerializeField] PlayerHandleItem _playerHandlItem;
    InputsManager _inputsManager;
    //Camera Stuff
    private float _camerasDestroyedCount;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1;
        _playerHandlItem.OnHoldingGun += PlayerIsHoldingGun;
        _gameOverPanel.SetActive(false);
        _inputsManager = InputsManager.instance;
    }

    private void OnDisable()
    {
        _playerHandlItem.OnHoldingGun -= PlayerIsHoldingGun;
    }

    private void PlayerIsHoldingGun(bool obj)
    {
        _isHoldingGun = obj;
    }

    public void SetBulletCount()
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
    }

    private void Update()
    {
        _bulletCountText.text = BulletCount.ToString();

        if(_isHoldingGun)
        {
            _crosshairImage.enabled = true;
        }else
        {
            _crosshairImage.enabled = false;
        }

        _cameraDestroyedCountText.text = _camerasDestroyedCount.ToString();
        _gameOverCameraDestroyedCountText.text = _camerasDestroyedCount.ToString();


    }

    public void GameOver()
    {
        _inputsManager.cursorLocked = false;
        _inputsManager.cursorInputForLook = false;
        _inGamePanel.gameObject.SetActive(false);
        _gameOverPanel.gameObject.SetActive(true);
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
}
