using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class NurseReset : MonoBehaviour
{
    private InteractionController interactionController;

    private GameObject currentInteractionObject;

    [SerializeField] float waitTime = 2f;

    Vector3 originalPosition;

    private NavMeshAgent agent;

    private bool isResetting = false;


    private void Awake()
    {
        interactionController = FindFirstObjectByType<InteractionController>();

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        
    }

    public void NurseCoroutine()
    {
        StartCoroutine(NurseBehaviour(currentInteractionObject));
    }

    private IEnumerator NurseBehaviour(GameObject target)
    {   
        isResetting = true;

        agent.SetDestination(target.transform.position);

        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        yield return new WaitForSeconds(waitTime);

        //Reset object
        //target.Reset();
        
        agent.SetDestination(originalPosition);

        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

        isResetting = false;
    }
}
