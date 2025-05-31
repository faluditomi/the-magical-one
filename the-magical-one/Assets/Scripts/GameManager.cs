using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;

    private bool isLevitationInProgress;
    private Transform currentLevitatingTransform;

    public static GameManager Instance()
    {
        if(_instance == null)
        {
            var obj = new GameObject("GameManager");
            _instance = obj.AddComponent<GameManager>();
            DontDestroyOnLoad(obj);
        }

        return _instance;
    }

    public void StartLevitating(Transform levitatingObject)
    {
        isLevitationInProgress = true;
        currentLevitatingTransform = levitatingObject;
    }

    public void StopLevitating()
    {
        isLevitationInProgress = false;
        currentLevitatingTransform = null;
    }

    public bool IsLevitationInProgress()
    {
        return isLevitationInProgress;
    }

    public Transform GetCurrentLevitatingTransform()
    {
        return currentLevitatingTransform;
    }

}
