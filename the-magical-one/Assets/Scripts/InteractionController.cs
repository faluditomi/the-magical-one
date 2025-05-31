using UnityEngine;

public class InteractionController : MonoBehaviour
{

    private Transform cameraTransform;
    private Transform levitatePosition;
    private GameObject currentTarget;

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
            currentTarget = hit.collider.gameObject;
            //turn on the currentTarget's outline
        }
        else if(currentTarget != null)
        {
            //turn off the currentTarget's outline
            currentTarget = null;
        }
    }

    private void OnEnable()
    {
        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Levitate, Levitate);
    }

    private void OnDisable()
    {
        SpellEventSubscriber.Instance().UnsubscribeFromSpell(SpellWords.Levitate, Levitate);
    }

    private void Levitate(SpellArgs args)
    {
        LevitateArgs myArgs = SpellSessionCache.GetSpellArgs<LevitateArgs>(args);

        if(currentTarget != null)
        {
            currentTarget.GetComponent<LevitateBehaviour>().StartLevitate(myArgs.shuffleSpeed, myArgs.collectedRadius, levitatePosition);
        }
    }
    
}
