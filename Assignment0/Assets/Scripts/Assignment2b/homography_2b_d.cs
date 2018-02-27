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
	public GameObject skull;
	public Texture2D originalTextureSkull;

	public GameObject imageTarget;

	Texture2D imgTexture;
	public static bool allCubiesScaned = false;
	public double thresholdValue = 160;

	private MatOfPoint2f imagePoints;
	private Mat skullMatPngOriginal;


	// Use this for initialization
	void Start () {
		skullMatPngOriginal = MatDisplay.LoadRGBATexture("flying_skull_tex.png");
		// originalTextureSkull = skull.GetComponent<Renderer> ().material.mainTexture;
		imagePoints = new MatOfPoint2f();
		imagePoints.alloc(4);
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

			MatOfPoint test2 = new MatOfPoint ();
			MatOfPoint2f matof2point = new MatOfPoint2f ();
			matof2point.alloc (4);

			Point[] largestShape = new OpenCVForUnity.Point[4];
			float largestArea = 0;

			for (int i = 0; i < tempTargets.Count; i++) {
				Point[] arr = tempTargets [i].toArray ();

				double area = this.PolygonArea (arr);
				largestArea = Mathf.Max ((float)area, (float)largestArea);

				if (largestArea == area) {
					largestShape = arr;
				}
			}

			if (largestArea == 0)
				return;
				
			for (int z = 0; z < largestShape.Length; z++) {
				imagePoints.put(z, 0, largestShape[z].x, largestShape[z].y);
				MatDisplay.PutPoint2f(matof2point, z, new Vector2((float)largestShape[z].x, (float)(largestShape[z].y)));

				Imgproc.circle (threshold, largestShape[z], 15, new Scalar (255, 0, 255), -1);
			}
				

			var skullPoints = new MatOfPoint2f (); // Creating a destination
			skullPoints.alloc (4); // Allocate memory
			skullPoints.put(0, 0, 1024, 0);
			skullPoints.put(1, 0, 1024, 1024);
			skullPoints.put(2, 0, 0, 1024);
			skullPoints.put(3, 0, 0, 0);

			var width = 640;
			var height = 480;
			var newPoints = new MatOfPoint2f (); // Creating a destination
			newPoints.alloc (4); // Allocate memory
			newPoints.put(1, 0, width, 0);
			newPoints.put(2, 0, width, height);
			newPoints.put(3, 0, 0, height);
			newPoints.put(0, 0, 0, 0);


			Mat skullTexture = new Mat ();// New mat as destination from warp
			Mat drawedTexture = new Mat ();// New mat as destination from warp

			var findHomography = Calib3d.findHomography (imagePoints, newPoints); // Finding the image
			var findHomography2 = Calib3d.findHomography (skullPoints, imagePoints); // Finding the image

			Imgproc.warpPerspective (cameraImageMat, drawedTexture, findHomography, new Size (cameraImageMat.width(), cameraImageMat.height()));
			Imgproc.warpPerspective (skullMatPngOriginal, skullTexture, findHomography2, new Size (cameraImageMat.width(), cameraImageMat.height()));

			Mat newMat = new Mat (); //"Prints" the skullTexture on videofeed
			Core.addWeighted(cameraImageMat, 0.95f, skullTexture, 0.4f, 0.0, newMat);

			Mat mergedTexture = new Mat (); // Merge the drawn texture with the skulltexture
			Core.addWeighted(skullTexture, 1f, drawedTexture, 1f, 0.0, mergedTexture);


			Texture2D unwarpedMergedTexture = new Texture2D (mergedTexture.cols(), mergedTexture.rows(), TextureFormat.RGBA32, false);
			MatDisplay.MatToTexture(mergedTexture, ref unwarpedMergedTexture); // Tag output og lav til texture...

			skull.GetComponent<Renderer> ().material.mainTexture = unwarpedMergedTexture; // Set textur på element


			MatDisplay.DisplayMat (drawedTexture, MatDisplaySettings.BOTTOM_LEFT);
			MatDisplay.DisplayMat (skullTexture, MatDisplaySettings.BOTTOM_RIGHT);

			MatDisplay.DisplayMat (newMat, MatDisplaySettings.FULL_BACKGROUND);
		}
			
	}

	private double PolygonArea(Point[] polygon)
	{
		int i,j;
		double area = 0; 

		for (i=0; i < polygon.Length; i++) {
			j = (i + 1) % polygon.Length;

			area += polygon[i].x * polygon[j].y;
			area -= polygon[i].y * polygon[j].x;
		}

		area /= 2;
		return (area < 0 ? -area : area);
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
