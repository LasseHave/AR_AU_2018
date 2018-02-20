using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using Vuforia;

public class visual_c : MonoBehaviour {

	Mat grayScale;
	Mat cameraImageMat;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		MatDisplay.SetCameraFoV (41.5f);

		Image cameraImageRaw = CameraDevice.Instance.GetCameraImage(
			Image.PIXEL_FORMAT.RGBA8888);
		if (cameraImageRaw != null) {
			if (cameraImageMat == null) {
				// Rows first, then columns.
				cameraImageMat = new Mat (cameraImageRaw.Height, cameraImageRaw.Width, CvType.CV_8UC4);
				grayScale = new Mat(cameraImageRaw.Height, cameraImageRaw.Width, CvType.CV_8UC4);
			}
				
			byte[] pixels = cameraImageRaw.Pixels;
			cameraImageMat.put (0, 0, pixels);
			Imgproc.cvtColor (cameraImageMat, grayScale, Imgproc.COLOR_RGB2GRAY);

			Imgproc.adaptiveThreshold (grayScale, grayScale, 255, Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C, Imgproc.THRESH_BINARY, 7, 7);
			MatDisplay.DisplayMat (grayScale, MatDisplaySettings.FULL_BACKGROUND);
		}
	}
}
