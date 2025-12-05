//using UnityEngine;
//using UnityEngine.UI;

//public class LiveCameraFeed : MonoBehaviour
//{
//    [Header("Camera Input")]
//    public RawImage cameraDisplay; // ALLEEN de live preview

//    [Header("Photo Outputs")]
//    public RawImage photoPreviewCanvasA;  // laat foto zien
//    public RawImage photoPreviewCanvasB;  // laat foto zien
//    public RawImage[] stripFrames;        // strip RawImages

//    private WebCamTexture webcam;
//    private int currentStripIndex = 0;

//    void Start()
//    {
//        webcam = new WebCamTexture();
//        cameraDisplay.texture = webcam; // Alleen live camera hier!
//        webcam.Play();
//    }

//    // PHOTO
//    public void TakePhoto()
//    {
//        Texture2D photo = new Texture2D(webcam.width, webcam.height);
//        photo.SetPixels(webcam.GetPixels());
//        photo.Apply();

//        // Toon de foto op beide canvassen
//        if (photoPreviewCanvasA != null)
//            photoPreviewCanvasA.texture = photo;

//        if (photoPreviewCanvasB != null)
//            photoPreviewCanvasB.texture = photo;

//        // Voeg toe aan strip
//        if (currentStripIndex < stripFrames.Length)
//        {
//            stripFrames[currentStripIndex].texture = photo;
//            currentStripIndex++;
//        }
//    }

//    void OnDestroy()
//    {
//        if (webcam != null) webcam.Stop();
//    }
//}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiCameraFeed : MonoBehaviour
{
    [Header("Live camera preview panels")]
    public RawImage[] liveCameraPanels;

    [Header("Photo output panels")]
    public RawImage[] photoSlots;

    [Header("Final Combined Output")]
    public RawImage[] combinedPhotoSlots; // 5 slots in combinedPanel

    [Header("Combined Panel Canvas")]
    public GameObject fullStripCanvas; // Het Canvas dat Full-Strip heet

    private WebCamTexture webcam;
    private Texture2D[] savedPhotos;

    void Start()
    {
        StartCamera();
        savedPhotos = new Texture2D[photoSlots.Length];
    }

    void StartCamera()
    {
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("Geen camera gevonden!");
            return;
        }

        webcam = new WebCamTexture(WebCamTexture.devices[0].name);
        webcam.Play();

        foreach (RawImage panel in liveCameraPanels)
        {
            panel.texture = webcam;
        }
    }

    public void TakePhoto(int slotIndex)
    {
        StartCoroutine(CapturePhoto(slotIndex));
    }

    IEnumerator CapturePhoto(int index)
    {
        yield return new WaitForEndOfFrame();

        Texture2D photo = new Texture2D(webcam.width, webcam.height);
        photo.SetPixels(webcam.GetPixels());
        photo.Apply();

        savedPhotos[index] = photo;
        photoSlots[index].texture = savedPhotos[index];
    }

    // Toon alle gemaakte foto's in de combinedPanel slots
    public void ShowCombinedPhotos()
    {
        for (int i = 0; i < combinedPhotoSlots.Length; i++)
        {
            if (i < savedPhotos.Length && savedPhotos[i] != null)
            {
                combinedPhotoSlots[i].texture = savedPhotos[i];
                combinedPhotoSlots[i].gameObject.SetActive(true);
            }
            else
            {
                combinedPhotoSlots[i].gameObject.SetActive(false);
            }
        }
    }

    // Automatisch aanroepen wanneer Full-Strip Canvas actief wordt
    void OnEnable()
    {
        if (fullStripCanvas != null && fullStripCanvas.activeSelf)
        {
            ShowCombinedPhotos();
        }
    }

    void OnDestroy()
    {
        if (webcam != null)
            webcam.Stop();
    }
}
