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


	public bool drawButtonState = false;

	public bool foundFingerHSV = false;

	public GameObject circlePrefab;

	private GameObject drawPlate;
	private GameObject drawButton;
	//private Renderer rend;
	//private Texture2D newTexture;
	private int frameCount = 0;

	// Use this for initialization
	void Start () {
		drawPlate = GameObject.Find ("DrawPlate");
		drawButton = GameObject.Find ("drawButton");
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

			if (!foundFingerHSV && drawButtonState) {
				var virtualButtonViewCoords = Camera.main.WorldToViewportPoint (drawButton.transform.position);
				var pixel = cameraImageMat.get ((int)virtualButtonViewCoords.x, (int)virtualButtonViewCoords.y);
				Debug.Log (pixel[0] + ", " + pixel[1] + ", " + pixel[2]);
			}

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

			// Imgproc.drawContours (cameraImageMat, contours, largestIdx, new Scalar (0, 0, 255), 1);

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
	
				Vector3 p = new Vector3();
				Camera  c = Camera.main;
				Event   e = Event.current;
				Vector2 mousePos = new Vector2();

				// Get the mouse position from Event.
				// Note that the y position from Event is inverted.
				mousePos.x = (float)finger.x;
				mousePos.y = (float)(c.pixelHeight - finger.y);

				p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 22) );

				//var fingerObj = GameObject.Find ("Finger");
				//fingerObj.transform.position = p;

				Ray ray = c.ScreenPointToRay (mousePos);
				RaycastHit hit;
				Physics.Raycast(c.transform.position, ray.direction, out hit, 10000.0f);

				if (hit.collider) {
					//fingerObj.transform.position = hit.point;

					if (drawButtonState || frameCount < 30) {
						Instantiate (circlePrefab, hit.point, Quaternion.identity);
					}
				}


			}

			MatDisplay.DisplayMat (cameraImageMat, MatDisplaySettings.FULL_BACKGROUND);
			MatDisplay.DisplayMat (hsv, MatDisplaySettings.BOTTOM_LEFT);
		}

		if (drawButtonState) {
			frameCount = 0;
		} else {
			frameCount++;
		}
	}
}
