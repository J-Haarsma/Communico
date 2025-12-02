using UnityEngine;
using UnityEngine.UI;

public class LiveCameraFeed : MonoBehaviour
{
    public RawImage cameraDisplay;
    private WebCamTexture webcam;

    void Start()
    {
        webcam = new WebCamTexture();
        cameraDisplay.texture = webcam;
        webcam.Play();
    }

    public Texture2D CapturePhoto()
    {
        Texture2D photo = new Texture2D(webcam.width, webcam.height);
        photo.SetPixels(webcam.GetPixels());
        photo.Apply();
        return photo;
    }

    void OnDestroy()
    {
        if (webcam != null)
            webcam.Stop();
    }
}
