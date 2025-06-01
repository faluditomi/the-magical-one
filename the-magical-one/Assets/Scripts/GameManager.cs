using UnityEngine;

public class GameManager : MonoBehaviour
{

    private bool isLevitationInProgress;
    public bool isDialogueInProgress;
    public bool isPastDeath;
    public bool hasMagic;
    public bool isPastWizard;
    public bool hasFireball;
    public bool isCameraHacked;
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

    public void StartDialogue()
    {
        isDialogueInProgress = true;
    }

    public void StopDialogue()
    {
        isDialogueInProgress = false;
    }

    public void PastDeath()
    {
        isPastDeath = true;
    }

    public void HasMagic()
    {
        hasMagic = true;
    }

    public void PastWizard()
    {
        isPastWizard = true;
    }

    public void HasFireball()
    {
        hasFireball = true;
    }
    
    public Transform GetCurrentLevitatingTransform()
    {
        return currentLevitatingTransform;
    }

}
