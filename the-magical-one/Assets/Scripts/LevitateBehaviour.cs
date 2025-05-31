using Unity.VisualScripting;
using UnityEngine;

public class LevitateBehaviour : MonoBehaviour
{

    private bool isLevitating;
    private float currentShuffleSpeed;
    private float currentCollectedRadius;
    private float collectionSpeed = 1f;
    private Transform currentDestination;
    private Vector3 currentShuffleOffset;
    private Rigidbody myRigidbody;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(isLevitating)
        {
            if(Vector3.Distance(transform.position, currentDestination.position) > currentCollectedRadius)
            {
                Vector3 direction = (currentDestination.position - transform.position).normalized;
                // myRigidbody.MovePosition(transform.position + direction * Time.fixedDeltaTime * collectionSpeed);
                myRigidbody.linearVelocity = direction * collectionSpeed;
            }
            else
            {
                Vector3 currentShuffleTarget = currentDestination.position + currentShuffleOffset;

                if(Vector3.Distance(transform.position, currentShuffleTarget) < 0.2f)
                {
                    myRigidbody.linearVelocity = Vector3.zero;
                    currentShuffleOffset = Random.insideUnitSphere * currentCollectedRadius;
                }

                // float step = currentShuffleSpeed * Time.fixedDeltaTime;
                // Vector3 newPosition = Vector3.MoveTowards(transform.position, currentShuffleTarget, step);
                // myRigidbody.MovePosition(newPosition);
                Vector3 direction = (currentShuffleTarget - transform.position).normalized;
                myRigidbody.linearVelocity = direction * currentShuffleSpeed;
            }
        }
    }

    public void StartLevitate(float currentShuffleSpeed, float currentCollectedRadius, Transform currentDestination)
    {
        GameManager.Instance().StartLevitating();
        isLevitating = true;
        this.currentShuffleSpeed = currentShuffleSpeed;
        this.currentDestination = currentDestination;
        this.currentCollectedRadius = currentCollectedRadius;
        currentShuffleOffset = Random.insideUnitSphere * currentCollectedRadius;
        myRigidbody.useGravity = false;
    }

    public void StopLevitate()
    {
        GameManager.Instance().StopLevitating();
        myRigidbody.linearVelocity = Vector3.zero;
        isLevitating = false;
        currentShuffleSpeed = 0f;
        currentDestination = null;
        currentCollectedRadius = 0f;
        currentShuffleOffset = Vector3.zero;
        myRigidbody.useGravity = true;
    }

}
