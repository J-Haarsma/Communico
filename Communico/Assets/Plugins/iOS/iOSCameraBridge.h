#import <Foundation/Foundation.h>

#ifdef __cplusplus
extern "C" {
#endif

// Start the camera preview in a rectangle
void StartiOSCamera(float x, float y, float width, float height);

// Update the preview rectangle
void UpdateCameraRect(float x, float y, float width, float height);

// Take a photo and return it to Unity
void TakePhotoIOS(int index);

// Stop the camera
void StopIOSCamera();

#ifdef __cplusplus
}
#endif


