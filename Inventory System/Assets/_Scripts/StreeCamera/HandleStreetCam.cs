using UnityEngine;

public class HandleStreetCam : MonoBehaviour
{
    private float health = 10;
    private bool _isCameraDamaged = false;
    [SerializeField] private ParticleSystem _fireParticleSystem;

    [SerializeField] private Outline _outline;

    Timer timer;
    LevelManager levelManager;
    private void Start()
    {
        levelManager = LevelManager.instance;
        timer = Timer.Instance;
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
        if (health < 1 && _isCameraDamaged == false)
        {
            health = 0;
            _fireParticleSystem.Play();
            _isCameraDamaged = true;
            levelManager.AddCameraDestroyedCount();
            timer.AddTime();
            _outline.OutlineWidth = 0;
        }
    }
}
