using UnityEngine;

public class LaunchController : MonoBehaviour
{

    private static LaunchController _instance;

    private Transform cameraTransform;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Launch, Launch);
    }

    private void Launch(SpellArgs args)
    {
        LaunchArgs myArgs = SpellSessionCache.GetSpellArgs<LaunchArgs>(args);

        if(GameManager.Instance().IsLevitationInProgress())
        {
            Transform transformToLaunch = GameManager.Instance().GetCurrentLevitatingTransform();
            transformToLaunch.GetComponent<LevitateBehaviour>().StopLevitate();
            transformToLaunch.GetComponent<Rigidbody>().AddForce(cameraTransform.forward * myArgs.launchForce, ForceMode.Impulse);
        }
    }
    
}
