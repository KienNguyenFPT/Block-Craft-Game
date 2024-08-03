using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    private Transform target;
    private NavMeshAgent agent;
    private float timer;
    private Animator animator;
    private bool isWandering = false;

    private NPCStats npcStats;
    private float attackCooldown = 0;
    private static readonly int VelocityHash = Animator.StringToHash("Velocity");
    private static readonly int VelocityXHash = Animator.StringToHash("X");
    private static readonly int VelocityYHash = Animator.StringToHash("Y");
    private static readonly int IdleState = Animator.StringToHash("IdleState");
    private float velo = 0.0f;
    private readonly string[] attackTriggers = { "Attack1", "Attack2", "Attack3", "Buff", "Hit" };
    private readonly string[] idleState = { "Idle", "Eat", "Sit", "Sleep" };
    private bool isIdle = false;
    private float idleWaitTime = 5.0f;
    private float idleTimer = 0f;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        npcStats = GetComponent<NPCStats>();
        animator = GetComponent<Animator>();

        if (agent == null || npcStats == null || animator == null)
        {
            Debug.LogError("Missing required component on NPC.");
            return;
        }

        timer = Random.Range(npcStats.wanderTimer, npcStats.wanderTimer * 5);
        StartCoroutine(Wander());
        animator.SetFloat(VelocityHash, velo);
    }

    void Update()
    {
        if (animator == null || agent == null) return;

        float currentVelocity = agent.velocity.magnitude;
        animator.SetFloat(VelocityHash, currentVelocity);

        Vector3 currentVelocityVector = agent.velocity;
        float velX = currentVelocityVector.normalized.x;
        float velY = currentVelocityVector.normalized.z;

        animator.SetFloat(VelocityXHash, velX);
        animator.SetFloat(VelocityYHash, velY);

        if (!isWandering && !isIdle)
        {
            if (target == null)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleWaitTime)
                {
                    float randomIndex = Random.Range(0, idleState.Length);
                    animator.SetFloat(IdleState, randomIndex);
                    idleTimer = 0f;
                }
            }
            else
            {
                animator.SetFloat(IdleState, 0);
                isIdle = true;
            }
        }

        if (target != null && agent.enabled && !npcStats.isDied)
        {
            GuyStats targetStats = target.GetComponent<GuyStats>();
            if (targetStats != null && targetStats.IsDied)
            {
                isIdle = false;
                target = null;
                return;
            }

            agent.SetDestination(target.position);
            if (agent.remainingDistance <= npcStats.attackDistance && !agent.pathPending)
            {
                if (attackCooldown <= 0f)
                {
                    AttackTarget();
                }
                else
                {
                    attackCooldown -= Time.deltaTime;
                }
            }
        }
    }

    void AttackTarget()
    {
        if (animator == null) return;

        int randomIndex = Random.Range(0, attackTriggers.Length);
        string randomAttackTrigger = attackTriggers[randomIndex];

        animator.SetTrigger(randomAttackTrigger);
        if (Vector3.Distance(transform.position, target.position) <= npcStats.attackDistance)
        {
            var targetStats = target.GetComponent<GuyStats>();
            if (targetStats != null)
            {
                targetStats.TakeDamage(npcStats.attackPoint);
            }
        }
        attackCooldown = npcStats.attackSpeed;
    }

    IEnumerator Wander()
    {
        while (target == null)
        {
            timer = npcStats.wanderTimer;

            while (true)
            {
                Vector3 newPos = RandomNavSphere(transform.position, npcStats.wanderRadius, -1);
                NavMeshHit hit;
                if (NavMesh.SamplePosition(newPos, out hit, 1.0f, NavMesh.AllAreas))
                {
                    newPos = hit.position;
                    break;
                }

                if (agent.enabled)
                {
                    agent.SetDestination(newPos);
                    isWandering = true;
                }
            }
            yield return new WaitForSeconds(timer);
            isWandering = false;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("NPCAttack") && other.CompareTag("Player"))
        {
            target = other.transform;
            timer = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameObject.CompareTag("NPCAttack") && other.CompareTag("Player"))
        {
            isIdle = false;
            target = null;
            idleTimer = 0f;
        }
    }
}
