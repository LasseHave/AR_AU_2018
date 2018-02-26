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

			//convert color to gray
			Imgproc.cvtColor (cameraImageMat, grayScale, Imgproc.COLOR_RGB2GRAY);

			// Blur image
			Imgproc.GaussianBlur(grayScale, grayScale, new Size(5, 5), 0);

			//thresholding make the image black and white
			Imgproc.threshold(grayScale, threshold, 0, thresholdValue, Imgproc.THRESH_OTSU);


			// Find contours and draw
			List<MatOfPoint> contours = new List<MatOfPoint>();
			List<MatOfPoint> ls_mop = new List<MatOfPoint>();
			Mat hierarchy = new Mat();
			Imgproc.findContours(threshold, ls_mop, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);


			for (int i = 0; i < ls_mop.Count; i++) {
				Imgproc.drawContours (threshold, ls_mop, i, new Scalar (255, 0, 0), 2, 8, hierarchy, 0, new Point (0, 0));
			}


			List<MatOfPoint> tempTargets = new List<MatOfPoint>();
			for(int i=0; i<ls_mop.Count; i++)
			{
				MatOfPoint cp = ls_mop[i];
				MatOfPoint2f cn = new MatOfPoint2f(cp.toArray());
				double p = Imgproc.arcLength(cn, true);

				MatOfPoint2f approx = new MatOfPoint2f();
				//convert contour to readable polygon
				Imgproc.approxPolyDP(cn, approx, 0.03 * p, true);

				// find a contour with 4 points
				if (approx.toArray().Length == 4)
				{

					MatOfPoint approxPt = new MatOfPoint();
					approx.convertTo(approxPt, CvType.CV_32S);

					float maxCosine = 0;
					for (int j = 2; j < 5; j++)
					{
						Vector2 v1 = new Vector2((float)(approx.toArray()[j % 4].x - approx.toArray()[j - 1].x), (float)(approx.toArray()[j % 4].y - approx.toArray()[j - 1].y));
						Vector2 v2 = new Vector2((float)(approx.toArray()[j - 2].x - approx.toArray()[j - 1].x), (float)(approx.toArray()[j - 2].y - approx.toArray()[j - 1].y));

						float angle = Mathf.Abs(Vector2.Angle(v1, v2));
						maxCosine = Mathf.Max(maxCosine, angle);
					}

					if(maxCosine < 135f)
					{
						tempTargets.Add(approxPt);
					}

				}

			}

			for (int i = 0; i < tempTargets.Count; i++) {
				Point[] arr = tempTargets [i].toArray ();
				for (int z = 0; z < arr.Length; z++) {
					Imgproc.circle (threshold, arr[z], 15, new Scalar (255, 0, 255), -1);
				}
			}

			/*
			for (int i=0; i<ls_mop.Count; i++) {
				double returnVal = Imgproc.matchShapes (ls_mop [1], ls_mop [i], Imgproc.CV_CONTOURS_MATCH_I1, 0);
				Debug.Log ("returnVal " + i + " " + returnVal);

				Point point = new Point ();
				float[] radius = new float[1];
				Imgproc.minEnclosingCircle (new MatOfPoint2f (ls_mop [i].toArray ()), point, radius);
				Debug.Log ("point.ToString() " + point.ToString ());
				Debug.Log ("radius.ToString() " + radius [0]);

				Imgproc.circle (threshold, point, 5, new Scalar (0, 0, 255), -1);
				Imgproc.putText (threshold, " " + returnVal, point, Core.FONT_HERSHEY_SIMPLEX, 0.4, new Scalar (0, 255, 0), 1, Imgproc.LINE_AA, false);
			}
			*/

			// Approx polygons - From this part im not so sure whether it works.
			//MatOfPoint2f matOfPoint2f = new MatOfPoint2f();
			//MatOfPoint2f approxCurve = new MatOfPoint2f();
			//Imgproc.approxPolyDP(matOfPoint2f, approxCurve, Imgproc.arcLength(matOfPoint2f, true) * 0.02, true);


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
