using UnityEngine;

public class InteractionController : MonoBehaviour
{

    private Transform cameraTransform;
    private Transform levitatePosition;
    private GameObject currentLevitateTarget;
    private GameObject currentDialogueTarget;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        levitatePosition = transform.Find("LevitatePosition");
    }

    private void Update()
    {
        if(GameManager.Instance().IsLevitationInProgress())
        {
            return;
        }

        RaycastHit hit;
        Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, int.MaxValue);

        if(hit.collider.CompareTag("Levitateable"))
        {
            currentLevitateTarget = hit.collider.gameObject;
            //turn on the currentTarget's outline
        }
        else if(hit.collider.CompareTag("StartDialogue"))
        {
            currentDialogueTarget = hit.collider.gameObject;
        }
        else
        {
            if(currentLevitateTarget != null)
            {
                //turn off the currentTarget's outline
                currentLevitateTarget = null;
            }
            if(currentDialogueTarget != null)
            {
                currentDialogueTarget = null;
            }
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

    private void OnDisable()
    {
        SpellEventSubscriber.Instance().UnsubscribeFromSpell(SpellWords.Levitate, Levitate);
        SpellEventSubscriber.Instance().UnsubscribeFromSpell(SpellWords.Hello, StartDialogue);
    }

    private void Levitate(SpellArgs args)
    {
        LevitateArgs myArgs = SpellSessionCache.GetSpellArgs<LevitateArgs>(args);

        if(currentLevitateTarget != null)
        {
            currentLevitateTarget.GetComponent<LevitateBehaviour>().StartLevitate(myArgs.shuffleSpeed, myArgs.collectedRadius, levitatePosition);
        }
    }

    private void StartDialogue(SpellArgs args)
    {
        if(currentDialogueTarget != null)
        {
            AIConversant aIConversant = currentDialogueTarget.GetComponent<AIConversant>();
            aIConversant.StartCoroutine(aIConversant.RunDialogue());
        }
    }
}
