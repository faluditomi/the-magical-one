using UnityEngine;

public class ResetController : MonoBehaviour
{

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void Reset()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

}
