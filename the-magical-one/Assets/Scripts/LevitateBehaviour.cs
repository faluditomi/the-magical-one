using System.Collections.Generic;
using UnityEngine;

public class LevitateBehaviour : MonoBehaviour
{

    private bool isLevitating;
    private float currentShuffleSpeed;
    private float currentCollectedRadius;
    private float collectionSpeed = 20f;
    private Transform currentDestination;
    private Vector3 currentShuffleOffset;
    private Quaternion initialRotationOffset;
    private Rigidbody myRigidbody;
    private GameManager gameManager;
    public bool isRope;
    public List<Rigidbody> ropeRigidbodies;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        gameManager = FindFirstObjectByType<GameManager>();
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
            //     Vector3 toTarget = currentDestination.position - transform.position;
            //     float distance = toTarget.magnitude;
            //     Vector3 direction = toTarget.normalized;
            //     float speed = Mathf.Min(distance * collectionSpeed, collectionSpeed);
            //     myRigidbody.linearVelocity = direction * speed;
            // }
            // else
            // {
            //     if(Vector3.Distance(transform.position, currentDestination.position + currentDestination.TransformDirection(currentShuffleOffset)) < 0.3f)
            //     {
            //         myRigidbody.linearVelocity = Vector3.zero;
            //         myRigidbody.angularVelocity = Vector3.zero;
            //         currentShuffleOffset = GetNewShuffleOffset();
            //     }
                
            //     Vector3 toTarget = currentDestination.position + currentDestination.TransformDirection(currentShuffleOffset) - transform.position;
            //     float distance = toTarget.magnitude;
            //     Vector3 direction = toTarget.normalized;
            //     float speed = Mathf.Min(distance * currentShuffleSpeed, currentShuffleSpeed);
            //     myRigidbody.linearVelocity = direction * speed;
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
        gameManager.StartLevitating(transform);
        isLevitating = true;
        this.currentShuffleSpeed = currentShuffleSpeed;
        this.currentDestination = currentDestination;
        this.currentCollectedRadius = currentCollectedRadius;
        currentShuffleOffset = GetNewShuffleOffset();
        initialRotationOffset = Quaternion.Inverse(currentDestination.rotation) * transform.rotation;

        if(!isRope)
        {
            myRigidbody.useGravity = false;
            myRigidbody.isKinematic = false;
        }
        else
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.unplug, new Vector3(0,1.64f,-4.058f));
            foreach(Rigidbody rope in ropeRigidbodies)
            {
                rope.useGravity = false;
                rope.isKinematic = false;
            }
        }
    }

    public void StopLevitate()
    {
        gameManager.StopLevitating();
        isLevitating = false;
        currentShuffleSpeed = 0f;
        currentDestination = null;
        currentCollectedRadius = 0f;
        currentShuffleOffset = Vector3.zero;

        if(!isRope)
        {
            myRigidbody.linearVelocity = Vector3.zero;
            myRigidbody.useGravity = true;
        }
        else
        {
            foreach(Rigidbody rope in ropeRigidbodies)
            {
                rope.linearVelocity = Vector3.zero;
                rope.useGravity = true;
            }
        }
    }

}
