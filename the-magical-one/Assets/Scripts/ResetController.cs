using UnityEngine;

public class ResetController : MonoBehaviour
{

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody myRigidbody;
    private LevitateBehaviour levitateBehaviour;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        levitateBehaviour = GetComponent<LevitateBehaviour>();
    }

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void Reset()
    {
        levitateBehaviour.StopLevitate();
        myRigidbody.linearVelocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

}
