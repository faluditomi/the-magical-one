using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using static UnityEngine.Rendering.DebugUI;

public class Radio : MonoBehaviour
{
    private EventInstance radioInstance;

    private void Start()
    {
        AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.radio, gameObject);
        radioInstance = AudioManager.instance.Create3DEventInstance(FMODEvents.instance.radio, gameObject, GetComponent<Rigidbody>());
        radioInstance.start();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(radioInstance.getParameterByName("RadioDamage", out float value) == 0)
        {
            radioInstance.setParameterByName("RadioDamage", 1);
            value++;

            Debug.Log("RadioDamage = " + value);
        }

        if(value == 1)
        {
            radioInstance.setParameterByName("RadioDamage", 2);
            value++;

            Debug.Log("The radio has been fixed!");
        } else { return; }

        
    }   
}
