using UnityEngine;

public class InteractionController : MonoBehaviour
{

    private NurseReset nurseReset;
    private Transform cameraTransform;
    private Transform levitatePosition;
    private GameObject currentLevitateTarget;
    private GameObject currentDialogueTarget;
    private ParticleSystem currentHoverParticles;
    private GameManager gameManager;

    public bool fireballActive = false;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        levitatePosition = transform.Find("LevitatePosition");
        nurseReset = FindFirstObjectByType<NurseReset>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        if(gameManager.IsLevitationInProgress())
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

        if(hit.collider.CompareTag("Levitateable") && gameManager.hasMagic)
        {
            currentLevitateTarget = hit.collider.gameObject;
            currentHoverParticles = currentLevitateTarget.transform.Find("HoverParticles").GetComponent<ParticleSystem>();
            currentHoverParticles.Play();
        }
        else if(hit.collider.CompareTag("StartDialogue"))
        {
            currentDialogueTarget = hit.collider.gameObject;

            if((!currentDialogueTarget.name.Equals("Dialogue") || (gameManager.isPastDeath && !gameManager.isPastWizard))
            && !gameManager.isDialogueInProgress)
            {
                currentHoverParticles = currentDialogueTarget.transform.Find("HoverParticles").GetComponent<ParticleSystem>();
                currentHoverParticles.Play();
            }


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
            currentLevitateTarget = null;
        }
        if(currentDialogueTarget != null)
        {
            currentDialogueTarget = null;
        }
        if(currentHoverParticles != null)
        {
            currentHoverParticles.Stop();
            currentHoverParticles = null;
        }
    }

    private void OnEnable()
    {
        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Levitate, Levitate);

        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Hello, StartDialogue);
        // SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Yep, StartDialogue);
        // SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Okay, StartDialogue);
        // SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Yes, StartDialogue);
        // SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Yeah, StartDialogue);

        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Fireball, Fireball);
    }

    private void Levitate(SpellArgs args)
    {
        LevitateArgs myArgs = SpellSessionCache.GetSpellArgs<LevitateArgs>(args);

        if(currentLevitateTarget != null && gameManager.hasMagic)
        {
            currentHoverParticles.Stop();
            currentLevitateTarget.GetComponent<LevitateBehaviour>().StartLevitate(myArgs.shuffleSpeed, myArgs.collectedRadius, levitatePosition);
            nurseReset.AddTarget(currentLevitateTarget, currentLevitateTarget.transform.Find("ResetPosition").position);
        }
    }

    private void StartDialogue(SpellArgs args)
    {
        if(currentDialogueTarget != null && !gameManager.isDialogueInProgress)
        {
            if(currentDialogueTarget.name.Equals("Dialogue") && (!gameManager.isPastDeath || gameManager.isPastWizard))
            {
                return;
            }

            AIConversant aIConversant = currentDialogueTarget.GetComponent<AIConversant>();
            aIConversant.StartCoroutine(aIConversant.RunDialogue());
        }
    }

    private void Fireball(SpellArgs args)
    {
        FireballArgs myArgs = SpellSessionCache.GetSpellArgs<FireballArgs>(args);

        FireballController shooter = GetComponent<FireballController>();

        if(shooter != null && fireballActive)
        {
            shooter.ShootFireball(myArgs.speed, myArgs.radius);
        }
    }

    public GameObject GetCurrentLevitateTarget()
    {
        return currentLevitateTarget;
    }
    
}
