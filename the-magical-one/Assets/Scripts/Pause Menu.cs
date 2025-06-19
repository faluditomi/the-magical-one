using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;

    private Bus masterBus;

    private int minutes;
    private int seconds;

    private bool isPaused;

    private CameraController cameraController;

    private void Awake()
    {
        //masterBus = RuntimeManager.GetBus("bus:/");
        cameraController = FindFirstObjectByType<CameraController>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void OnEnable()
    {
        ResumeGame();
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;

        Cursor.visible = true;

        isPaused = true;

        // Time.timeScale = 0;
        cameraController.enabled = false;

        // masterBus.setPaused(true);
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;

        isPaused = false;

        // Time.timeScale = 1;
        cameraController.enabled = true;

        // masterBus.setPaused(false);
    }

    public void RestartGame()
    {
        masterBus.setPaused(true);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        ResumeGame();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void VolumeSlider()
    {
        pausePanel.SetActive(false);
    }
}
