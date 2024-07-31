using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// gun base class
public class Gun : MonoBehaviour
{
    protected FPSController player;

    // references
    [SerializeField] protected Transform gunBarrelEnd;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Animator anim;

    // stats
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected float timeBetweenShots = 0.1f;
    [SerializeField] protected bool isAutomatic = false;

    // private variables
    protected int ammo;
    public int Ammo {  
        get { return ammo; } 
        set { ammo = (value < 0) ? 0 : value;
            AmmoChange.Invoke(ammo, maxAmmo); } }
    protected float elapsed = 0;

    UnityAction<int, int> AmmoChange;

    // Start is called before the first frame update
    void Start()
    {
        AmmoChange += FindObjectOfType<PlayerHUD>().SetAmmo;

        ammo = maxAmmo;
        AmmoChange.Invoke(ammo, maxAmmo);
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;

        // cheat code to refill ammo
        if (Input.GetKeyDown(KeyCode.R))
        {
            AddAmmo(999);
            AmmoChange.Invoke(ammo, maxAmmo);
        }
    }

    public virtual void Equip(FPSController p)
    {
        player = p;
        AmmoChange.Invoke(ammo, maxAmmo);
    }

    public virtual void Unequip() { }

    public bool AttemptAutomaticFire()
    {
        if (!isAutomatic)
            return false;

        return true;
    }

    public virtual bool AttemptFire()
    {
        if (ammo <= 0)
        {
            return false;
        }

        if(elapsed < timeBetweenShots)
        {
            return false;
        }

        AmmoChange.Invoke(ammo-1, maxAmmo);
        FindObjectOfType<GunAim>().Aim();
        return true;
    }

    public virtual bool AttemptAltFire()
    {
        return false;
    }

    public virtual void AddAmmo(int amount)
    {
        ammo += amount;

        if (ammo > maxAmmo)
            ammo = maxAmmo;
    }
}
