using UnityEngine;

public abstract class Weapon : ScriptableObject { 
    
    [SerializeField] public float damage;
    [SerializeField] public int ammo;
    [SerializeField] public float fireRate;
    [SerializeField] public Faction ammoFaction;
    [SerializeField] public float volume = 1.0f; // Volume for the sound
    [SerializeField] public AudioClip[] fireSounds; // Sound for firing

    public abstract void Shoot(Transform weaponTip);
    public abstract void Reload();

    public bool HasAmmo()
    {
        return ammo > 0;
    }

}
