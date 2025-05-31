using UnityEngine;

public class LevitateBehaviour : MonoBehaviour
{

    private bool isLevitating;
    private float currentShuffleSpeed;
    private float currentCollectedRadius;
    private float collectionSpeed = 10f;
    private Transform currentDestination;
    private Vector3 currentShuffleOffset;
    private Quaternion initialRotationOffset;
    private Rigidbody myRigidbody;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(isLevitating)
        {
            if(Vector3.Distance(transform.position, currentDestination.position) > 0.1f)
            {
                Vector3 toTarget = currentDestination.position - transform.position;
                float distance = toTarget.magnitude;
                Vector3 direction = toTarget.normalized;
                float speed = Mathf.Min(distance * collectionSpeed, collectionSpeed);
                myRigidbody.linearVelocity = direction * speed;
            }
            else
            {
                myRigidbody.linearVelocity = Vector3.zero;
            }
            // if(Vector3.Distance(transform.position, currentDestination.position) > currentCollectedRadius)
            // {
            //     Vector3 direction = (currentDestination.position - transform.position).normalized;
            //     myRigidbody.linearVelocity = direction * collectionSpeed;
            // }
            // else
            // {
            //     if(Vector3.Distance(transform.position, currentDestination.position + currentDestination.TransformDirection(currentShuffleOffset)) < 0.3f)
            //     {
            //         myRigidbody.linearVelocity = Vector3.zero;
            //         currentShuffleOffset = GetNewShuffleOffset();
            //     }

            //     Vector3 direction = (currentDestination.position + currentDestination.TransformDirection(currentShuffleOffset) - transform.position).normalized;
            //     myRigidbody.linearVelocity = direction * currentShuffleSpeed;
            // }

            transform.rotation = currentDestination.rotation * initialRotationOffset;
        }
    }

    private Vector3 GetNewShuffleOffset()
    {
        return new Vector3(Random.Range(0.5f, currentCollectedRadius), Random.Range(0.5f, currentCollectedRadius), Random.Range(0.5f, Mathf.Clamp(currentCollectedRadius / 2f, 0f, float.MaxValue)));
    }

    public void StartLevitate(float currentShuffleSpeed, float currentCollectedRadius, Transform currentDestination)
    {
        GameManager.Instance().StartLevitating(transform);
        isLevitating = true;
        this.currentShuffleSpeed = currentShuffleSpeed;
        this.currentDestination = currentDestination;
        this.currentCollectedRadius = currentCollectedRadius;
        currentShuffleOffset = GetNewShuffleOffset();
        initialRotationOffset = Quaternion.Inverse(currentDestination.rotation) * transform.rotation;
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
