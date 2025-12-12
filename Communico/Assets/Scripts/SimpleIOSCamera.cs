using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class SimpleIOSCamera : MonoBehaviour
{
    [Header("Live Camera RawImage")]
    public RawImage liveCameraPanel;

    [Header("Photo Slots (5 total)")]
    public RawImage[] photoSlots;

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void StartiOSCamera(float x, float y, float width, float height);

    [DllImport("__Internal")]
    private static extern void UpdateCameraRect(float x, float y, float width, float height);

    [DllImport("__Internal")]
    private static extern void TakePhotoIOS(int index);

    [DllImport("__Internal")]
    private static extern void StopIOSCamera();
#endif

    private Texture2D[] savedPhotos;

    void Start()
    {
        savedPhotos = new Texture2D[photoSlots.Length];

#if UNITY_IOS
        UpdatePreviewRect();
#endif
    }

    void Update()
    {
#if UNITY_IOS
        UpdatePreviewRect(); // follow UI transitions
#endif
    }

    private void UpdatePreviewRect()
    {
        if (liveCameraPanel == null) return;

        RectTransform rt = liveCameraPanel.rectTransform;

        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        float x = corners[0].x;
        float y = Screen.height - corners[1].y; // convert to UIKit coords
        float width = rt.rect.width * rt.lossyScale.x;
        float height = rt.rect.height * rt.lossyScale.y;

        StartiOSCamera(x, y, width, height);
        UpdateCameraRect(x, y, width, height);
    }

    public void TakePhoto(int index)
    {
#if UNITY_IOS
        TakePhotoIOS(index);
#endif
    }

    public void OnPhotoReadyIOS(string message)
    {
        string[] parts = message.Split('|');
        int index = int.Parse(parts[0]);
        byte[] png = System.Convert.FromBase64String(parts[1]);

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(png);

        photoSlots[index].texture = tex;
        savedPhotos[index] = tex;
    }

    void OnDestroy()
    {
#if UNITY_IOS
        StopIOSCamera();
#endif
    }
}


