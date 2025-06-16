using System.Linq;
using UnityEditor;
using UnityEngine;

public class DialoguePostprocessor : AssetPostprocessor
{
    //Automatically called by Unity after assets are imported/created. Creates the root node for new Dialogue assets.
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach(string path in importedAssets)
        {
            Dialogue dialogue = AssetDatabase.LoadAssetAtPath<Dialogue>(path);

            if(dialogue != null && !dialogue.GetAllNodes().Any())
            {
                EditorApplication.delayCall += () =>
                {
                    if(dialogue != null)
                    {
                        if(!dialogue.GetAllNodes().Any())
                        {
                            Debug.Log($"Initializing new dialogue at: {path}");

                            dialogue.CreateNode(null);

                            EditorUtility.SetDirty(dialogue);

                            AssetDatabase.SaveAssets();
                        }
                    }
                };
            }
        }
    }
}
