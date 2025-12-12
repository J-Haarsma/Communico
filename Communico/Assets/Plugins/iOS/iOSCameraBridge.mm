#import "iOSCameraBridge.h"
#import <UIKit/UIKit.h>
#import "UnityInterface.h"
#import <AVFoundation/AVFoundation.h>

@interface CameraManager : NSObject <AVCapturePhotoCaptureDelegate>
@property (nonatomic, strong) AVCaptureSession *session;
@property (nonatomic, strong) AVCapturePhotoOutput *photoOutput;
@property (nonatomic, strong) AVCaptureVideoPreviewLayer *previewLayer;
@property (nonatomic, strong) UIView *previewView;
@end

@implementation CameraManager

+ (instancetype)shared {
    static CameraManager *instance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[CameraManager alloc] init];
    });
    return instance;
}

- (void)startCamera:(CGRect)rect {
    if (!self.session) {
        self.session = [[AVCaptureSession alloc] init];
        self.session.sessionPreset = AVCaptureSessionPresetPhoto;

        AVCaptureDevice *device = [AVCaptureDevice defaultDeviceWithMediaType:AVMediaTypeVideo];
        if (!device) return;

        NSError *error = nil;
        AVCaptureDeviceInput *input = [AVCaptureDeviceInput deviceInputWithDevice:device error:&error];
        if (input && [self.session canAddInput:input]) {
            [self.session addInput:input];
        }

        self.photoOutput = [[AVCapturePhotoOutput alloc] init];
        if ([self.session canAddOutput:self.photoOutput]) {
            [self.session addOutput:self.photoOutput];
        }

        self.previewView = [[UIView alloc] initWithFrame:rect];
        self.previewView.clipsToBounds = YES;

        self.previewLayer = [AVCaptureVideoPreviewLayer layerWithSession:self.session];
        self.previewLayer.videoGravity = AVLayerVideoGravityResizeAspectFill;
        self.previewLayer.frame = self.previewView.bounds;
        [self.previewView.layer addSublayer:self.previewLayer];

        dispatch_async(dispatch_get_main_queue(), ^{
            UIView *rootView = [UIApplication sharedApplication].keyWindow.rootViewController.view;
            [rootView addSubview:self.previewView];
        });

        [self.session startRunning];
    } else {
        [self updateCameraRect:rect];
    }
}

- (void)updateCameraRect:(CGRect)rect {
    dispatch_async(dispatch_get_main_queue(), ^{
        self.previewView.frame = rect;
        self.previewLayer.frame = self.previewView.bounds;
    });
}

- (void)capturePhoto:(int)index {
    if (!self.photoOutput) return;

    AVCapturePhotoSettings *settings = [AVCapturePhotoSettings photoSettings];
    [self.photoOutput capturePhotoWithSettings:settings delegate:self];
    objc_setAssociatedObject(settings, @"photoIndex", @(index), OBJC_ASSOCIATION_RETAIN_NONATOMIC);
}

- (void)stopCamera {
    [self.session stopRunning];
    [self.previewView removeFromSuperview];
    self.session = nil;
    self.photoOutput = nil;
    self.previewLayer = nil;
    self.previewView = nil;
}

#pragma mark - AVCapturePhotoCaptureDelegate

- (void)captureOutput:(AVCapturePhotoOutput *)output
didFinishProcessingPhoto:(AVCapturePhoto *)photo
                 error:(NSError *)error {
    NSNumber *indexNum = objc_getAssociatedObject(photo.resolvedSettings, @"photoIndex");
    int index = indexNum ? [indexNum intValue] : 0;

    NSData *data = [photo fileDataRepresentation];
    if (!data) return;

    NSString *b64 = [data base64EncodedStringWithOptions:0];

    UnitySendMessage("SimpleIOSCamera", "OnPhotoReadyIOS", [b64 UTF8String]);
}

@end

// C functions exposed to Unity

void StartiOSCamera(float x, float y, float width, float height) {
    CGRect rect = CGRectMake(x, y, width, height);
    [[CameraManager shared] startCamera:rect];
}

void UpdateCameraRect(float x, float y, float width, float height) {
    CGRect rect = CGRectMake(x, y, width, height);
    [[CameraManager shared] updateCameraRect:rect];
}

void TakePhotoIOS(int index) {
    [[CameraManager shared] capturePhoto:index];
}

void StopIOSCamera() {
    [[CameraManager shared] stopCamera];
}

