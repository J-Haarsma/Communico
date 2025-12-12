using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class SimpleIOSCamera : MonoBehaviour
{
    public RawImage liveCameraPanel;
    public RawImage[] photoSlots;
    private Texture2D[] savedPhotos;

#if UNITY_IOS
    [DllImport("__Internal")] private static extern void StartiOSCamera(float x, float y, float width, float height);
    [DllImport("__Internal")] private static extern void UpdateCameraRect(float x, float y, float width, float height);
    [DllImport("__Internal")] private static extern void TakePhotoIOS(int index);
    [DllImport("__Internal")] private static extern void StopIOSCamera();
#endif

    void Start()
    {
        savedPhotos = new Texture2D[photoSlots.Length];
        UpdatePreviewRect();
    }

    void Update()
    {
        UpdatePreviewRect(); // follow UI movement
    }

    void UpdatePreviewRect()
    {
        if (!liveCameraPanel) return;
        RectTransform rt = liveCameraPanel.rectTransform;
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        float x = corners[0].x;
        float y = Screen.height - corners[1].y;
        float width = rt.rect.width * rt.lossyScale.x;
        float height = rt.rect.height * rt.lossyScale.y;

#if UNITY_IOS
        StartiOSCamera(x, y, width, height);
        UpdateCameraRect(x, y, width, height);
#endif
    }

    public void TakePhoto(int index)
    {
#if UNITY_IOS
        TakePhotoIOS(index);
#endif
    }

    public void OnPhotoReadyIOS(string base64)
    {
        byte[] png = System.Convert.FromBase64String(base64);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(png);
        for (int i = 0; i < photoSlots.Length; i++)
        {
            if (photoSlots[i] == null) continue;
            photoSlots[i].texture = tex;
        }
    }

    void OnDestroy()
    {
#if UNITY_IOS
        StopIOSCamera();
#endif
    }
}



