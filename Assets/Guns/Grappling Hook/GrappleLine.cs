using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleLine : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;
    bool didHit = false;

    Vector3[] positions = new Vector3[2];
    GameObject barrel;
    GameObject bullet;


    public void Initialize(GameObject gunBarrelEnd, GameObject bullet)
    {
        this.barrel = gunBarrelEnd;
        this.bullet = bullet;
    }
    void Update()
    {
        if (bullet != null || didHit)
        {
            positions[0] = barrel.transform.position;
            positions[1] = bullet.transform.position;
            lineRenderer.SetPositions(positions);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Hooked()
    {
        didHit = true;
    }
}
