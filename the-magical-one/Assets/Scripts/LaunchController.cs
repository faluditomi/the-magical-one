using UnityEngine;

public class LaunchController : MonoBehaviour
{

    private static LaunchController _instance;

    private Transform cameraTransform;
    
    private GameManager gameManager;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        cameraTransform = Camera.main.transform;
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnEnable()
    {
        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Launch, Launch);
    }

    private void Launch(SpellArgs args)
    {
        LaunchArgs myArgs = SpellSessionCache.GetSpellArgs<LaunchArgs>(args);
        

        if(gameManager.IsLevitationInProgress())
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.magicLevitateLaunch, new Vector3(0,0,0));
            Transform transformToLaunch = gameManager.GetCurrentLevitatingTransform();
            transformToLaunch.GetComponent<LevitateBehaviour>().StopLevitate();
            transformToLaunch.GetComponent<Rigidbody>().AddForce(cameraTransform.forward * myArgs.launchForce, ForceMode.Impulse);
        }
    }
    
}
