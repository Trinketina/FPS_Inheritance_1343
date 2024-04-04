using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrapplingHook : Gun
{

    bool reeledIn = true;
    

    public override bool AttemptFire()
    {
        if (!base.AttemptFire() || !reeledIn)
            return false;

        var b = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
        b.GetComponent<Projectile>().Initialize(0, 100, 2, 0, GrappleShot);


        anim.SetTrigger("shoot");
        elapsed = 0;
        ammo -= 1;

        return true;
    }

    void GrappleShot(HitData data)
    {
        CharacterController player = this.GetComponentInParent<CharacterController>();

        StartCoroutine(GrappleCoroutine(player,player.transform.position,data.location));
    }

    IEnumerator GrappleCoroutine(CharacterController player, Vector3 origin, Vector3 newPos)
    {
        float reelTime = 0;
        reeledIn = false;
        while (reelTime < .48f)
        {
            player.transform.position = Vector3.Lerp(origin, newPos, reelTime*2);

            reelTime += Time.deltaTime;
            yield return null;
        }

        reeledIn = true;
    }
}
