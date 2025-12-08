// //using UnityEngine;
// //using UnityEngine.UI;

// //public class LiveCameraFeed : MonoBehaviour
// //{
// //    [Header("Camera Input")]
// //    public RawImage cameraDisplay; // ALLEEN de live preview

// //    [Header("Photo Outputs")]
// //    public RawImage photoPreviewCanvasA;  // laat foto zien
// //    public RawImage photoPreviewCanvasB;  // laat foto zien
// //    public RawImage[] stripFrames;        // strip RawImages

// //    private WebCamTexture webcam;
// //    private int currentStripIndex = 0;

// //    void Start()
// //    {
// //        webcam = new WebCamTexture();
// //        cameraDisplay.texture = webcam; // Alleen live camera hier!
// //        webcam.Play();
// //    }

// //    // PHOTO
// //    public void TakePhoto()
// //    {
// //        Texture2D photo = new Texture2D(webcam.width, webcam.height);
// //        photo.SetPixels(webcam.GetPixels());
// //        photo.Apply();

// //        // Toon de foto op beide canvassen
// //        if (photoPreviewCanvasA != null)
// //            photoPreviewCanvasA.texture = photo;

// //        if (photoPreviewCanvasB != null)
// //            photoPreviewCanvasB.texture = photo;

// //        // Voeg toe aan strip
// //        if (currentStripIndex < stripFrames.Length)
// //        {
// //            stripFrames[currentStripIndex].texture = photo;
// //            currentStripIndex++;
// //        }
// //    }

// //    void OnDestroy()
// //    {
// //        if (webcam != null) webcam.Stop();
// //    }
// //}
// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

// public class MultiCameraFeed : MonoBehaviour
// {
//     [Header("Live camera preview panels")]
//     public RawImage[] liveCameraPanels;

//     [Header("Photo output panels")]
//     public RawImage[] photoSlots;

//     [Header("Final Combined Output")]
//     public RawImage[] combinedPhotoSlots; // 5 slots in combinedPanel

//     [Header("Combined Panel Canvas")]
//     public GameObject fullStripCanvas; // Het Canvas dat Full-Strip heet

//     private WebCamTexture webcam;
//     private Texture2D[] savedPhotos;

//     void Start()
//     {
//         StartCamera();
//         savedPhotos = new Texture2D[photoSlots.Length];
//     }

//     void StartCamera()
//     {
//         if (WebCamTexture.devices.Length == 0)
//         {
//             Debug.LogError("Geen camera gevonden!");
//             return;
//         }

//         webcam = new WebCamTexture(WebCamTexture.devices[0].name);
//         webcam.Play();

//         foreach (RawImage panel in liveCameraPanels)
//         {
//             panel.texture = webcam;
//         }
//     }

//     public void TakePhoto(int slotIndex)
//     {
//         StartCoroutine(CapturePhoto(slotIndex));
//     }

//     IEnumerator CapturePhoto(int index)
//     {
//         yield return new WaitForEndOfFrame();

//         Texture2D photo = new Texture2D(webcam.width, webcam.height);
//         photo.SetPixels(webcam.GetPixels());
//         photo.Apply();

//         savedPhotos[index] = photo;
//         photoSlots[index].texture = savedPhotos[index];
//     }

//     // Toon alle gemaakte foto's in de combinedPanel slots
//     public void ShowCombinedPhotos()
//     {
//         for (int i = 0; i < combinedPhotoSlots.Length; i++)
//         {
//             if (i < savedPhotos.Length && savedPhotos[i] != null)
//             {
//                 combinedPhotoSlots[i].texture = savedPhotos[i];
//                 combinedPhotoSlots[i].gameObject.SetActive(true);
//             }
//             else
//             {
//                 combinedPhotoSlots[i].gameObject.SetActive(false);
//             }
//         }
//     }

//     // Automatisch aanroepen wanneer Full-Strip Canvas actief wordt
//     void OnEnable()
//     {
//         if (fullStripCanvas != null && fullStripCanvas.activeSelf)
//         {
//             ShowCombinedPhotos();
//         }
//     }

//     void OnDestroy()
//     {
//         if (webcam != null)
//             webcam.Stop();
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiCameraFeed : MonoBehaviour
{
    [Header("Live camera preview panels")]
    public RawImage[] liveCameraPanels;      // panels that show the live webcam feed

    [Header("Photo output panels (per step)")]
    public RawImage[] photoSlots;            // panels that show the photo immediately after capture

    [Header("Final Combined Output (full strip)")]
    public RawImage[] combinedPhotoSlots;    // slots on the final full-strip page

    private WebCamTexture webcam;
    private Texture2D[] savedPhotos;         // store all taken photos here

    void Start()
    {
        StartCamera();

        // Make sure we can store at least as many photos as we have slots
        int maxSlots = Mathf.Max(photoSlots.Length, combinedPhotoSlots.Length);
        savedPhotos = new Texture2D[maxSlots];
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

        // Show the live feed on all preview panels
        foreach (RawImage panel in liveCameraPanels)
        {
            if (panel != null)
                panel.texture = webcam;
        }
    }

    /// <summary>
    /// Called by your buttons, passing the index of the slot to fill (0,1,2,...)
    /// </summary>
    public void TakePhoto(int slotIndex)
    {
        StartCoroutine(CapturePhoto(slotIndex));
    }

    private IEnumerator CapturePhoto(int index)
    {
        // Wait until end of frame to read pixels
        yield return new WaitForEndOfFrame();

        if (webcam == null || !webcam.isPlaying)
            yield break;

        Texture2D photo = new Texture2D(webcam.width, webcam.height, TextureFormat.RGB24, false);
        photo.SetPixels(webcam.GetPixels());
        photo.Apply();

        // Store it
        if (index >= 0 && index < savedPhotos.Length)
        {
            savedPhotos[index] = photo;
        }

        // Show in the per-step photo slot (e.g. Pic-Strip (N))
        if (index >= 0 && index < photoSlots.Length && photoSlots[index] != null)
        {
            photoSlots[index].texture = photo;
            photoSlots[index].gameObject.SetActive(true);
        }

        // ALSO immediately show it in the final combined strip slot
        if (index >= 0 && index < combinedPhotoSlots.Length && combinedPhotoSlots[index] != null)
        {
            combinedPhotoSlots[index].texture = photo;
            combinedPhotoSlots[index].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Optional: if you ever need to refresh the final strip manually,
    /// you can call this (for example from the Full-Strip panel's OnEnable).
    /// </summary>
    public void ShowCombinedPhotos()
    {
        for (int i = 0; i < combinedPhotoSlots.Length; i++)
        {
            bool hasPhoto = (i < savedPhotos.Length && savedPhotos[i] != null);

            if (i < combinedPhotoSlots.Length && combinedPhotoSlots[i] != null)
            {
                if (hasPhoto)
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
    }

    void OnDestroy()
    {
        if (webcam != null)
            webcam.Stop();
    }
}
