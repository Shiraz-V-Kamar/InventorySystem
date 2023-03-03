using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class Gun : MonoBehaviour
{
    private GameObject _bulletParticleSystem;
    private ParticleSystem _impactParticleSystem;
    private TrailRenderer _bulletTrail;

    private int _shootDistance;
    private float _lastShootTime;

    [SerializeField] bool _addBulletSpread = false;
    [SerializeField] private Vector3 _bulletSpreadVariant = new Vector3(0.02f, 0.02f, 0.02f);
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private float shootDelay = .5f;
    [SerializeField] private LayerMask _shootLayerMask;
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] float ImpactoffsetVal = 0;
    [SerializeField] private ParticleSystem _shootingSystem;
    [SerializeField] private WeaponScriptableObject _weaponScriptableObject;

    private LevelManager _levelManager;
    //AudioManager _audioManager;

    public Action<bool> OnGettingTargetInAim;

    private void Start()
    {
        //_audioManager = AudioManager.instance;
        _levelManager = LevelManager.instance;
        SetupWeaponDetails(_weaponScriptableObject);
    }
    private void SetupWeaponDetails(WeaponScriptableObject element)
    {
        _bulletParticleSystem = element.BulletParticleSystem;
        _impactParticleSystem = element.ImpactParticleSystem;
        _bulletTrail = element.BulletTrail;
        _shootDistance = element.ShootDistance;
    }
    public void Shoot()
    {

        Vector3 direction = GetDirection();
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        RaycastHit hit;

        // Checks to see if there are boject which can be shot in the layer
        if (Physics.Raycast(ray, out hit, _shootDistance, _shootLayerMask))
        {
            if (_lastShootTime + shootDelay < Time.time && _levelManager.BulletCount > 0)
            {
                _shootingSystem.Play();
                // Play the sound
                //_audioManager.PlaySound("Element Projectile");
                _levelManager.DecrementBulletCount();

                // Instantiates a trail and particle which moves towards hit point
                TrailRenderer trail = Instantiate(_bulletTrail, _bulletSpawnPoint.position, Quaternion.identity);
                trail.enabled = false;
                GameObject trailParticle = Instantiate(_bulletParticleSystem, _bulletSpawnPoint.position, Quaternion.identity, trail.transform.parent);
                trailParticle.transform.localPosition = Vector3.zero;

                StartCoroutine(SpawnTrail(trail, hit, trailParticle));
                // Damage the hit item 
                _lastShootTime = Time.time;
            }
        }
                OnGettingTargetInAim?.Invoke(false);
    }

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
        
        // if the item hit is cctv camera play imapct particle system and destory after dureation
        if (hit.collider.tag == Helper.CAMERA_TAG)
        {
            OnGettingTargetInAim?.Invoke(true);
            // Destroyed particle system
            HandleStreetCam cam = hit.collider.GetComponent<HandleStreetCam>();
            cam.DealDamage();
            //PlayImapctSound
            Instantiate(_impactParticleSystem, offsetPos, Quaternion.LookRotation(hit.normal));
        }
        Destroy(trail.gameObject, trail.time);
        Destroy(trailParticle.gameObject, trail.time);
    }


    // To randomize the gun bullet spread
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
