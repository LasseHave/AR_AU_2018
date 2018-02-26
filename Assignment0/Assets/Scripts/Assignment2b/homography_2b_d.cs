using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using Vuforia;

public class homography_2b_d : MonoBehaviour {

	Mat grayScale;
	Mat threshold;
	Mat cameraImageMat;
	Mat cameraImageBlurMat = new Mat();
	Texture2D imgTexture;
	public static bool allCubiesScaned = false;
	public double thresholdValue = 160;



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
				threshold = new Mat(cameraImageRaw.Height, cameraImageRaw.Width, CvType.CV_8UC4);

			}

			// Threshold and Grayscale
			byte[] pixels = cameraImageRaw.Pixels;
			cameraImageMat.put (0, 0, pixels);
			Imgproc.cvtColor (cameraImageMat, grayScale, Imgproc.COLOR_RGB2GRAY);
			Imgproc.threshold(grayScale, threshold, thresholdValue, 0, Imgproc.THRESH_TOZERO);


			// Find contours and draw
			List<MatOfPoint> contours = new List<MatOfPoint>();
			List<MatOfPoint> ls_mop = new List<MatOfPoint>();
			Mat hierarchy = new Mat();
			Imgproc.findContours(threshold, ls_mop, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
			Imgproc.drawContours(threshold, ls_mop, -1, new Scalar(200, 0, 200), 2, 1, hierarchy, 1000,new Point(0,0));

			// Approx polygons - From this part im not so sure whether it works.
			MatOfPoint2f matOfPoint2f = new MatOfPoint2f();
			MatOfPoint2f approxCurve = new MatOfPoint2f();
			Imgproc.approxPolyDP(matOfPoint2f, approxCurve, Imgproc.arcLength(matOfPoint2f, true) * 0.02, true);



			MatDisplay.DisplayMat (threshold, MatDisplaySettings.FULL_BACKGROUND);
		}
			
	}

	private double angle(Point pt1, Point pt2, Point pt0)
	{
		double dx1 = pt1.x - pt0.x;
		double dy1 = pt1.y - pt0.y;
		double dx2 = pt2.x - pt0.x;
		double dy2 = pt2.y - pt0.y;
		return (dx1 * dx2 + dy1 * dy2) / System.Math.Sqrt((dx1 * dx1 + dy1 * dy1) * (dx2 * dx2 + dy2 * dy2) + 1e-10);
	}
}
