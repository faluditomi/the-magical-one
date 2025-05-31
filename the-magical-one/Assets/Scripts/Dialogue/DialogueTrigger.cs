using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] List<string> actions = new List<string>();

    [SerializeField] UnityEvent @event;
        
    //Triggers Unity Events.
    public void Trigger(List<string> actionsToTrigger)
    {
        foreach(string i in actionsToTrigger)
        {
            if(actions.Contains(i))
                @event.Invoke();
        }
    }
}
