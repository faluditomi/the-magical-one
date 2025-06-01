using UnityEngine;

public class GameManager : MonoBehaviour
{

    private bool isLevitationInProgress;
    public bool isDialogueInProgress;
    public bool isPastDeath;
    public bool hasMagic;
    public bool isPastWizard;
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
    
    public Transform GetCurrentLevitatingTransform()
    {
        return currentLevitatingTransform;
    }

}
