using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isLevitationInProgress;
    public bool hasMagic;
    public bool hasFireball;
    public bool isCameraHacked;
    public bool isReadyToDie;
    private Transform currentLevitatingTransform;

    public void StartLevitating(Transform levitatingObject)
    {
        isLevitationInProgress = true;

        currentLevitatingTransform = levitatingObject;
    }

    public void StopLevitating()
    {
        isLevitationInProgress = false;

        currentLevitatingTransform = null;
    }

    public bool IsLevitationInProgress()
    {
        return isLevitationInProgress;
    }

    public void SetCameraHacked()
    {
        isCameraHacked = true;
    }

    public bool GetCameraHacked()
    {
        return isCameraHacked;
    }

    public void HasMagic()
    {
        hasMagic = true;
    }

    public void HasFireball()
    {
        hasFireball = true;
    }

    public void SetReadyToDie()
    {
        isReadyToDie = true;
    }

    public bool GetReadyToDie()
    {
        return isReadyToDie;
    }

    public Transform GetCurrentLevitatingTransform()
    {
        return currentLevitatingTransform;
    }

}
