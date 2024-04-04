using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkRay : Gun
{
    bool shrink = true;
    float swapTime = 0;
    float tParticles = 0;
    [SerializeField]
    ParticleSystem rayParticles;

    [SerializeField]
    MeshRenderer blaster;

    [SerializeField]
    Material shrinkMat;
    [SerializeField]
    Material growMat;

    public override bool AttemptFire()
    {
        if (!base.AttemptFire() || elapsed < swapTime)
            return false;

        var b = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
        var main = rayParticles.main;

        if (shrink)
        {
            b.GetComponent<Projectile>().Initialize(0, 100, 2, 0, Shrink);
            main.startColor = Color.blue;
        }
        else
        {
            b.GetComponent<Projectile>().Initialize(0, 100, 2, 0, Grow);
            main.startColor = Color.red;
        }

        
        if (tParticles > .2f) 
        {
            //not the best way to implement this, but trying my best to work within the constraints set
            //i could have tested for when the KeyUp, to turn off a Looping animation, or something like that, but it would've required adding more listening code that should stay in the FPSController
            rayParticles.Emit(1);
            rayParticles.Play();
            tParticles = 0;
        }
        tParticles += elapsed;
        anim.SetTrigger("shoot");
        swapTime = 0;
        elapsed = 0;
        ammo -= 1;

        return true;
    }

    public override bool AttemptAltFire()
    {
        if (elapsed < swapTime)
            return false;

        anim.SetTrigger("swap");

        swapTime = .5f;
        elapsed = 0;
        shrink = !shrink;

        return true;
    }

    void Shrink(HitData data)
    {
        Vector3 impactLocation = data.location;

        if (data.target.gameObject.GetComponentInParent<Rigidbody>() != null)
        {
            data.target.gameObject.GetComponentInParent<Rigidbody>().transform.localScale *= .99f;
        }
    }

    void Grow(HitData data)
    {
        Vector3 impactLocation = data.location;

        if (data.target.gameObject.GetComponentInParent<Rigidbody>()!= null)
        {
            data.target.gameObject.GetComponentInParent<Rigidbody>().transform.localScale *= 1.01f;
        }
    }


    public void SwapColor()
    {
        blaster.materials[2].color = shrink ? shrinkMat.color : growMat.color;
    }
}
