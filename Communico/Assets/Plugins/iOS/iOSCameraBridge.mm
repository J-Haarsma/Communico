#import "iOSCameraBridge.h"
#import <UIKit/UIKit.h>
#import "UnityInterface.h"

#import "UnityCameraManager-Swift.h"

void StartiOSCamera(float x, float y, float width, float height) {
    [[UnityCameraManager shared] startCameraWithX:x y:y width:width height:height];
}

void UpdateCameraRect(float x, float y, float width, float height) {
    [[UnityCameraManager shared] updateCameraRectWithX:x y:y width:width height:height];
}

void TakePhotoIOS(int index) {
    [[UnityCameraManager shared] capturePhotoWithIndex:index];
}

void StopIOSCamera() {
    [[UnityCameraManager shared] stopCamera];
}
