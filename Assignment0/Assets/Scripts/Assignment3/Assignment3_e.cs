using UnityEngine;
using OpenCVForUnity;
using Vuforia;
using System.Collections;
using System.Collections.Generic;

public class Assignment3_e : MonoBehaviour {

	Mat cameraImageMat;

	[Range(0f, 180f)]
	public int minH = 108;
	[Range(0f, 180f)]
	public int maxH = 133;
	[Range(0f, 255f)]
	public int minS = 53;
	[Range(0f, 255f)]
	public int maxS = 113;
	[Range(0f, 255f)]
	public int minV = 178;
	[Range(0f, 255f)]
	public int maxV = 222;

	public int blurSize = 5;
	public int elementSize = 7;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		MatDisplay.SetCameraFoV (41.5f);

		Image cameraImageRaw = CameraDevice.Instance.GetCameraImage(Image.PIXEL_FORMAT.RGBA8888);
		if (cameraImageRaw != null) {
			if (cameraImageMat == null) {
				// Rows first, then columns.
				cameraImageMat = new Mat (cameraImageRaw.Height, cameraImageRaw.Width, CvType.CV_8UC4);
			}

			byte[] pixels = cameraImageRaw.Pixels;
			cameraImageMat.put (0, 0, pixels);

			/// Preprocessing
			// HSV
			Mat hsv = new Mat();
			Imgproc.cvtColor (cameraImageMat, hsv, Imgproc.COLOR_BGR2HSV);
			Core.inRange (hsv, new Scalar(minH, minS, minV), new Scalar(maxH, maxS, maxV), hsv);
			// Blur
			Imgproc.medianBlur(hsv, hsv, blurSize);
			Mat element = Imgproc.getStructuringElement (Imgproc.MORPH_ELLIPSE, new Size (2 * elementSize + 1, 2 * elementSize + 1), new Point (elementSize, elementSize));
			Imgproc.dilate (hsv, hsv, element);
			/// Contours

			List<MatOfPoint> contours = new List<MatOfPoint> ();
			Mat hierarchy= new Mat();

			Imgproc.findContours (hsv, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE, new Point (0, 0));
			double largestContour = 0;
			int largestIdx = -1;

			for (int i = 0; i < contours.Count; i++) {
				MatOfPoint contour = contours [i];
				double area = Imgproc.contourArea (contour);

				if (area > largestContour) {
					largestContour = area;
					largestIdx = i;
				}
			}
			Imgproc.drawContours (cameraImageMat, contours, largestIdx, new Scalar (0, 0, 255), 1);


			// Polygon / hull
			if (contours.Count > 0) {
				var contourPoints = contours [largestIdx].toList ();

				MatOfInt indexes = new MatOfInt();

				Imgproc.convexHull (contours[largestIdx], indexes, false);

				List<MatOfPoint> hull = new List<MatOfPoint> ();

				var list = indexes.toList ();

				var points = new List<Point> ();
				for (int i = 0; i < list.Count; i++) {
					var index = list[i];
					points.Add (contourPoints [index]);

				}
				hull.Add ( new MatOfPoint(points.ToArray()) );

				Imgproc.drawContours(cameraImageMat, hull, 0, new Scalar (0, 255, 0), 3);

				if (points.ToArray ().Length > 2) {
					
				}
			}


			MatDisplay.DisplayMat (cameraImageMat, MatDisplaySettings.FULL_BACKGROUND);
			MatDisplay.DisplayMat (hsv, MatDisplaySettings.BOTTOM_LEFT);
		}
	}
}
