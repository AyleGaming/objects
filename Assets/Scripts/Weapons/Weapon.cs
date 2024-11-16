using UnityEngine;

public class Weapon { 
    protected float damage;
    protected int ammo;
    protected float fireRate;

    protected Transform weaponTip;

    public virtual void Shoot()
    {
        if (HasAmmo())
        {
            ammo--;
        }
    }

    public virtual void Reload()
    {

    }

    public bool HasAmmo()
    {
        return ammo > 0;
    }

    public Weapon(Transform tip)
    {
        weaponTip = tip;
    }

}
