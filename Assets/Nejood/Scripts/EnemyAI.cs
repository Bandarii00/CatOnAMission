using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform patrolTarget;
    public float stoppingDistance = 1.5f;
    // public Animator animator;
    public string hideAnimationTrigger = "FoundHidingPlayer";

    private NavMeshAgent agent;
    private GameObject player;
    private bool isChasing = false;
    private bool isPlayerHiding = false;
    private bool isWaitingAfterHide = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoToPatrolTarget();
    }

    void Update()
    {
        if (isWaitingAfterHide) return;

        if (player != null)
        {
            isPlayerHiding = IsPlayerInHiding();

            if (isPlayerHiding)
            {
                if (isChasing)
                {
                    StopChasingAndReact();
                }
            }
            else if (isChasing)
            {
                agent.SetDestination(player.transform.position);
                // animator.SetBool("IsChasing", true);
            }
        }

        if (!isChasing)
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        // animator.SetBool("IsChasing", false);
        if (Vector3.Distance(transform.position, patrolTarget.position) > stoppingDistance)
        {
            agent.SetDestination(patrolTarget.position);
        }
    }

    private void GoToPatrolTarget()
    {
        agent.SetDestination(patrolTarget.position);
    }

    private void StopChasingAndReact()
    {
        isChasing = false;
        isWaitingAfterHide = true;
        agent.ResetPath();
        // animator.SetTrigger(hideAnimationTrigger);
        // animator.SetBool("IsChasing", false);
        StartCoroutine(ResumeAfterHide());
    }

    private System.Collections.IEnumerator ResumeAfterHide()
    {
        yield return new WaitForSeconds(3f); 
        isWaitingAfterHide = false;
        GoToPatrolTarget();
    }

    private bool IsPlayerInHiding()
    {
        Collider[] hits = Physics.OverlapSphere(player.transform.position, 0.2f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("HidingSpot"))
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            isChasing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
            player = null;
        }
    }
}
