using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Radio : MonoBehaviour
{
    private EventInstance radioInstance;

    private void Start()
    {
        AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.radio, gameObject);
        radioInstance = AudioManager.instance.CreateEventInstance(FMODEvents.instance.radio);
        radioInstance.start();
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(radioInstance.getParameterByName("RadioDamage", out float value) == 0)
        {
            radioInstance.setParameterByName("RadioDamage", 1);

        }
        else if(value == 1f)
        {
            radioInstance.setParameterByName("RadioDamage", 2);
        }

    }   
}
