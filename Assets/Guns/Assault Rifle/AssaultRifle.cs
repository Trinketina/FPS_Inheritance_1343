using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AssaultRifle : Gun
{
    [SerializeField]
    ParticleSystem particles;


    [SerializeField]
    GameObject blood;

    public override bool AttemptFire()
    {
        if (!base.AttemptFire())
            return false;
        var b = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
        b.GetComponent<Projectile>().Initialize(1, 90, 2f, 1, OnHit);

        particles.Play();
        elapsed = 0;
        anim.speed = .3f; //slows the animation so that the next bullet interrupts it before the gun can fully recoil.
        anim.SetTrigger("shoot");
        ammo -= 1;

        return true;
    }

    void OnHit(HitData data)
    {
        var hit = Instantiate(blood, data.location, Quaternion.LookRotation(data.direction));
        Destroy(hit.gameObject,.5f);
    }
}
