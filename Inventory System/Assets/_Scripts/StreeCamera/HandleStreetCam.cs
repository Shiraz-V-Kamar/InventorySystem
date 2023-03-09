using System;
using UnityEngine;

public class HandleStreetCam : MonoBehaviour
{
    private float health = 10;
    private bool _isCameraDamaged = false;
    [SerializeField]private float _timeBeforeRotation = 3f;
    [SerializeField] private ParticleSystem _fireParticleSystem;

    [SerializeField] private Outline _outline;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _camerOnFireSound;

    Timer timer;
    LevelManager levelManager;
    AudioManager audioManager;
    private void Start()
    {
        audioManager = AudioManager.instance;
        levelManager = LevelManager.instance;
        timer = Timer.Instance;
        _audioSource.Play();
    }
    public void DealDamage()
    {
        health -= 5;
    }

    private void Update()
    {
        // when damaging the camera play fire particle system
        // disable the outline or set outline width to '0'
        // add time to timer
        if (!_audioSource.isPlaying && _isCameraDamaged == false)
        {
            Invoke("PlayCameraSound", _timeBeforeRotation);
        }
        if (health < 1 && _isCameraDamaged == false)
        {
            health = 0;
            _fireParticleSystem.Play();
            _isCameraDamaged = true;
            levelManager.AddCameraDestroyedCount();
            timer.AddTime();
            _outline.OutlineWidth = 0;

            audioManager.PlaySound(Helper.GUN_SHOOT_IMPACT_SOUND);
            _audioSource.clip = _camerOnFireSound;
            _audioSource.volume = 0.1f;
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }

    private void PlayCameraSound()
    {
        _audioSource.Play();
    }
}
