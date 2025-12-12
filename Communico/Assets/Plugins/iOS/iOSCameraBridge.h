#import <Foundation/Foundation.h>

#ifdef __cplusplus
extern "C" {
#endif

void StartiOSCamera(float x, float y, float width, float height);
void UpdateCameraRect(float x, float y, float width, float height);
void TakePhotoIOS(int index);
void StopIOSCamera();

#ifdef __cplusplus
}
#endif

