using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Physgun : Gun
{
    [SerializeField]
    LineRenderer lineRenderer;
    LineRenderer line;

    bool holdingSomething = false;
    Rigidbody heldObject;
    [SerializeField]
    GameObject locationTarget;

    public override bool AttemptFire()
    {
        if (!base.AttemptFire())
            return false;

        if (holdingSomething)
        {
            //stop holding
            heldObject.useGravity = true;
            heldObject.constraints = RigidbodyConstraints.FreezeRotation;

            holdingSomething = false;
            Destroy(line.gameObject);
            //only want elapsed to restart when releasing, that way you can pick up and release quickly
            elapsed = 0;
        }
        else
        {
            var b = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
            b.GetComponent<Projectile>().Initialize(1, 90, 2f, 1, OnHit);

        }

        anim.SetTrigger("shoot");
        ammo -= 1;

        return true;
    }
    public override bool AttemptAltFire()
    {
        if (!base.AttemptFire() || !holdingSomething)
            return false;

        //stop holding without reenabling gravity

        heldObject.constraints = RigidbodyConstraints.FreezeAll;
        holdingSomething = false;
        Destroy(line.gameObject);

        elapsed = 0;
        anim.SetTrigger("shoot");
        ammo -= 1;

        return true;
    }

    public override void Unequip() 
    {
        base.Unequip();
        if (holdingSomething)
        {
            StopAllCoroutines();
            Destroy(line.gameObject);
            //stop holding

            heldObject.useGravity = true;
            heldObject.constraints = RigidbodyConstraints.FreezeRotation;

            holdingSomething = false;

            
        }
    }


    void OnHit(HitData data)
    {
       if (data.target.GetComponentInParent<Rigidbody>() != null)
        {
            Debug.Log("hit smth");
            holdingSomething = true;

            //location for the locationTarget
            Vector3 loc = gunBarrelEnd.transform.position + gunBarrelEnd.transform.forward * Vector3.Distance(gunBarrelEnd.transform.position, data.location);
            //where the heldObject should be trying to get to in the coroutine
            locationTarget.transform.position = loc;

            heldObject = data.target.gameObject.GetComponentInParent<Rigidbody>();
            heldObject.constraints = RigidbodyConstraints.FreezeRotation;
            heldObject.useGravity = false;

            line = Instantiate(lineRenderer, gunBarrelEnd.transform);
            StartCoroutine(MoveObjectCoroutine());
        }
    }

    IEnumerator MoveObjectCoroutine()
    {
        while (holdingSomething)
        {
            Vector3 currPos = heldObject.transform.position;
            Vector3 target = locationTarget.transform.position;
            float strength = Vector3.Distance(currPos, target)*5f;
            Debug.Log(strength);

            //heldObject.position = Vector3.MoveTowards(currPos, target, strength*Time.deltaTime);
            Vector3 dir = target - currPos;
            dir.Normalize();
            heldObject.velocity = strength * dir;

            Vector3[] positions = {
                gunBarrelEnd.transform.position,
                currPos
            };
            line.SetPositions(positions);
            yield return null;
        }
    }
}
