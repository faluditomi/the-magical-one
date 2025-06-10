using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

public class DialogueEditor : EditorWindow
{
    #region Attributes
    Dialogue selectedDialogue = null;

    [NonSerialized] DialogueNode draggingNode = null;

    [NonSerialized] DialogueNode creatingNode = null;

    [NonSerialized] DialogueNode deletingNode = null;

    [NonSerialized] DialogueNode linkingParentNode = null;

    [NonSerialized] GUIStyle nodeStyle;

    [NonSerialized] GUIStyle specialNodeStyle;

    [NonSerialized] GUIStyle selectedNodeStyle;

    [NonSerialized] Vector2 draggingOffset;

    [NonSerialized] Vector2 draggingCanvasOffset;

    private Vector2 scrollPosition;

    private string selectedDialogueNodeName = null;

    [NonSerialized] bool draggingCanvas = false;

    const float canvasSize = 4000;
        
    const float backgroundSize = 50;
    #endregion

    #region Editor Methods
    [MenuItem("Dialogue/Dialogue Editor")]
    public static void ShowEditorWindow()
    {
        GetWindow(typeof(DialogueEditor), false, "DialogueEditor");
    }

    [OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;

        if(dialogue != null)
        {
            ShowEditorWindow();

            return true;
        }

        return false;
    }

    //Layouts the DialogueNodes and styles them.
    private void OnEnable()
    {
        OnSelectionChanged();

        Selection.selectionChanged += OnSelectionChanged;

        nodeStyle = new GUIStyle();

        nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;

        nodeStyle.normal.textColor = Color.white;

        nodeStyle.padding = new RectOffset(20, 20, 20, 20);

        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        specialNodeStyle = new GUIStyle();

        specialNodeStyle.normal.background = EditorGUIUtility.Load("node4") as Texture2D;

        specialNodeStyle.normal.textColor = Color.white;

        specialNodeStyle.padding = new RectOffset(20, 20, 20, 20);

        specialNodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();

        selectedNodeStyle.normal.background = EditorGUIUtility.Load("node0 on") as Texture2D;

        selectedNodeStyle.normal.textColor = Color.white;

        selectedNodeStyle.padding = new RectOffset(20, 20, 20, 20);

        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
    }

    //Redraws the UI.
    private void OnSelectionChanged()
    {
        Dialogue newDialogue = Selection.activeObject as Dialogue;

        if(newDialogue)
        {
            selectedDialogue = newDialogue;

            Repaint();
        }
    }

    //Draws the UI of the Editor.
    private void OnGUI()
    {
        if(!selectedDialogue)
        {
            EditorGUILayout.LabelField("No Dialogue Selected", EditorStyles.centeredGreyMiniLabel);
        }
        else
        {
            ProcessEvents();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);

            Texture2D backgroundTex = Resources.Load("DialogueEditorBackground") as Texture2D;

            Rect texCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);

            GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);

            foreach(DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawConnections(node);

                DrawNode(node);
            }

            EditorGUILayout.EndScrollView();

            if(creatingNode)
            {
                selectedDialogue.CreateNode(creatingNode);

                creatingNode = null;
            }

            if(deletingNode)
            {
                selectedDialogue.DeleteNode(deletingNode);

                deletingNode = null;
            }
        }
    }

    //Allows the nodes to be dragged.
    private void ProcessEvents()
    {
        if(Event.current.type == EventType.MouseDown && !draggingNode)
        {
            draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);

            if(draggingNode)
            {
                draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    
                Selection.activeObject = draggingNode;

                selectedDialogueNodeName = draggingNode.name;
            }
            else
            {
                selectedDialogueNodeName = null;

                draggingCanvas = true;

                draggingCanvasOffset = Event.current.mousePosition + scrollPosition;

                Selection.activeObject = selectedDialogue;
            }
        }
        else if(Event.current.type == EventType.MouseDrag && draggingNode)
        {
            draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);

            GUI.changed = true;
        }
        else if(Event.current.type == EventType.MouseDrag && draggingCanvas)
        {
            scrollPosition = draggingCanvasOffset - Event.current.mousePosition;

            GUI.changed = true;
        }
        else if(Event.current.type == EventType.MouseUp && draggingNode)
        {
            draggingNode = null;
        }
        else if(Event.current.type == EventType.MouseUp && draggingCanvas)
        {
            draggingCanvas = false;
        }
    }
    #endregion

    #region Normal Methods
    private void DrawNode(DialogueNode node)
    {
        GUIStyle style;

        if(node.GetTriggerActions().Count == 0)
        {
            style = nodeStyle;
        }
        else
        {
            style = specialNodeStyle;
        }

        if(node.name == selectedDialogueNodeName)
        {
            style = selectedNodeStyle;
        }

        GUILayout.BeginArea(node.GetRect(), style);

        //node.SetSpeakerText(EditorGUILayout.TextField(node.GetSpeakerText()));

        node.SetText(EditorGUILayout.TextField(node.GetDialogueText()));

        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Add"))
        {
            creatingNode = node;
        }

        DrawLinkButtons(node);

        if(GUILayout.Button("Delete"))
        {
            deletingNode = node;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    //Linking and unlinking nodes.
    private void DrawLinkButtons(DialogueNode node)
    {
        if(!linkingParentNode)
        {
            if(GUILayout.Button("Link"))
            {
                linkingParentNode = node;
            }
        }
        else if(linkingParentNode == node)
        {
            if(GUILayout.Button("Cancel"))
            {
                linkingParentNode = null;
            }
        }
        else if(linkingParentNode.GetChildren().Contains(node.name))
        {
            if(GUILayout.Button("Unlink"))
            {
                linkingParentNode.RemoveChild(node.name);

                linkingParentNode = null;
            }
        }
        else
        {
            if(GUILayout.Button("Child"))
            {
                linkingParentNode.AddChild(node.name);

                linkingParentNode = null;
            }
        }
    }

    //Draws the connections between nodes with Bezier curves.
    private void DrawConnections(DialogueNode node)
    {
        foreach(DialogueNode childNode in selectedDialogue.GetAllChildren(node))
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);

            Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);

            Vector3 controlPointOffset = endPosition - startPosition;

            controlPointOffset.y = 0f;

            controlPointOffset.x *= 1f;

            if(node.name == selectedDialogueNodeName)
            {
                Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffset, endPosition - controlPointOffset, Color.white, null, 4f);
            }
            else
            {
                Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffset, endPosition - controlPointOffset, Color.gray, null, 4f);
            }
        }
    }

    private DialogueNode GetNodeAtPoint(Vector2 point)
    {
        DialogueNode foundNode = null;

        foreach(DialogueNode node in selectedDialogue.GetAllNodes())
        {
            if(node.GetRect().Contains(point))
            {
                foundNode = node;
            }
        }

        return foundNode;
    }
}
#endregion
