using TOS.Dialogue;
using UnityEngine;

public class InteractionController : MonoBehaviour
{

    private NurseReset nurseReset;
    private Transform cameraTransform;
    private Transform levitatePosition;
    private GameObject currentLevitateTarget;
    private GameObject currentDialogueTarget;
    private ParticleSystem currentHoverParticles;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        levitatePosition = transform.Find("LevitatePosition");
        nurseReset = FindFirstObjectByType<NurseReset>();
    }

    private void Update()
    {
        if(GameManager.Instance().IsLevitationInProgress())
        {
            return;
        }

        RaycastHit hit;
        Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, int.MaxValue);

        if(hit.collider == null)
        {
            NullTargets();
            return;
        }

        if(hit.collider.CompareTag("Levitateable"))
        {
            currentLevitateTarget = hit.collider.gameObject;
            currentHoverParticles = currentLevitateTarget.transform.Find("HoverParticles").GetComponent<ParticleSystem>();
            currentHoverParticles.Play();
        }
        else if(hit.collider.CompareTag("StartDialogue"))
        {
            currentDialogueTarget = hit.collider.gameObject;
        }
        else
        {
            NullTargets();
        }
    }

    private void NullTargets()
    {
        if(currentLevitateTarget != null)
        {
            currentHoverParticles.Stop();
            currentHoverParticles = null;
            currentLevitateTarget = null;
        }
        if(currentDialogueTarget != null)
        {
            currentDialogueTarget = null;
        }
    }

    private void OnEnable()
    {
        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Levitate, Levitate);

        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Hello, StartDialogue);
        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Yep, StartDialogue);
        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Okay, StartDialogue);
        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Yes, StartDialogue);
        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Yeah, StartDialogue);
    }

    private void Levitate(SpellArgs args)
    {
        LevitateArgs myArgs = SpellSessionCache.GetSpellArgs<LevitateArgs>(args);

        if(currentLevitateTarget != null)
        {
            currentHoverParticles.Stop();
            currentLevitateTarget.GetComponent<LevitateBehaviour>().StartLevitate(myArgs.shuffleSpeed, myArgs.collectedRadius, levitatePosition);
            nurseReset.AddTarget(currentLevitateTarget, currentLevitateTarget.transform.Find("ResetPosition").position);
        }
    }

    private void StartDialogue(SpellArgs args)
    {
        if(currentDialogueTarget != null)
        {
            print("GGGG");
            AIConversant aIConversant = currentDialogueTarget.GetComponent<AIConversant>();
            aIConversant.StartCoroutine(aIConversant.RunDialogue());
        }
    }

    public GameObject GetCurrentLevitateTarget()
    {
        return currentLevitateTarget;
    }
    
}
