using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private float _timer;
    [SerializeField] private float _timerMax;
    [SerializeField] private float _timerDecrement;
    [SerializeField] private float _timerIncrement;

    public static Timer Instance;
    private Coroutine _timerCorutine; //my co-routine
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Image _timerImage;

    LevelManager _levelManager;
    
    
    private void Awake()
    {
        Instance = this;
    }
    public void ReduceTime()
    {
        _timer -= _timerDecrement;
        if(_timer<0)
        {
            _timer = 0;
            TimeOver();
        }
    }


    public void AddTime()
    {
        _timer += _timerIncrement; 
        if(_timer>_timerMax)
        {
            _timer = _timerMax;
        }
    }

    private void Start()
    {
        _timerImage.fillAmount = _timer/_timer;
        StartTimer();
        _levelManager = LevelManager.instance;
    }
    public void StartTimer()
    {
        _timerCorutine = StartCoroutine(EnergyTimer());
    }

    private IEnumerator EnergyTimer()
    {
        while (_timer > 0)
        {
            yield return new WaitForSeconds(1f);
            _timer -= 1;
            float minutes = Mathf.FloorToInt(_timer / 60);
            float seconds = Mathf.FloorToInt(_timer % 60);
            _timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
            _timerImage.fillAmount = _timer / _timerMax;
        }
        if(_timer<1)
        {
            TimeOver();
        }
    }

    public void PauseTimer()
    {
        StopCoroutine(_timerCorutine);
    }

    private void TimeOver()
    {
        _levelManager.GameOver();
    }

}
