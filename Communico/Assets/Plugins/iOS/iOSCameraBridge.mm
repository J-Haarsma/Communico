#import "iOSCameraBridge.h"
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface UnityBridgeCaller : NSObject
+ (void)startCamera;
+ (void)takePhoto:(int)index;
@end

@implementation UnityBridgeCaller

+ (void)startCamera {
    // Swift handles session startup automatically
}

+ (void)takePhoto:(int)index {
    // Call into Swift using NSNotification
    [[NSNotificationCenter defaultCenter]
        postNotificationName:@"UnityRequestPhotoCapture"
                      object:nil
                    userInfo:@{@"index": @(index)}];
}

@end

void StartiOSCamera() {
    [UnityBridgeCaller startCamera];
}

void TakePhotoWithBackgroundRemovalIOS(int index) {
    [UnityBridgeCaller takePhoto:index];
}

