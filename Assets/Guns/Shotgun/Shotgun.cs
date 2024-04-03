using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shotgun : Gun
{

    public override bool AttemptFire()
    {
        if (!base.AttemptFire())
            return false;

        var rotation = gunBarrelEnd.rotation;
        rotation.x += Random.Range(-.1f, .1f);
        rotation.y += Random.Range(-.1f, .1f);
        var b1 = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, rotation);
        b1.GetComponent<Projectile>().Initialize(2, 100, .5f, 0, GrappleShot);

        rotation = gunBarrelEnd.rotation; //reinitializing it so rand x and y aren't compounded
        rotation.x += Random.Range(-.15f, .15f);
        rotation.y += Random.Range(-.15f, .15f);
        var b2 = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, rotation);
        b2.GetComponent<Projectile>().Initialize(2, 100, .5f, 0, GrappleShot);

        rotation = gunBarrelEnd.rotation; //reinitializing
        rotation.x += Random.Range(-.15f, .15f);
        rotation.y += Random.Range(-.15f, .15f);
        var b3 = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, rotation);
        b3.GetComponent<Projectile>().Initialize(2, 100, .5f, 0, GrappleShot);

        rotation = gunBarrelEnd.rotation; //reinitializing
        rotation.x += Random.Range(-.2f, .2f);
        rotation.y += Random.Range(-.2f, .2f);
        var b4 = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, rotation);
        b4.GetComponent<Projectile>().Initialize(2, 100, .5f, 0, GrappleShot);

        rotation = gunBarrelEnd.rotation; //reinitializing
        rotation.x += Random.Range(-.2f, .2f);
        rotation.y += Random.Range(-.2f, .2f);
        var b5 = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, rotation);
        b5.GetComponent<Projectile>().Initialize(2, 100, .5f, 0, GrappleShot);

        rotation = gunBarrelEnd.rotation; //reinitializing
        rotation.x += Random.Range(-.2f, .2f);
        rotation.y += Random.Range(-.2f, .2f);

        var b6 = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, rotation);
        b6.GetComponent<Projectile>().Initialize(2, 100, .5f, 0, GrappleShot);

        anim.SetTrigger("shoot");
        elapsed = 0;
        ammo -= 1;

        return true;
    }

    void GrappleShot(HitData data)
    {
        CharacterController player = this.GetComponentInParent<CharacterController>();

    }
}
