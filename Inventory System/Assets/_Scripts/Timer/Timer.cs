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

    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Image _timerImage;

    private Coroutine _timerCorutine; //my co-routine
    LevelManager _levelManager;
    public static Timer Instance;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _timerImage.fillAmount = _timer / _timer;
        StartTimer();
        _levelManager = LevelManager.instance;
    }
    private IEnumerator MissionTimer()
    {
        //Subtracts 1 every second and the timer value is convered in minutes and seconds
        while (_timer > 0)
        {
            yield return new WaitForSeconds(1f);
            _timer -= 1;
            float minutes = Mathf.FloorToInt(_timer / 60);
            float seconds = Mathf.FloorToInt(_timer % 60);
            _timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
            _timerImage.fillAmount = _timer / _timerMax;
        }
        if (_timer < 1)
        {
            TimeOver();
        }
    }
    private void TimeOver()
    {
        _levelManager.GameOver();
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

    public void StartTimer()
    {
        _timerCorutine = StartCoroutine(MissionTimer());
    }

    public void PauseTimer()
    {
        StopCoroutine(_timerCorutine);
    }

  

}
