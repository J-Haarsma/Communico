import UIKit
import AVFoundation

@objc public class UnityCameraManager: NSObject {

    @objc public static let shared = UnityCameraManager()

    private var session: AVCaptureSession?
    private var output: AVCapturePhotoOutput?
    private var previewLayer: AVCaptureVideoPreviewLayer?
    private var previewView: UIView?

    @objc public func startCamera(x: CGFloat, y: CGFloat, width: CGFloat, height: CGFloat) {
        if session == nil {
            let session = AVCaptureSession()
            session.sessionPreset = .photo

            guard let device = AVCaptureDevice.default(.builtInWideAngleCamera,
                                                       for: .video,
                                                       position: .back),
                  let input = try? AVCaptureDeviceInput(device: device)
            else { return }

            session.addInput(input)

            let output = AVCapturePhotoOutput()
            session.addOutput(output)
            self.output = output
            self.session = session

            previewView = UIView(frame: CGRect(x: x, y: y, width: width, height: height))
            previewView?.clipsToBounds = true

            let layer = AVCaptureVideoPreviewLayer(session: session)
            layer.videoGravity = .resizeAspectFill
            layer.frame = previewView!.bounds
            previewView!.layer.addSublayer(layer)

            previewLayer = layer

            DispatchQueue.main.async {
                if let root = UIApplication.shared.windows.first?.rootViewController?.view {
                    root.addSubview(self.previewView!)
                }
            }

            session.startRunning()
        }

        updateCameraRect(x: x, y: y, width: width, height: height)
    }

    @objc public func updateCameraRect(x: CGFloat, y: CGFloat, width: CGFloat, height: CGFloat) {
        DispatchQueue.main.async {
            self.previewView?.frame = CGRect(x: x, y: y, width: width, height: height)
            self.previewLayer?.frame = self.previewView?.bounds ?? .zero
        }
    }

    @objc public func capturePhoto(index: Int) {
        let settings = AVCapturePhotoSettings()
        output?.capturePhoto(with: settings, delegate: PhotoDelegate(index: index))
    }

    @objc public func stopCamera() {
        session?.stopRunning()
        previewView?.removeFromSuperview()
        session = nil
        output = nil
        previewLayer = nil
    }
}

class PhotoDelegate: NSObject, AVCapturePhotoCaptureDelegate {
    let index: Int

    init(index: Int) { self.index = index }

    func photoOutput(_ output: AVCapturePhotoOutput,
                     didFinishProcessingPhoto photo: AVCapturePhoto,
                     error: Error?) {

        guard let data = photo.fileDataRepresentation() else { return }
        let b64 = data.base64EncodedString()

        let msg = "\(index)|\(b64)"

        msg.withCString { cStr in
            "SimpleIOSCamera".withCString { obj in
                "OnPhotoReadyIOS".withCString { method in
                    UnitySendMessage(obj, method, cStr)
                }
            }
        }
    }
}

@_silgen_name("UnitySendMessage")
func UnitySendMessage(_ obj: UnsafePointer<CChar>,
                      _ method: UnsafePointer<CChar>,
                      _ msg: UnsafePointer<CChar>)

