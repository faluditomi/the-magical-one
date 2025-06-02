using System.Collections.Generic;
using UnityEngine;

public class ResetController : MonoBehaviour
{

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody myRigidbody;
    private LevitateBehaviour levitateBehaviour;
    [SerializeField] bool isRope;
    [SerializeField] bool isPlug;
    public List<Rigidbody> ropeRigidbodies;
    private EndingSequence endingSequence;

    private Vector3[] initialPositions;        // Added
    private Quaternion[] initialRotations;     // Added

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        levitateBehaviour = GetComponent<LevitateBehaviour>();
        endingSequence = FindFirstObjectByType<EndingSequence>();
    }

    private void Start()
    {
        //if(isRope)
        //{
        //    foreach(Rigidbody rope in ropeRigidbodies)
        //    {
        //        initialPosition = rope.transform.position;
        //        initialRotation = rope.transform.rotation;
        //    }
        //}
        //else
        //{
        //    initialPosition = transform.position;
        //    initialRotation = transform.rotation;
        //}

        if(isRope)
        {
            int count = ropeRigidbodies.Count;
            initialPositions = new Vector3[count];
            initialRotations = new Quaternion[count];

            for(int i = 0; i < count; i++)
            {
                initialPositions[i] = ropeRigidbodies[i].transform.position;
                initialRotations[i] = ropeRigidbodies[i].transform.rotation;
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

        if(isPlug)
        {
            endingSequence.EndEdgingDeath();
        }

        //if(isRope)
        //{
        //    foreach(Rigidbody rope in ropeRigidbodies)
        //    {
        //        rope.linearVelocity = Vector3.zero;
        //        rope.angularVelocity = Vector3.zero;
        //        rope.isKinematic = true;
        //        rope.transform.position = initialPosition;
        //        rope.transform.rotation = initialRotation;
        //    }
        //}
        //else
        //{
        //    myRigidbody.linearVelocity = Vector3.zero;
        //    myRigidbody.angularVelocity = Vector3.zero;
        //    myRigidbody.isKinematic = true;
        //    transform.position = initialPosition;
        //    transform.rotation = initialRotation;
        //}

        if(isRope)
        {
            for(int i = 0; i < ropeRigidbodies.Count; i++)
            {
                Rigidbody rope = ropeRigidbodies[i];
                rope.linearVelocity = Vector3.zero;
                rope.angularVelocity = Vector3.zero;
                rope.isKinematic = true;
                rope.transform.position = initialPositions[i];
                rope.transform.rotation = initialRotations[i];
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
