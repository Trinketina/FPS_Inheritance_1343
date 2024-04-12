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
    float maxDist = 1;
    float MaxDist { get { return maxDist; } set { maxDist = (value > 5) ? value : 5; } }

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
            //starts to chase player if within range
            ChangeState(EnemyMovementStates.Pursue, 0);
            return;
        }
        if (elapsed >= recoveryTime)
        {
            //otherwise wander within a range
            float randX = Random.Range(-wanderRange, +wanderRange);
            float randZ = Random.Range(-wanderRange, +wanderRange);
            Vector3 wanderPos = new Vector3(origin.x + randX, origin.y, origin.z + randZ);

            NavMeshHit hit;
            maxDist = transform.localScale.y * 2;
            bool gotHit = NavMesh.SamplePosition(wanderPos, out hit, maxDist, NavMesh.AllAreas);
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
            //if close enough, attack
            ChangeState(EnemyMovementStates.Attack, 0);
            return;
        }
        if (CheckRange(playerPos, agent.transform.position, pursueRange))
        {
            //otherwise keep pursuing
            agent.SetDestination(playerPos);
        }
        else
        {
            //if out of range, switch to wander
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
        //Rigidbody.velocity += (player.transform.position - agent.transform.position).normalized * attackMagnitude;

        //finished attack, move to recovery
        ChangeState(EnemyMovementStates.Recovery, 0);
    }
    void Recovery()
    {
        if (elapsed > recoveryTime/2)
        {
            //check to make sure the agent is above working NavMesh before reenabling
            maxDist = transform.localScale.y * 2;
            bool isSpawnable = NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, MaxDist, NavMesh.AllAreas);
            if (isSpawnable)
            {
                agent.enabled = true;
                Rigidbody.isKinematic = true;
                Damager.SetActive(false);
            }
            else if (elapsed > recoveryTime*2) 
            {
                //if Enemy gets stuck and can't find a safe place to spawn, it respawns
                Respawn();
                return;
            } 
            else { return; }
        }
        if (elapsed >= recoveryTime && agent.enabled)
        {
            //if successfully reenabled, start to wander
            ChangeState(EnemyMovementStates.Wander, recoveryTime);
        }
    }

    void ReEnableAI()
    {
        //when disabled, it will try to reenable
        if (elapsed > .1f)
        {
            if (Rigidbody.velocity.magnitude <= 0f && 
                Rigidbody.constraints != RigidbodyConstraints.FreezeAll)
            {
                //enable the AI
                //enter the recovery state with full recoveryTime so it immediately tries to re-enable navAgent
                ChangeState(EnemyMovementStates.Recovery, recoveryTime);
                //Debug.Log("Nav Agent Re-enabled");
                return;
            }
        }
        if (elapsed > recoveryTime*2)
        {
            //this is for when the enemy has been frozen by the Physgun, allowing it to break free after double the length of recovery
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            Rigidbody.useGravity = true;
            ResetTime();
            return;
        }
    }
    public void DisableAI() //is called when hit by attack
    {
        if (moveState != EnemyMovementStates.Disabled)
        {
            //disable the AI
            agent.enabled = false;
            Rigidbody.isKinematic = false;
            //Debug.Log("Nav Agent Disabled");
            ChangeState(EnemyMovementStates.Disabled, 0);
            return;
        }
        //makes sure to reset the time after each attack, so it doesn't re-enable mid-additional-attack
        ResetTime();
    }

    public void ResetTime() // can be called by other functions, just in case
    {
        //mostly added specifically for the Physgun, so that it can reset the timer while in its Coroutine
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
