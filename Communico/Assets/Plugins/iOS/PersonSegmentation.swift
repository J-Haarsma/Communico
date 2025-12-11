import Foundation
import AVFoundation
import Vision
import UIKit

@objc public class PersonSegmentation: NSObject {

    static let shared = PersonSegmentation()

    private var session: AVCaptureSession?
    private var photoOutput: AVCapturePhotoOutput?

    override init() {
        super.init()

        NotificationCenter.default.addObserver(
            self,
            selector: #selector(handleCaptureRequest(_:)),
            name: NSNotification.Name("UnityRequestPhotoCapture"),
            object: nil
        )
    }

    @objc func handleCaptureRequest(_ n: Notification) {
        let index = (n.userInfo?["index"] as? Int) ?? 0
        capture(index: index)
    }

    func ensureSession() {
        if session != nil { return }

        let s = AVCaptureSession()
        s.sessionPreset = .photo

        guard let device = AVCaptureDevice.default(.builtInWideAngleCamera,
                                                   for: .video,
                                                   position: .back),
              let input = try? AVCaptureDeviceInput(device: device),
              s.canAddInput(input) else { return }

        s.addInput(input)

        let output = AVCapturePhotoOutput()
        guard s.canAddOutput(output) else { return }
        s.addOutput(output)

        self.photoOutput = output
        self.session = s

        s.startRunning()
    }

    func capture(index: Int) {
        ensureSession()

        guard let output = photoOutput else { return }

        let settings = AVCapturePhotoSettings()
        settings.isHighResolutionPhotoEnabled = true

        output.capturePhoto(with: settings,
                           delegate: PhotoHandler(index: index))
    }
}

// ----------------------------------------------------------
// MARK: - AVCapture Delegate
// ----------------------------------------------------------

class PhotoHandler: NSObject, AVCapturePhotoCaptureDelegate {

    let index: Int

    init(index: Int) {
        self.index = index
    }

    func photoOutput(_ output: AVCapturePhotoOutput,
                     didFinishProcessingPhoto photo: AVCapturePhoto,
                     error: Error?) {

        guard let cgImage = photo.cgImageRepresentation() else { return }

        let uiImage = UIImage(cgImage: cgImage)

        // Vision person segmentation request
        let request = VNGeneratePersonSegmentationRequest()
        request.outputPixelFormat = kCVPixelFormatType_OneComponent8

        let handler = VNImageRequestHandler(cgImage: uiImage.cgImage!, options: [:])

        var pngData: Data? = nil

        if (try? handler.perform([request])) != nil,
           let result = request.results?.first as? VNPixelBufferObservation,
           let maskedPNG = Self.applyMask(base: uiImage, mask: result.pixelBuffer) {
            pngData = maskedPNG
        } else {
            pngData = uiImage.pngData()
        }

        guard let finalData = pngData else { return }

        let b64 = finalData.base64EncodedString()
        let message = "\(index)|\(b64)"

        // Send result back to Unity
        message.withCString { cString in
            "MultiCameraFeed2".withCString { obj in
                "OnPhotoReadyIOS".withCString { method in
                    UnitySendMessage(obj, method, cString)
                }
            }
        }
    }

    // Apply segmentation mask
    static func applyMask(base: UIImage, mask: CVPixelBuffer) -> Data? {

        CVPixelBufferLockBaseAddress(mask, .readOnly)
        defer { CVPixelBufferUnlockBaseAddress(mask, .readOnly) }

        let context = CIContext()
        let baseCI = CIImage(cgImage: base.cgImage!)
        let maskCI = CIImage(cvPixelBuffer: mask)

        // Composite masked image
        let composite = baseCI.applyingFilter(
            "CIBlendWithAlphaMask",
            parameters: ["inputMaskImage": maskCI]
        )

        if let cg = context.createCGImage(composite, from: composite.extent) {
            return UIImage(cgImage: cg).pngData()
        }

        return nil
    }
}

// UnitySendMessage declaration
@_silgen_name("UnitySendMessage")
func UnitySendMessage(_ obj: UnsafePointer<CChar>,
                      _ method: UnsafePointer<CChar>,
                      _ msg: UnsafePointer<CChar>)

