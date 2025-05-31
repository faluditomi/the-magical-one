using Unity.VisualScripting;
using UnityEngine;

public class LevitateBehaviour : MonoBehaviour
{

    private bool isLevitating;
    private float currentShuffleSpeed;
    private float currentCollectedRadius;
    private float collectionSpeed = 1f;
    private Transform currentDestination;
    private Vector3 currentJitterTarget;
    private Rigidbody myRigidbody;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(isLevitating)
        {
            // if(Vector3.Distance(transform.position, currentDestination.position) > 5f)
            // {
            //     Vector3 direction = (currentDestination.position - transform.position).normalized;
            //     myRigidbody.MovePosition(transform.position + direction * Time.fixedDeltaTime * collectionSpeed);
            // }
            // else
            // {
            //     Vector3 jitter = Random.insideUnitSphere * currentCollectedRadius * currentShuffleSpeed;
            //     myRigidbody.MovePosition(transform.position + jitter);
            // }
        }
    }

    public void StartLevitate(float currentShuffleSpeed, float currentCollectedRadius, Transform currentDestination)
    {
        GameManager.Instance().StartLevitating();
        isLevitating = true;
        this.currentShuffleSpeed = currentShuffleSpeed;
        this.currentCollectedRadius = currentCollectedRadius;
        this.currentDestination = currentDestination;
    }

    public void StopLevitate()
    {
        GameManager.Instance().StopLevitating();
        isLevitating = false;
        currentShuffleSpeed = 0f;
        currentCollectedRadius = 0f;
        currentDestination = null;
    }

}
