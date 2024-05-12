using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public float parallaxFactor;
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    float amountTravelled = 0f;
    float height;
    public float width;
    void Start()
    {
        SetupParallax();
    }
    public void SetupParallax()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        Vector3 camPos = cameraTransform.position;
        Vector3 leftandbottomSideOfScreen = new Vector3(camPos.x + Screen.width * 0.0025f, camPos.y - Screen.height * 0.005f, 0f);
        float height = Screen.height * 0.004f * parallaxFactor;
        transform.localScale = new Vector3((height) / 3.24f, height/ 3.24f, 1f);
        //float width = height / (5.76f / 3.24f);
        transform.position = leftandbottomSideOfScreen + Vector3.up * MainScript.brickWidth + Vector3.up * height * 0.5f ;
        /*switch (parallaxFactor)
        {
            case 0:
                transform.localScale = new Vector3(Screen.width * 0.006f / 5.76f, Screen.height * 0.004f / 3.24f, 1f) ;
                transform.position = leftandbottomSideOfScreen + Vector3.up * MainScript.brickWidth + Vector3.up * Screen.height * 0.002f ;
                break;
            case 1:
                transform.localScale = new Vector3(Screen.width * 0.008f / 5.76f, Screen.height * 0.007f / 3.24f, 1f) ;
                transform.position = leftandbottomSideOfScreen + Vector3.up * MainScript.brickWidth + Vector3.up * Screen.height * 0.0035f ;
                break;
            case 2:
                transform.localScale = new Vector3(Screen.width * 0.01f / 5.76f, Screen.height * 0.008f / 3.24f, 1f);
                transform.position = leftandbottomSideOfScreen + Vector3.up * MainScript.brickWidth + Vector3.up * Screen.height * 0.004f ;
                break;
        }*/
    }
    public void UpdateParallax(float timePassed)
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        Vector3 actualMovement = new Vector3(deltaMovement.x * 1f / (parallaxFactor + 1f), 0, 0);
        transform.Translate(actualMovement, Space.World);
        amountTravelled += deltaMovement.x - actualMovement.x;
        lastCameraPosition = cameraTransform.position;
        if(cameraTransform.position.x - transform.position.x > Screen.width * 0.0025f)
        {
            //Debug.Log("getshere");
            transform.Translate(Vector3.right * width * transform.localScale.x,Space.World);
        }
    }
    void Update()
    {
        UpdateParallax(Time.deltaTime);
    }
}
