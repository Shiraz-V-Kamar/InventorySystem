using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class Gun : MonoBehaviour
{
    [SerializeField] bool _addBulletSpread = false;
    [SerializeField] private Vector3 _bulletSpreadVariant = new Vector3(0.02f, 0.02f, 0.02f);
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private float shootDelay = .5f;
    [SerializeField] private LayerMask _shootLayerMask;
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] float ImpactoffsetVal = 0;


    private int _shootDistance;
    private GameObject _bulletParticleSystem;
    private ParticleSystem _shootingSystem;
    private ParticleSystem _impactParticleSystem;
    private TrailRenderer _bulletTrail;
    private int bulletCount;


    private float _lastShootTime;

    public InputsManager _inputsManager;
    //AudioManager _audioManager;

    public Action OnShootDecreaseBulletCount;
    public Action<int> OnWallShot;
    public Action<bool> OnShootingAimAt;

    private void Start()
    {
        //_audioManager = AudioManager.instance;
        _inputsManager = InputsManager.instance;
        //playerCollisions.OnCollecting += SetBulletCount;
    }

    private void SetBulletCount(int obj)
    {
        bulletCount = obj;
    }

    public void Shoot(bool ShootPressed)
    {
        bool shootPressed = ShootPressed;
        if (!shootPressed)
            return;

        if (_lastShootTime + shootDelay < Time.time && bulletCount > 0)
        {
            OnShootDecreaseBulletCount?.Invoke();

            _shootingSystem.Play();
            Vector3 direction = GetDirection();
            Ray ray = Camera.main.ScreenPointToRay(_inputsManager.MousePos);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, float.MaxValue, _shootLayerMask) && shootPressed)
            {
                if (hit.collider.transform != null)
                {
                    // Play the sound
                    //_audioManager.PlaySound("Element Projectile");
                    OnWallShot?.Invoke(5);
                    bulletCount--;

                    TrailRenderer trail = Instantiate(_bulletTrail, _bulletSpawnPoint.position, Quaternion.identity);
                    trail.enabled = false;
                    GameObject trailParticle = Instantiate(_bulletParticleSystem, _bulletSpawnPoint.position, Quaternion.identity, trail.transform.parent);
                    trailParticle.transform.localPosition = Vector3.zero;

                    StartCoroutine(SpawnTrail(trail, hit, trailParticle));
                    // Damage the hit item 
                    _lastShootTime = Time.time;
                    OnShootingAimAt?.Invoke(true);

                }
            }
        }
    }

    /*private void SetupBulletDetails(ItemScriptableObject element)
    {
        _bulletParticleSystem = element.;
        _impactParticleSystem = element.;
        _bulletTrail = element.;
    }*/

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit, GameObject trailParticle)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            trailParticle.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        
        trail.transform.position = hit.point;

        Vector3 offsetPos = new Vector3(hit.point.x, hit.point.y, hit.point.z - ImpactoffsetVal);

        //PlayImapctSound

        Instantiate(_impactParticleSystem, offsetPos, Quaternion.LookRotation(hit.normal));


        Destroy(trail.gameObject, trail.time);
        Destroy(trailParticle.gameObject, trail.time);
        OnShootingAimAt?.Invoke(false);
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;

        if (_addBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-_bulletSpreadVariant.x, _bulletSpreadVariant.x),
                Random.Range(-_bulletSpreadVariant.y, _bulletSpreadVariant.y),
                Random.Range(-_bulletSpreadVariant.z, _bulletSpreadVariant.z)
                );

            direction.Normalize();
        }

        return direction;
    }


}
