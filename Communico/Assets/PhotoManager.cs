using UnityEngine;
using UnityEngine.UI;

public class LiveCameraFeed : MonoBehaviour
{
    [Header("Camera Feed Outputs")]
    public RawImage[] cameraDisplays;  // 5 panels met live feed

    [Header("Photo Strip Outputs")]
    public RawImage[] stripFrames;     // foto’s hiernaartoe

    private WebCamTexture webcam;
    private int currentStripIndex = 0;

    void Start()
    {
        webcam = new WebCamTexture();
        webcam.Play();

        // Geef alle panels dezelfde live feed
        foreach (var display in cameraDisplays)
            if (display != null)
                display.texture = webcam;
    }

    public void TakePhoto()
    {
        Texture2D photo = new Texture2D(webcam.width, webcam.height);
        photo.SetPixels(webcam.GetPixels());
        photo.Apply();

        // Stop de foto in de strip
        if (currentStripIndex < stripFrames.Length)
        {
            stripFrames[currentStripIndex].texture = photo;
            currentStripIndex++;
        }
    }

    private void OnDestroy()
    {
        if (webcam != null) webcam.Stop();
    }
}

