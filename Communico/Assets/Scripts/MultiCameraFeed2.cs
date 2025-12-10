using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;

public class MultiCameraFeed2: MonoBehaviour
{
    [Header("Live camera preview panels")]
    public RawImage[] liveCameraPanels;

    [Header("Photo output panels (per step)")]
    public RawImage[] photoSlots;

    [Header("Final Combined Output (full strip)")]
    public RawImage[] combinedPhotoSlots;

    private WebCamTexture webcam;
    private Texture2D[] savedPhotos;

#if UNITY_IOS
    // Native plugin call
    [DllImport("__Internal")]
    private static extern void StartiOSCamera();

    [DllImport("__Internal")]
    private static extern void TakePhotoWithBackgroundRemovalIOS(int index);
#endif

    void Start()
    {
        int maxSlots = Mathf.Max(photoSlots.Length, combinedPhotoSlots.Length);
        savedPhotos = new Texture2D[maxSlots];

#if UNITY_IOS
        // iPad: use native Vision-framework camera
        StartiOSCamera();
#else
        // Editor/Windows/Android fallback
        StartUnityCamera();
#endif
    }

    void StartUnityCamera()
    {
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("Geen camera gevonden!");
            return;
        }

        // Choose BACK camera if available
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

        webcam = new WebCamTexture(camName);
        webcam.Play();

        foreach (RawImage panel in liveCameraPanels)
            if (panel != null)
                panel.texture = webcam;
    }

    public void TakePhoto(int slotIndex)
    {
#if UNITY_IOS
        TakePhotoWithBackgroundRemovalIOS(slotIndex);
#else
        StartCoroutine(CapturePhoto_Unity(slotIndex));
#endif
    }

    private IEnumerator CapturePhoto_Unity(int index)
    {
        yield return new WaitForEndOfFrame();

        if (webcam == null || !webcam.isPlaying)
            yield break;

        Texture2D photo = new Texture2D(webcam.width, webcam.height, TextureFormat.ARGB32, false);
        photo.SetPixels(webcam.GetPixels());
        photo.Apply();

        AssignPhoto(index, photo);
    }

    // Called from native iOS plugin (Objective-C)
    public void OnPhotoReadyIOS(string message)
    {
        // message = "index|base64_png"
        string[] parts = message.Split('|');
        int index = int.Parse(parts[0]);
        byte[] pngBytes = System.Convert.FromBase64String(parts[1]);

        Texture2D photo = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        photo.LoadImage(pngBytes);

        AssignPhoto(index, photo);
    }

    private void AssignPhoto(int index, Texture2D photo)
    {
        if (index >= 0 && index < savedPhotos.Length)
            savedPhotos[index] = photo;

        if (index < photoSlots.Length && photoSlots[index] != null)
        {
            photoSlots[index].texture = photo;
            photoSlots[index].gameObject.SetActive(true);
        }

        if (index < combinedPhotoSlots.Length && combinedPhotoSlots[index] != null)
        {
            combinedPhotoSlots[index].texture = photo;
            combinedPhotoSlots[index].gameObject.SetActive(true);
        }
    }

    void OnDestroy()
    {
        if (webcam != null)
            webcam.Stop();
    }
}

