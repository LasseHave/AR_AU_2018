using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using Vuforia;

public class homography_2b_c : MonoBehaviour {

	public Camera cam;

	public GameObject corner1;
	public GameObject corner2;
	public GameObject corner3;
	public GameObject corner4;
	public GameObject skull;

	public float fx;
	public float fy;
	public float cx;
	public float cy;

	private MatOfPoint2f imagePoints;
	private Mat camImageMat;
	private byte[] texData;

	void Start () {
		imagePoints = new MatOfPoint2f();
		imagePoints.alloc(4);
	}

	void Update () {
		//Access camera image provided by Vuforia
		Image camImg = CameraDevice.Instance.GetCameraImage(Image.PIXEL_FORMAT.RGBA8888);

		if(camImg!=null)
		{
			if (camImageMat == null)
			{
				//First time -> instantiate camera image specific data
				camImageMat = new Mat(camImg.Height, camImg.Width, CvType.CV_8UC4);  //Note: rows=height, cols=width
			}

			camImageMat.put(0,0, camImg.Pixels);

			// Der mangler lidt ændringer her!!!
			//Replace with your own projection matrix. This approach only uses fy.
			cam.fieldOfView = 2 * Mathf.Atan(camImg.Height * 0.5f / fy) * Mathf.Rad2Deg;

			Vector3 worldPnt1 = corner1.transform.position;
			Vector3 worldPnt2 = corner2.transform.position;
			Vector3 worldPnt3 = corner3.transform.position;
			Vector3 worldPnt4 = corner4.transform.position;

			//See lecture slides
			Matrix4x4 Rt = cam.transform.worldToLocalMatrix;
			Matrix4x4 A = Matrix4x4.identity;
			A.m00 = fx;
			A.m11 = fy;
			A.m02 = cx;
			A.m12 = cy;

			Matrix4x4 worldToImage = A * Rt;

			Vector3 hUV1 = worldToImage.MultiplyPoint3x4(worldPnt1);
			Vector3 hUV2 = worldToImage.MultiplyPoint3x4(worldPnt2);
			Vector3 hUV3 = worldToImage.MultiplyPoint3x4(worldPnt3);
			Vector3 hUV4 = worldToImage.MultiplyPoint3x4(worldPnt4);

			//hUV are the image coordinates in 2D homogeneous coordinates, we need to normalize, i.e., divide by Z
			Vector2 uv1 = new Vector2(hUV1.x, hUV1.y) / hUV1.z;
			Vector2 uv2 = new Vector2(hUV2.x, hUV2.y) / hUV2.z;
			Vector2 uv3 = new Vector2(hUV3.x, hUV3.y) / hUV3.z;
			Vector2 uv4 = new Vector2(hUV4.x, hUV4.y) / hUV4.z;

			//don't forget to alloc before putting values into a MatOfPoint2f
			imagePoints.put(0, 0, uv1.x, camImg.Height - uv1.y);
			imagePoints.put(1, 0, uv2.x, camImg.Height - uv2.y);
			imagePoints.put(2, 0, uv3.x, camImg.Height - uv3.y);
			imagePoints.put(3, 0, uv4.x, camImg.Height - uv4.y);

			//Debug draw points
			Point imgPnt1 = new Point(imagePoints.get(0, 0));
			Point imgPnt2 = new Point(imagePoints.get(1, 0));
			Point imgPnt3 = new Point(imagePoints.get(2, 0));
			Point imgPnt4 = new Point(imagePoints.get(3, 0));
			Imgproc.circle(camImageMat, imgPnt1, 5, new Scalar(255, 0, 0, 255));
			Imgproc.circle(camImageMat, imgPnt2, 5, new Scalar(0, 255, 0, 255));
			Imgproc.circle(camImageMat, imgPnt3, 5, new Scalar(0, 0, 255, 255));
			Imgproc.circle(camImageMat, imgPnt4, 5, new Scalar(255, 255, 0, 255));
			Scalar lineCl = new Scalar(200, 120, 0, 160);
			Imgproc.line(camImageMat, imgPnt1, imgPnt2, lineCl);
			Imgproc.line(camImageMat, imgPnt2, imgPnt3, lineCl);
			Imgproc.line(camImageMat, imgPnt3, imgPnt4, lineCl);
			Imgproc.line(camImageMat, imgPnt4, imgPnt1, lineCl);

			// FROM HERE
			var newPoints = new MatOfPoint2f (); // Creating a destination
			newPoints.alloc (4); // Allocate memory
			var findHomography = Calib3d.findHomography (imagePoints, newPoints); // Finding the image
			Mat outputMat = new Mat(); // New mat as destination from warp
			Imgproc.warpPerspective (camImageMat, outputMat, findHomography, new Size (outputMat.width(),outputMat.height()));

			Texture2D unwarpedTexture = new Texture2D (outputMat.cols(), outputMat.rows(), TextureFormat.RGBA32, false); // Her går det nok galt

			MatDisplay.MatToTexture (outputMat, ref unwarpedTexture); // Tag output og lav til texture...
			skull.GetComponent<Renderer> ().material.mainTexture = unwarpedTexture; // Set textur på element

			MatDisplay.DisplayMat(camImageMat, MatDisplaySettings.FULL_BACKGROUND);
		}
	}
}
