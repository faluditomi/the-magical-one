using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    // This script holds references to all events in FMOD.

    // Sets the class itself to be static, meaning that even if the script is put on two different objects, so that two
    // "instances" of the class exist in the same scene, the content of those to instances is shared.
    // So whatever is changed in instance_01, is also changed in instance_02.
    // Also sets the contents of the class, to be readable from any other script, but can only be changed from inside the class itself.
    public static FMODEvents instance { get; private set; }

    [field: Header("Ambience")]
    [field: SerializeField] public EventReference machineDialysis { get; private set; }
    [field: SerializeField] public EventReference aircondition { get; private set; }
    [field: SerializeField] public EventReference windowSounds { get; private set; }


    [field: Header("SFX")]
    [field: SerializeField] public EventReference magicLevitate { get; private set; }
    [field: SerializeField] public EventReference magicLevitateLaunch { get; private set; }
    [field: SerializeField] public EventReference radio { get; private set; }
    [field: SerializeField] public EventReference radioImpact { get; private set; }


    [field: Header("NPC SFX")]
    [field: SerializeField] public EventReference nurseBotIdle { get; private set; }
    [field: SerializeField] public EventReference nurseBotMoving { get; private set; }
    [field: SerializeField] public EventReference nurseBotTalking { get; private set; }


    //[field: Header("UI")]
    //[field: SerializeField] public EventReference pickUp { get; private set; }
    //[field: SerializeField] public EventReference dropOff { get; private set; }


    // Basically makes sure that only one instance of this class, will be taken into account,
    // in the case that more than one instance exists in the same scene. Not sure why this is important actually...
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Found more than one FMODEvents instance in the scene.");
        }
        instance = this;
    }
}
