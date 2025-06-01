using UnityEngine;

public class CameraBoxCollisionControllerExtravaganza : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag.Equals("CameraBox"))
        {
            collision.transform.Find("Kid Drawing").gameObject.SetActive(true);
            //set cam hacked bool in game manager and make nurse not come in anymore
            Destroy(gameObject);
        }
    }
}
