using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using Vuforia;

public class LiveCodeScript : MonoBehaviour {

	Mat cameraImageMat;
	Mat cameraImageBlurMat = new Mat();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		MatDisplay.SetCameraFoV (40.5f);

		Image cameraImageRaw = CameraDevice.Instance.GetCameraImage(
			Image.PIXEL_FORMAT.RGBA8888);
		if (cameraImageRaw != null) {
			if (cameraImageMat == null) {
				// Rows first, then columns.
				cameraImageMat = new Mat (cameraImageRaw.Height, cameraImageRaw.Width, CvType.CV_8UC4);
			}
			byte[] pixels = cameraImageRaw.Pixels;
			cameraImageMat.put (0, 0, pixels);

			MatDisplay.DisplayMat (cameraImageMat, MatDisplaySettings.BOTTOM_LEFT);
			Imgproc.blur(cameraImageMat, cameraImageBlurMat, new Size(12,12));
			MatDisplay.DisplayMat (cameraImageBlurMat, MatDisplaySettings.FULL_BACKGROUND);

		}
	}
}
