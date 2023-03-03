using UnityEngine;

public class HandleStreetCam : MonoBehaviour
{
    private float health = 10;
    [SerializeField] private ParticleSystem _fireParticleSystem;

    private bool _isCameraDamaged = false;

    LevelManager levelManager;
    Timer timer;

    private void Start()
    {
        levelManager = LevelManager.instance;
        timer = Timer.Instance;
    }
    public void DealDamage()
    {
        health -= 5;
        Debug.Log("Dealing Damage");
    }

    private void Update()
    {
        if (health < 1 && _isCameraDamaged == false)
        {
            health = 0;
            _fireParticleSystem.Play();
            _isCameraDamaged = true;
            levelManager.AddCameraDestroyedCount();
            timer.AddTime();
        }
    }
}