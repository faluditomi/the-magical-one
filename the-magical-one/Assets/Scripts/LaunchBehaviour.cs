using UnityEngine;

public class LaunchBehaviour : MonoBehaviour
{

    private void Start()
    {

    }

    private void Update()
    {

    }

    private void OnEnable()
    {
        SpellEventSubscriber.Instance().SubscribeToSpell(SpellWords.Launch, Launch);
    }

    private void OnDisable()
    {
        SpellEventSubscriber.Instance().UnsubscribeFromSpell(SpellWords.Launch, Launch);
    }

    private void Launch(SpellArgs args)
    {
        LaunchArgs myArgs = SpellSessionCache.GetSpellArgs<LaunchArgs>(args);
    }
    
}
