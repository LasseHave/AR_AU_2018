using UnityEngine;
using OpenCVForUnity;
using Vuforia;
using System.Collections;
using System.Collections.Generic;

public class Assignment3_e : MonoBehaviour {

	Mat cameraImageMat;

	[Range(0f, 180f)]
	public int minH = 95;
	[Range(0f, 180f)]
	public int maxH = 150;
	[Range(0f, 255f)]
	public int minS = 43;
	[Range(0f, 255f)]
	public int maxS = 108;
	[Range(0f, 255f)]
	public int minV = 119;
	[Range(0f, 255f)]
	public int maxV = 168;

	public int blurSize = 5;
	public int elementSize = 5;


	public bool isDrawing = false;

	private GameObject drawPlate;
	private Renderer rend;
	private Texture2D newTexture;

	// Use this for initialization
	void Start () {

		drawPlate = GameObject.Find ("DrawPlate");

		rend = drawPlate.GetComponent<Renderer>();
		newTexture = Instantiate(rend.material.mainTexture) as Texture2D;
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

			if (largestIdx != -1) {
				var largeContour = contours [largestIdx].toList();

				double minimumY = 10000;
				Point finger = null;


				for (var i = 0; i < largeContour.Count; i++) {
					var po = largeContour [i];
					if (po.y < minimumY) {
						minimumY = po.y;
						finger = po;
					}
				}


				rend.material.mainTexture = newTexture;

	
				Vector3 p = new Vector3();
				Camera  c = Camera.main;
				Event   e = Event.current;
				Vector2 mousePos = new Vector2();

				// Get the mouse position from Event.
				// Note that the y position from Event is inverted.
				mousePos.x = (float)finger.x;
				mousePos.y = (float)(c.pixelHeight - finger.y);

				p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 22) );

				GameObject.Find ("Finger").transform.position = p;

				Ray ray = c.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0));
				Debug.DrawRay(ray.origin, ray.direction * 22, Color.yellow);

				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					Renderer rend = hit.transform.GetComponentInParent<Renderer>();

					if (rend == null || rend.material == null || rend.material.mainTexture == null)
						return;

					Debug.Log ("HIT");

					Texture2D tex = rend.material.mainTexture as Texture2D;
					Vector2 pixelUV = hit.textureCoord;
					pixelUV.x *= tex.width;
					pixelUV.y *= tex.height;

					Debug.Log (pixelUV);

					tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.black);

					rend.material.mainTexture = tex;
				}

				/*
				Vector3 localPos = drawPlate.transform.InverseTransformPoint (p);
				Debug.Log (localPos);

				var height = 22;//rend.material.mainTexture.height;
				var width = 22;//rend.material.mainTexture.width;

				var texX = localPos.x / width * width;
				var texY = localPos.y / height * height;

				Debug.Log (texX + ":" + texY);

				newTexture.SetPixel ((int)texX, (int)texY, Color.black);

				// actually apply all SetPixels, don't recalculate mip levels
				newTexture.Apply(false);
				*/

			}

			/*
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

				//if (points.ToArray ().Length > 2) {
				//}
			}
			*/

			MatDisplay.DisplayMat (cameraImageMat, MatDisplaySettings.FULL_BACKGROUND);
			MatDisplay.DisplayMat (hsv, MatDisplaySettings.BOTTOM_LEFT);
		}
	}
}
