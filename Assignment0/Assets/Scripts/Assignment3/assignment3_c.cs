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
    private Mat faceWithSquare;
    private Mat eyeCameraMat;
    private Mat eyeWithCircles;
    private Mat warpedMat;
    private Mat faceWithCirclesMat;
	private bool faceDetected = false;
	private string faceXML;
    private string eyeXML;
    private OpenCVForUnity.Rect roi;
    Texture2D unwarpedTexture;


	void Start () {
		faceXML = "Assets/OpenCVForUnity/StreamingAssets/haarcascade_frontalface_alt.xml";
        eyeXML = "Assets/haarcascade_eye.xml";

        faceCameraMat = new Mat ();
		faceWithSquare = new Mat ();

        eyeCameraMat = new Mat();
        eyeWithCircles = new Mat();

        warpedMat = new Mat();
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

			Face.getFacesHAAR (faceCameraMat, faceWithSquare, faceXML);
			// Face.drawFacemarks (faceCameraMat, faceWithCirclesMat);
			Debug.Log(faceWithSquare.height ());
            //if(faceWithSquare.height() > 0)
            //{
            //    warpedMat = new Mat()
            //}

            for (var i = 0; i < faceWithSquare.height (); i++) {
                double[] rec = faceWithSquare.get (i, 0);
				Imgproc.rectangle (faceCameraMat, new Point (rec [0], rec [1]), new Point (rec[0]+rec [2], rec [1]+rec [3]), new Scalar(0, 0, 255), 5);
                roi = new OpenCVForUnity.Rect(new Point(rec[0], rec[1]), new Point(rec[0] + rec[2], rec[1] + rec[3]));
            }
            //faceCameraMat.adjustROI((int)roi.yMin, (int) roi.yMax, (int) roi.xMin, (int) roi.xMax);
            faceCameraMat = new Mat(faceCameraMat, roi);
            //faceCameraMat = new Mat(faceCameraMat,roi);

            Face.getFacesHAAR(faceCameraMat, eyeWithCircles, eyeXML);
            // Face.drawFacemarks (faceCameraMat, faceWithCirclesMat);
            Debug.Log(eyeWithCircles.height());
            if (eyeWithCircles.height() != 0)
            {
                for (var i = 0; i < 2; i++)
                {
                    double[] rec = eyeWithCircles.get(i, 0);
                    Debug.Log(rec[0]);
                    Point eye_centers = new Point(rec[2] * 0.5F + rec[0], rec[3] * 0.5F + rec[1]);
                    int radius = (int) Mathf.Sqrt(Mathf.Pow(((float)rec[2])*0.5F, 2F) + Mathf.Pow(((float)rec[3]) * 0.5F, 2F));

                    Imgproc.circle(faceCameraMat, new Point(eye_centers.x, eye_centers.y), radius, new Scalar(255, 0, 0), 5);
                    // Imgproc.circle(camImageMat, new Point(rec[2]-rec[0], rec[3]-rec[1]), 5, new Scalar(255, 0, 0), 5);
                    //Imgproc.rectangle(camImageMat, new Point(rec[0], rec[1]), new Point(rec[0] + rec[2], rec[1] + rec[3]), new Scalar(0, 0, 255), 5);
                }
            }




            MatDisplay.MatToTexture(faceCameraMat, ref unwarpedTexture); // Tag output og lav til texture... 
            faceImageTarget.GetComponent<Renderer>().material.mainTexture = unwarpedTexture; 


            MatDisplay.DisplayMat(camImageMat, MatDisplaySettings.FULL_BACKGROUND);

		}


	}
}
