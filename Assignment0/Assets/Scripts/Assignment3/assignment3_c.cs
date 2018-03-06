using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using Vuforia;

public class assignment3_c : MonoBehaviour {
	public VideoCapture videoCap;

	public Camera cam;
	public GameObject faceImageTarget;
	public ImageTargetBehaviour image_controller;

	private Mat camImageMat;
	private Mat faceCameraMat;
	private Mat faceWithCirclesMat;
	private bool faceDetected = false;

	Texture2D unwarpedTexture;


	void Start () {
		faceCameraMat = new Mat ();
		faceWithCirclesMat = new Mat ();
		// Start webcam on init
		videoCap = new VideoCapture ();
		videoCap.open (0);
	}

	// Update is called once per frame
	void Update () {
		Image camImg = CameraDevice.Instance.GetCameraImage(Image.PIXEL_FORMAT.RGBA8888);

		if (camImg != null) {
			if (camImageMat == null) {
				//First time -> instantiate camera image specific data
				camImageMat = new Mat (camImg.Height, camImg.Width, CvType.CV_8UC4);  //Note: rows=height, cols=width
			}
			camImageMat.put(0, 0, camImg.Pixels);

			// Read from videoCap and save in mat
			videoCap.read (faceCameraMat);

			Face.getFacesHAAR (faceCameraMat, faceWithCirclesMat, "Assets/OpenCVForUnity/StreamingAssets/haarcascade_frontalface_alt.xml");
			// We just get a random Mat in return, that not really is circles.
			// Face.drawFacemarks (faceCameraMat, faceWithCirclesMat);
			Debug.Log(faceWithCirclesMat.height ());
			for (var i = 0; i < faceWithCirclesMat.height (); i++) {
				double[] rec = faceWithCirclesMat.get (i, 0);
				Imgproc.rectangle (faceCameraMat, new Point (rec [0], rec [1]), new Point (rec [2], rec [3]), new Scalar(0, 0, 255), 5);
			}
				

			MatDisplay.MatToTexture(faceCameraMat, ref unwarpedTexture); // Tag output og lav til texture... 
			faceImageTarget.GetComponent<Renderer>().material.mainTexture = unwarpedTexture; 


			MatDisplay.DisplayMat(camImageMat, MatDisplaySettings.FULL_BACKGROUND);

		}


	}
}
