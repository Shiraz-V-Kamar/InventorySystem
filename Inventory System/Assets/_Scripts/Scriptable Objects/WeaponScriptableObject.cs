using UnityEngine;

[CreateAssetMenu(menuName ="Scriptable Object/Weapon")]
public class WeaponScriptableObject : ItemScriptableObject
{
    public float FireRate;
    public int ShootDistance;

    [Header("Bullet Effects")]
    public ParticleSystem ImpactParticleSystem;
    public GameObject BulletParticleSystem;
    public TrailRenderer BulletTrail;
}
