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
			Imgproc.threshold(grayScale, threshold, 120, 0, Imgproc.THRESH_TOZERO);


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

			List<OpenCVForUnity.Rect> rects = new List<OpenCVForUnity.Rect>();
			try
			{
				// Each figure in the hierachy
				for (int idx = 0; idx >= 0; idx = (int)hierarchy.get(0, idx)[0])
				{
					MatOfPoint contour = contours[idx];
					OpenCVForUnity.Rect rect = Imgproc.boundingRect(contour);
					double contourArea = Imgproc.contourArea(contour);
					matOfPoint2f.fromList(contour.toList());

					Imgproc.approxPolyDP(matOfPoint2f, approxCurve, Imgproc.arcLength(matOfPoint2f, true) * 0.02, true);
					long total = approxCurve.total();

					if (total == 4)
					{
						ArrayList cos = new ArrayList();
						Point[] points = approxCurve.toArray();

						for (int j = 2; j < total + 1; j++)
						{
							cos.Add(angle(points[(int)(j % total)], points[j - 2], points[j - 1]));
						}

						cos.Sort();
						double minCos = (double)cos[0];
						double maxCos = (double)cos[cos.Count - 1];
						bool isRect = total == 4 && minCos >= -0.1 && maxCos <= 0.3;

						if (isRect)
							Debug.Log("I found a rect");
						
						{
							if (rect.width > 20) rects.Add(rect);

							List<double[]> Colors = new List<double[]>();
							for (int op = 0; op < 10; op++)
							{
								if (rects.Count == 9)
								{
									allCubiesScaned = true;
									Color[] blockOfColour = imgTexture.GetPixels(rect.x + rect.width / 2, rect.y + rect.height, rect.width / 3, rect.height / 3, 0);

									float r = 0, g = 0, b = 0;
									foreach (Color pixelBlock in blockOfColour)
									{
										r += pixelBlock.r;
										g += pixelBlock.g;
										b += pixelBlock.b;
									}
									r = r / blockOfColour.Length;
									g = g / blockOfColour.Length;
									b = b / blockOfColour.Length;

									Color rgb = new Color(r, g, b);

									Colors.Add(new double[] { rgb.r * 255, rgb.g * 255, rgb.b * 255 });
									print(Colors.Count);
								}
							}
							Imgproc.drawContours(threshold, contours, idx, new Scalar(255, 100, 155), 4);
						}
					}
				}
			}
			catch (System.Exception e)
			{
			}


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
