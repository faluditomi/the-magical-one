using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using System;
using FMODUnity;
using FMOD.Studio;

public class NurseReset : MonoBehaviour
{

    [SerializeField] float waitTime = 2f;

    Vector3 originalPosition;

    private NavMeshAgent agent;

    private bool isResetting = false;

    private Stack<Tuple<GameObject, Vector3>> targets = new Stack<Tuple<GameObject, Vector3>>();

    private EventInstance nurseMovingInstance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        originalPosition = transform.position;
        AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.nurseBotMoving, gameObject);
        nurseMovingInstance = AudioManager.instance.Create3DEventInstance(FMODEvents.instance.nurseBotMoving, gameObject, GetComponent<Rigidbody>());
        nurseMovingInstance.setParameterByName("NursebotMoving", 0);
        nurseMovingInstance.start();

    }

    private void Update()
    {
        if(targets.Count > 0 && !isResetting)
        {
            StartCoroutine(NurseBehaviour());
        }
    }

    public void AddTarget(GameObject newTarget, Vector3 resetPosition)
    {
        targets.Push(new Tuple<GameObject, Vector3>(newTarget, resetPosition));
    }

    private IEnumerator NurseBehaviour()
    {
        isResetting = true;

        while(targets.Count > 0)
        {
            Tuple<GameObject, Vector3> resetObject = targets.Pop();
            agent.SetDestination(resetObject.Item2);
            nurseMovingInstance.setParameterByName("NursebotMoving", 1);

            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

            //turn on light
            nurseMovingInstance.setParameterByName("NursebotMoving", 0);
            yield return new WaitForSeconds(waitTime);

            resetObject.Item1.GetComponent<ResetController>().Reset();
            //turn off light

            yield return null;
        }

        agent.SetDestination(originalPosition);
        nurseMovingInstance.setParameterByName("NursebotMoving", 1);

        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
        nurseMovingInstance.setParameterByName("NursebotMoving", 0);

        isResetting = false;
    }
}
