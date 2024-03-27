using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkRay : Gun
{
    bool shrink = true;
    float swapTime = 0;
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
        if (shrink)
            b.GetComponent<Projectile>().Initialize(0, 100, 2, 0, Shrink); // version with special effect
        else
            b.GetComponent<Projectile>().Initialize(0, 100, 2, 0, Grow);

        anim.SetTrigger("shoot");
        swapTime = 0;
        elapsed = 0;
        ammo -= 1;

        return true;
    }

    public override bool AttemptAltFire()
    {
        if (!base.AttemptFire() || elapsed < swapTime)
            return false;

        swapTime = .5f;
        shrink = !shrink;

        blaster.materials[2].color = shrink? shrinkMat.color : growMat.color;

        return true;
    }

    void Shrink(HitData data)
    {
        Vector3 impactLocation = data.location;

        if (data.target.gameObject.layer == 7)
        {
            data.target.gameObject.GetComponentInParent<Rigidbody>().transform.localScale *= .99f;
        }
    }

    void Grow(HitData data)
    {
        Vector3 impactLocation = data.location;

        if (data.target.gameObject.layer == 7)
        {
            data.target.gameObject.GetComponentInParent<Rigidbody>().transform.localScale *= 1.01f;
        }
    }
}
