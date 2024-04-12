using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EnemyMovementStates
{
    Wander,
    Pursue,
    Attack,
    Recovery,
    Disabled
}

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyMovementStates moveState;
    [SerializeField] float wanderRange;
    [SerializeField] float pursueRange;
    [SerializeField] float attackRange;
    [SerializeField] float recoveryTime;
    [SerializeField] float attackMagnitude;

    [SerializeField] GameObject Damager;
    public Rigidbody Rigidbody { get; private set; }
    NavMeshAgent agent;
    Vector3 origin;
    FPSController player;

    float elapsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        origin = transform.position;
        player = FindFirstObjectByType<FPSController>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        switch (moveState)
        {
            case EnemyMovementStates.Wander:
                Wander();
                break;
            case EnemyMovementStates.Pursue:
                Pursue();
                break;
            case EnemyMovementStates.Attack:
                Attack();
                break;
            case EnemyMovementStates.Recovery:
                Recovery();
                break;
            case EnemyMovementStates.Disabled:
                ReEnableAI();
                break;
        }

    }

    void ChangeState(EnemyMovementStates _moveState, float time)
    {
        elapsed = time;
        moveState = _moveState;
    }
    bool CheckRange(Vector3 newPos, Vector3 currPos, float range)
    {
        if (Vector3.Magnitude(newPos - currPos) < range)
        {
            return true;
        }
        return false;
    }
    void Wander()
    {
        if (CheckRange(player.transform.position, agent.transform.position, pursueRange))
        {
            ChangeState(EnemyMovementStates.Pursue, 0);
            return;
        }
        if (elapsed >= recoveryTime)
        {
            float randX = Random.Range(-wanderRange, +wanderRange);
            float randZ = Random.Range(-wanderRange, +wanderRange);
            Vector3 wanderPos = new Vector3(origin.x + randX, origin.y, origin.z + randZ);
            NavMeshHit hit;
            bool gotHit = NavMesh.SamplePosition(wanderPos, out hit, 2, NavMesh.AllAreas);
            if (gotHit)
            {
                agent.SetDestination(hit.position);
                elapsed = 0;
            }
        }
        
    }
    void Pursue()
    {
        Vector3 playerPos = player.transform.position;
        if (CheckRange(playerPos, agent.transform.position, attackRange))
        {
            ChangeState(EnemyMovementStates.Attack, 0);
            return;
        }
        if (CheckRange(playerPos, agent.transform.position, pursueRange))
        {
            agent.SetDestination(playerPos);
        }
        else
        {
            ChangeState(EnemyMovementStates.Wander, recoveryTime);
        }
        
    }
    void Attack()
    {
        //attack
        agent.enabled = false;
        Rigidbody.isKinematic = false;
        Damager.SetActive(true);

        Rigidbody.AddForce((player.transform.position - agent.transform.position).normalized * attackMagnitude, ForceMode.VelocityChange);


        ChangeState(EnemyMovementStates.Recovery, 0);
    }
    void Recovery()
    {
        if (elapsed > recoveryTime/2)
        {
            bool isSpawnable = NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, gameObject.transform.localScale.y, NavMesh.AllAreas);
            if (isSpawnable)
            {
                agent.enabled = true;
                Rigidbody.isKinematic = true;
                Damager.SetActive(false);
            }
            else { return; }
        }
        if (elapsed >= recoveryTime && agent.enabled)
        {
            ChangeState(EnemyMovementStates.Wander, recoveryTime);
        }
    }

    void ReEnableAI()
    {
        Rigidbody.isKinematic = false;
        if (elapsed > .1f)
        {
            if (Rigidbody.velocity.magnitude <= 0f && 
                Rigidbody.constraints != RigidbodyConstraints.FreezeAll)
            {
                //enable the AI
                ChangeState(EnemyMovementStates.Recovery, recoveryTime);
                Debug.Log("Nav Agent Re-enabled");
                return;
            }
        }
        if (elapsed > 7f)
        {
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            Rigidbody.useGravity = true;
            ResetTime();
            return;
        }
    }
    public void DisableAI()
    {
        if (moveState != EnemyMovementStates.Disabled)
        {
            //disable the AI
            agent.enabled = false;
            Rigidbody.isKinematic = false;
            Debug.Log("Nav Agent Disabled");
            ChangeState(EnemyMovementStates.Disabled, 0);
            return;
        }
        ResetTime();
    }

    public void ResetTime()
    {
        elapsed = 0;
    }
    public void ApplyKnockback(Vector3 knockback)
    {
        GetComponent<Rigidbody>().AddForce(knockback, ForceMode.Impulse);
    }

    public void Respawn()
    {
        transform.position = origin;
    }
}
