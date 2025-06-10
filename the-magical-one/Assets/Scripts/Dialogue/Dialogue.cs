using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
{
    #region Attributes
    [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();

    Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

    [SerializeField] Vector2 newNodeOffset = new Vector2(250, 0);
    #endregion

    //Is called when a value is changed in the inspector or when a scriptable object is loaded.
    private void OnValidate()
    {
        if(nodeLookup != null)
        {
            nodeLookup.Clear();
        }

        foreach(DialogueNode node in GetAllNodes())
        {
            if(node)
            {
                nodeLookup[node.name] = node;
            }
        }
    }

    public DialogueNode GetNodeByIndex(int index)
    {
        return nodes[index];
    }

    public int GetAllNodesByIndex()
    {
        return nodes.Count;
    }

    public IEnumerable<DialogueNode> GetAllNodes()
    {
        return nodes;
    }

    public DialogueNode GetRootNode()
    {
        return nodes[0];
    }

    public List<DialogueNode> GetAllChildren(DialogueNode parentNode)
    {
        List<DialogueNode> dialogueNodes = new List<DialogueNode>();
            
        foreach(string childID in parentNode.GetChildren())
        {
            if(nodeLookup.ContainsKey(childID))
            {
                dialogueNodes.Add(nodeLookup[childID]);
            }
        }

        return dialogueNodes;
    }

    //Runs the methods if only its an Editor window.
    #if UNITY_EDITOR
    public void CreateNode(DialogueNode parent)
    {
        DialogueNode newNode = MakeNode(parent);

        Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node.");

        Undo.RecordObject(this, "Added Dialogue Node.");

        AddNode(newNode);
    }

    public void DeleteNode(DialogueNode nodeToDelete)
    {
        Undo.RecordObject(this, "Deleted Dialogue Node.");

        nodes.Remove(nodeToDelete);

        OnValidate();

        Undo.DestroyObjectImmediate(nodeToDelete);
    }

    private void AddNode(DialogueNode newNode)
    {
        nodes.Add(newNode);

        OnValidate();
    }

    private DialogueNode MakeNode(DialogueNode parent)
    {
        DialogueNode newNode = CreateInstance<DialogueNode>();

        //Where the random naming happens.
        newNode.name = Guid.NewGuid().ToString();

        if(parent)
        {
            parent.AddChild(newNode.name);

            newNode.SetPosition(parent.GetRect().position + newNodeOffset);

            //newNode.SetSpeakerTextGameObject(parent.GetSpeakerTextGameObject());

            newNode.SetDialogueTextGameObject(parent.GetDialogueTextGameObject());

            //newNode.SetSpeakerText(parent.GetSpeakerText());
        }

        return newNode;
    }
    #endif

    public void OnBeforeSerialize()
    {
        #if UNITY_EDITOR
        if(nodes.Count == 0)
        {
            DialogueNode newNode = MakeNode(null);

            AddNode(newNode);
        }

        if(AssetDatabase.GetAssetPath(this) != "")
        {
            foreach(DialogueNode node in GetAllNodes())
            {
                if(AssetDatabase.GetAssetPath(node) == "")
                {
                    AssetDatabase.AddObjectToAsset(node, this);
                }
            }
        }
        #endif
    }

    #region ISerializationCallbackReceiver Methods
    //Needed because the Script derives from ISerializationCallbackReceiver.
    public void OnAfterDeserialize()
    {
    }
    #endregion
}
