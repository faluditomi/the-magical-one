using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
// using Mono.Cecil;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;


    private EventInstance ambienceEventInstance;
    private EventInstance machineDialysisInstance;

    public EventInstance musicEventInstance;
    

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Found more than one AudioManager in the scene.");
        }
        instance = this;

        eventInstances= new List<EventInstance>();
        eventEmitters= new List<StudioEventEmitter>();
    }

    private void Start()
    {

        // Initializes and starts correct music and ambience
        //string currentScene = SceneManager.GetActiveScene().name;
        //switch (currentScene)
        //{
        //    case "Main Menu":
        //        InitializeMusic(FMODEvents.instance.menuMusic);
        //        InitializeAmbience(FMODEvents.instance.cityMenu);
        //        break;

        //    case "Map":
        //        InitializeMusic(FMODEvents.instance.music);
        //        InitializeAmbience(FMODEvents.instance.city);
        //        break;


        //}

        Initialize3DAmbienceObjects();
  
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateEventInstance(musicEventReference);
        musicEventInstance.start();
    }

    private void Initialize3DAmbienceObjects()
    {
        machineDialysisInstance = RuntimeManager.CreateInstance(FMODEvents.instance.machineDialysis);

        RuntimeManager.AttachInstanceToGameObject(machineDialysisInstance, AudioObjects.instance.machineDialysis, AudioObjects.instance.machineDialysis.GetComponent<Rigidbody>());
        eventInstances.Add(machineDialysisInstance);
        machineDialysisInstance.start();
    }

    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }

    public void StartInstancePlaybackAtThisPosition(EventInstance eventInstance, GameObject gameObject)
    {
        RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject);
        eventInstance.start();
    }


    public void StopInstancePlayback(EventInstance eventInstance, FMOD.Studio.STOP_MODE stop)
    {
        eventInstance.stop(stop);
    }
    public void SetGameParameter(EventInstance eventInstance, string parameterName, float parameterValue)
    {
        eventInstance.setParameterByName(parameterName, parameterValue);
    }

    // public void SetMusicArea(MusicArea area)
    // {
    //    musicEventInstance.setParameterByName("area", (float) area);
    // }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public EventInstance Create3DEventInstance(EventReference eventReference, GameObject emitterGameObject, Rigidbody emitterObjectRigidbody)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, emitterGameObject, emitterObjectRigidbody);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        // stop all of the event emitters, because if we don't, they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }

}
