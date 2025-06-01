using System.Collections.Generic;
using UnityEngine;

public class ResetController : MonoBehaviour
{

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody myRigidbody;
    private LevitateBehaviour levitateBehaviour;
    public bool isRope;
    public List<Rigidbody> ropeRigidbodies;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        levitateBehaviour = GetComponent<LevitateBehaviour>();
    }

    private void Start()
    {
        if(isRope)
        {
            foreach(Rigidbody rope in ropeRigidbodies)
            {
                initialPosition = rope.transform.position;
                initialRotation = rope.transform.rotation;
            }
        }
        else
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }
    }

    public void Reset()
    {
        levitateBehaviour.StopLevitate();

        if(isRope)
        {
            foreach(Rigidbody rope in ropeRigidbodies)
            {
                rope.linearVelocity = Vector3.zero;
                rope.angularVelocity = Vector3.zero;
                rope.isKinematic = true;
                rope.transform.position = initialPosition;
                rope.transform.rotation = initialRotation;
            }
        }
        else
        {
            myRigidbody.linearVelocity = Vector3.zero;
            myRigidbody.angularVelocity = Vector3.zero;
            myRigidbody.isKinematic = true;
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }
        
    }

}
