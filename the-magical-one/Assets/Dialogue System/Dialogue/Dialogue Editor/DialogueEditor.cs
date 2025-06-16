using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class DialogueEditor : EditorWindow
{
    #region Attributes
    Dialogue selectedDialogue = null;
    [NonSerialized] DialogueNode draggingNode = null;
    [NonSerialized] DialogueNode creatingNode = null;
    [NonSerialized] DialogueNode deletingNode = null;
    [NonSerialized] DialogueNode linkingParentNode = null;
    private DialogueNode selectedNode = null;

    [NonSerialized] GUIStyle nodeStyle;
    [NonSerialized] GUIStyle triggerActionsNodeStyle;
    [NonSerialized] GUIStyle selectedNodeStyle;
    [NonSerialized] GUIStyle scaledTextFieldStyle;
    [NonSerialized] GUIStyle scaledButtonStyle;
    [NonSerialized] GUIStyle currentScaledNodeStyle;

    [NonSerialized] Texture2D backgroundTex;

    private List<DialogueNode> reusableChildrenList = new List<DialogueNode>();

    [NonSerialized] Vector2 draggingOffset;
    [NonSerialized] Vector2 draggingCanvasOffset;
    private Vector2 scrollPosition;

    [NonSerialized] bool draggingCanvas = false;

    const float canvasSize = 4000;
    const float backgroundSize = 50;
    private float zoomScale = 1f;
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

        backgroundTex = Resources.Load("DialogueEditorBackground") as Texture2D;

        nodeStyle = new GUIStyle();

        nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;

        nodeStyle.normal.textColor = Color.white;

        nodeStyle.padding = new RectOffset(20, 20, 20, 20);

        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        nodeStyle.fontSize = 12;

        triggerActionsNodeStyle = new GUIStyle(nodeStyle);

        triggerActionsNodeStyle.normal.background = EditorGUIUtility.Load("node4") as Texture2D;

        selectedNodeStyle = new GUIStyle(nodeStyle);

        selectedNodeStyle.normal.background = EditorGUIUtility.Load("node0 on") as Texture2D;

        scaledTextFieldStyle = new GUIStyle(EditorStyles.textField);

        scaledButtonStyle = new GUIStyle(EditorStyles.miniButton);

        currentScaledNodeStyle = new GUIStyle();
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

            if(backgroundTex != null)
            {
                float scaledBackgroundSize = backgroundSize * zoomScale;

                Rect texCoords = new Rect(
                scrollPosition.x / scaledBackgroundSize,
                -scrollPosition.y / scaledBackgroundSize,
                position.width / scaledBackgroundSize,
                position.height / scaledBackgroundSize);

                GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), backgroundTex, texCoords);
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);

            IEnumerable<DialogueNode> allNodes = selectedDialogue.GetAllNodes();

            foreach(DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawConnections(node, zoomScale);
            }

            foreach(DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawNode(node, zoomScale);
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
        if(Event.current.type == EventType.ScrollWheel && Event.current.control)
        {
            zoomScale -= Event.current.delta.y * 0.01f;

            zoomScale = Mathf.Clamp(zoomScale, 0.5f, 1.0f);

            GUI.changed = true;

            Event.current.Use();
        }

        Vector2 mousePositionInCanvas = (Event.current.mousePosition + scrollPosition) / zoomScale;

        if(Event.current.type == EventType.MouseDown && !draggingNode)
        {
            draggingNode = GetNodeAtPoint(mousePositionInCanvas);

            if(draggingNode)
            {
                draggingOffset = draggingNode.GetRect().position - mousePositionInCanvas;
                    
                Selection.activeObject = draggingNode;

                selectedNode = draggingNode;
            }
            else
            {
                selectedNode = null;

                draggingCanvas = true;

                draggingCanvasOffset = Event.current.mousePosition + scrollPosition;

                Selection.activeObject = selectedDialogue;
            }
        }
        else if(Event.current.type == EventType.MouseDrag && draggingNode)
        {
            draggingNode.SetPosition(mousePositionInCanvas + draggingOffset);

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
    private void DrawNode(DialogueNode node, float zoom)
    {
        GUIStyle baseNodeStyle = nodeStyle;

        if(node.GetTriggerActions().Count > 0)
        {
            baseNodeStyle = triggerActionsNodeStyle;
        }
        else if(node == selectedNode)
        {
            baseNodeStyle = selectedNodeStyle;
        }

        //UpdateScaledStyle(baseNodeStyle, zoom, scaledTextFieldStyle, scaledButtonStyle);
        ApplyScalingToStyle(currentScaledNodeStyle, baseNodeStyle, zoom);
        ApplyScalingToStyle(scaledTextFieldStyle, EditorStyles.textField, zoom);
        ApplyScalingToStyle(scaledButtonStyle, EditorStyles.miniButton, zoom);

        Rect scaledRect = new Rect(node.GetRect().position * zoom,node.GetRect().size * zoom);

        GUILayout.BeginArea(scaledRect, currentScaledNodeStyle);

        node.SetText(EditorGUILayout.TextField(node.GetDialogueText(), scaledTextFieldStyle));

        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Add", scaledButtonStyle))
        {
            creatingNode = node;
        }

        DrawLinkButtons(node, scaledButtonStyle);

        if(GUILayout.Button("Delete", scaledButtonStyle))
        {
            deletingNode = node;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private void ApplyScalingToStyle(GUIStyle styleToScale, GUIStyle baseStyle, float zoom)
    {
        styleToScale.normal = baseStyle.normal;

        styleToScale.fontSize = Mathf.Max(1, (int) (baseStyle.fontSize * zoom));

        styleToScale.padding = new RectOffset(
        (int) (baseStyle.padding.left * zoom),
        (int) (baseStyle.padding.right * zoom),
        (int) (baseStyle.padding.top * zoom),
        (int) (baseStyle.padding.bottom * zoom));

        styleToScale.border = new RectOffset(
        (int) (baseStyle.border.left * zoom),
        (int) (baseStyle.border.right * zoom),
        (int) (baseStyle.border.top * zoom),
        (int) (baseStyle.border.bottom * zoom));
    }

    //Linking and unlinking nodes.
    private void DrawLinkButtons(DialogueNode node, GUIStyle buttonStyle)
    {
        if(!linkingParentNode)
        {
            if(GUILayout.Button("Link", buttonStyle))
            {
                linkingParentNode = node;
            }
        }
        else if(linkingParentNode == node)
        {
            if(GUILayout.Button("Cancel", buttonStyle))
            {
                linkingParentNode = null;
            }
        }
        else if(linkingParentNode.GetChildren().Contains(node.name))
        {
            if(GUILayout.Button("Unlink", buttonStyle))
            {
                linkingParentNode.RemoveChild(node.name);

                linkingParentNode = null;
            }
        }
        else
        {
            if(GUILayout.Button("Child", buttonStyle))
            {
                linkingParentNode.AddChild(node.name);

                linkingParentNode = null;
            }
        }
    }

    //Draws the connections between nodes with Bezier curves.
    private void DrawConnections(DialogueNode node, float zoom)
    {
        Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y) * zoom;

        selectedDialogue.GetChildren(node, reusableChildrenList);

        foreach(DialogueNode childNode in reusableChildrenList)
        {
            Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y) * zoom;

            Vector3 controlPointOffset = endPosition - startPosition;

            controlPointOffset.y = 0f;

            controlPointOffset.x *= 0.8f;

            Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffset, endPosition - controlPointOffset, node == selectedNode ? Color.white : Color.gray, null, 4f);
        }
    }

    private DialogueNode GetNodeAtPoint(Vector2 point)
    {
        DialogueNode foundNode = null;

        List<DialogueNode> allNodes = selectedDialogue.GetAllNodes() as List<DialogueNode>;

        if(allNodes != null)
        {
            for(int i = allNodes.Count - 1; i >= 0; i--)
            {
                if(allNodes[i].GetRect().Contains(point))
                {
                    foundNode = allNodes[i];

                    break;
                }
            }
        }

        return foundNode;
    }
}
#endregion
