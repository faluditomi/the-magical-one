using UnityEngine;

public class CameraBoxCollisionControllerExtravaganza : MonoBehaviour
{
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag.Equals("CameraBox"))
        {
            collision.transform.Find("Kid Drawing").gameObject.SetActive(true);
            gameManager.SetCameraHacked();
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<LevitateBehaviour>().enabled = false;
            transform.Find("HoverParticles").gameObject.SetActive(false);
        }
    }
}
