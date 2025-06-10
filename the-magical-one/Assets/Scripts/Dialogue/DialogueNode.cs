using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Dialogue Node", menuName = "DialogueNodes", order = 0)]
public class DialogueNode : ScriptableObject, ISerializationCallbackReceiver
{
    #region Attributes
    //[TextArea(1, 15)] [SerializeField] string speakerText;

    [TextArea(10,15)] [SerializeField] string dialogueText;

    [HideInInspector] [SerializeField] List<string> children = new List<string>();

    [HideInInspector] [SerializeField] Rect rect = new Rect(0, 0, 200, 100);

    //[SerializeField] public string speakerTextGameObject = "SpeakerText";

    [SerializeField] public string dialogueTextGameObject = "DialogueText";

    [SerializeField] public AudioClip mAudioClip;

    [SerializeField] List<string> triggerActions = new List<string>();
    #endregion

    #region Getters
    public Rect GetRect()
    {
        return rect;
    }

    //public string GetSpeakerText()
    //{
    //    return speakerText;
    //}

    public string GetDialogueText()
    {
        return dialogueText;
    }

    public List<string> GetChildren()
    {
        return children;
    }

    //public string GetSpeakerTextGameObject()
    //{
    //    return speakerTextGameObject;
    //}

    public string GetDialogueTextGameObject()
    {
        return dialogueTextGameObject;
    }

    public List<string> GetTriggerActions()
    {
        return triggerActions;
    }
    #endregion

    #region Setters
    #if UNITY_EDITOR
    public void SetPosition(Vector2 newPosition)
    {
        Undo.RecordObject(this, "Moved Dialogue Node.");

        rect.position = newPosition;

        EditorUtility.SetDirty(this);
    }

    //public void SetSpeakerText(string newSpeakerText)
    //{
    //    if(newSpeakerText != speakerText)
    //    {
    //        Undo.RecordObject(this, "Updated Speaker Text.");

    //        speakerText = newSpeakerText;

    //        EditorUtility.SetDirty(this);
    //    }
    //}

    public void SetText(string newDialogueText)
    {
        if(newDialogueText != dialogueText)
        {
            Undo.RecordObject(this, "Updated Dialogue Text.");

            dialogueText = newDialogueText;

            EditorUtility.SetDirty(this);
        }
    }

    //public void SetSpeakerTextGameObject(string newSpeakerTextGameObject)
    //{
    //    Undo.RecordObject(this, "Updated Speaker Text Name.");

    //    speakerTextGameObject = newSpeakerTextGameObject;

    //    EditorUtility.SetDirty(this);
    //}

    public void SetDialogueTextGameObject(string newDialogueTextGameObject)
    {
        Undo.RecordObject(this, "Updated Dialogue Text Name.");

        dialogueTextGameObject = newDialogueTextGameObject;

        EditorUtility.SetDirty(this);
    }

    public void AddChild(string childID)
    {
        Undo.RecordObject(this, "Added Dialogue Link.");

        children.Add(childID);

        EditorUtility.SetDirty(this);
    }

    public void RemoveChild(string childID)
    {
        Undo.RecordObject(this, "Removed Dialogue Link.");

        children.Remove(childID);

        EditorUtility.SetDirty(this);
    }
    #endif
    #endregion

    #region ISerializationCallbackReceiver Methods
    //Needed because the Script derives from ISerializationCallbackReceiver.
    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
    }
    #endregion
}
