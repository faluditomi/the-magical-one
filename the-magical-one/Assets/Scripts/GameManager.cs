using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;
    private bool isLevitationInProgress;

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

    public void StartLevitating()
    {
        isLevitationInProgress = true;
    }

    public void StopLevitating()
    {
        isLevitationInProgress = false;
    }

    public bool IsLevitationInProgress()
    {
        return isLevitationInProgress;
    }

}
