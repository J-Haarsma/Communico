using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleCameraUnity : MonoBehaviour
{
    public RawImage liveCameraPanel;      // live feed
    public RawImage[] photoSlots;         // photos taken
    private WebCamTexture webcam;
    private Texture2D[] savedPhotos;

    void Start()
    {
        savedPhotos = new Texture2D[photoSlots.Length];

        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("No camera found on this device!");
            return;
        }

        // Pick the back camera if available
        WebCamDevice? backCam = null;
        foreach (var cam in WebCamTexture.devices)
        {
            if (!cam.isFrontFacing)
            {
                backCam = cam;
                break;
            }
        }

        string camName = backCam.HasValue ? backCam.Value.name : WebCamTexture.devices[0].name;

        webcam = new WebCamTexture(camName, (int)liveCameraPanel.rectTransform.rect.width, (int)liveCameraPanel.rectTransform.rect.height);
        liveCameraPanel.texture = webcam;
        webcam.Play();
    }

    public void TakePhoto(int slotIndex)
    {
        if (webcam == null || !webcam.isPlaying) return;
        if (slotIndex < 0 || slotIndex >= photoSlots.Length) return;

        Texture2D photo = new Texture2D(webcam.width, webcam.height, TextureFormat.RGBA32, false);
        photo.SetPixels(webcam.GetPixels());
        photo.Apply();

        photoSlots[slotIndex].texture = photo;
        savedPhotos[slotIndex] = photo;
    }

    void OnDestroy()
    {
        if (webcam != null && webcam.isPlaying)
            webcam.Stop();
    }
}

