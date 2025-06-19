using UnityEngine;
using FMOD.Studio;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private Transform levitatePosition;
    private Transform cameraTransform;

    private GameObject currentLookTarget;

    private NurseReset nurseReset;
    private GameManager gameManager;
    private EndingSequence endingSequence;
    private FireballController fireballController;

    public EventInstance levitateEventInstance;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;

        nurseReset = FindFirstObjectByType<NurseReset>();
        gameManager = FindFirstObjectByType<GameManager>();
        endingSequence = FindFirstObjectByType<EndingSequence>();
        fireballController = GetComponent<FireballController>();
    }

    private void Start()
    {
        levitateEventInstance = AudioManager.instance.CreateEventInstance(FMODEvents.instance.magicLevitate);
    }

    private void OnEnable()
    {
        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Levitate, Levitate);
        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Hello, StartDialogue);
        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Fireball, Fireball);
    }

    private void Update()
    {
        if(gameManager.IsLevitationInProgress())
        {
            if(currentLookTarget != null)
            {
                ManageParticleState(null);

                currentLookTarget = null;
            }

            return;
        }

        GameObject newLookTarget = null;

        RaycastHit hit;

        int layerToIgnore = 2;

        int layerMask = ~(1 << layerToIgnore);

        if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, float.MaxValue, layerMask))
        {
            if(hit.collider.CompareTag("Levitateable") || hit.collider.CompareTag("StartDialogue"))
            {
                newLookTarget = hit.collider.gameObject;
            }
        }
        
        if(newLookTarget == currentLookTarget)
        {
            return;
        }

        ManageParticleState(newLookTarget);

        currentLookTarget = newLookTarget;
    }

    private void ManageParticleState(GameObject newTarget)
    {
        if(currentLookTarget != null && currentLookTarget.CompareTag("Levitateable"))
        {
            Transform particleTransform = currentLookTarget.transform.Find("HoverParticles");
            if(particleTransform != null && particleTransform.gameObject.activeSelf)
            {
                particleTransform.gameObject.SetActive(false);
            }
        }

        if(newTarget != null && newTarget.CompareTag("Levitateable") && gameManager.hasMagic)
        {
            Transform particleTransform = newTarget.transform.Find("HoverParticles");

            if(particleTransform != null)
            {
                particleTransform.gameObject.SetActive(true);
            }
        }
    }

    private void Levitate(SpellArgs args)
    {
        if(currentLookTarget != null && currentLookTarget.CompareTag("Levitateable") && gameManager.hasMagic)
        {
            LevitateArgs myArgs = SpellSessionCache.GetSpellArgs<LevitateArgs>(args);

            if(currentLookTarget.name == "Plug")
            {
                if(gameManager.GetCameraHacked())
                {
                    gameManager.SetReadyToDie();

                    endingSequence.StartEndingSequence();
                }
                else
                {
                    endingSequence.StartEdgingDeath();
                }
            }

            AudioManager.instance.StartInstancePlaybackAtThisPosition(levitateEventInstance, gameObject);

            currentLookTarget.GetComponent<LevitateBehaviour>().StartLevitate(myArgs.shuffleSpeed, myArgs.collectedRadius, levitatePosition);

            Transform resetPos = currentLookTarget.transform.Find("ResetPosition");

            if(resetPos != null)
            {
                nurseReset.AddTarget(currentLookTarget, resetPos.position);
            }
        }
    }

    private void StartDialogue(SpellArgs args)
    {
        if(currentLookTarget != null && currentLookTarget.CompareTag("StartDialogue"))
        {
            AIConversant aIConversant = currentLookTarget.GetComponent<AIConversant>();

            if(aIConversant != null)
            {
                aIConversant.StartDialogue();
            }
        }
    }

    private void Fireball(SpellArgs args)
    {
        if (fireballController != null && gameManager.hasFireball)
        {
            FireballArgs myArgs = SpellSessionCache.GetSpellArgs<FireballArgs>(args);

            fireballController.ShootFireball(myArgs.speed, myArgs.radius);
        }
    }
}
