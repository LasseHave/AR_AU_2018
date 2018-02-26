using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using Vuforia;

public class homography_2b_b : MonoBehaviour {

	public Camera cam;

	public GameObject corner1;
	public GameObject corner2;
	public GameObject corner3;
	public GameObject corner4;
	public GameObject skull;
	public GameObject imageTarget;
	public ImageTargetBehaviour image_controller;


	public float fx = 833.88811F;
	public float fy = 834.98600F;
	public float cx = 319.52686F;
	public float cy = 238.81514F;
	public float nearField = 0.05F;
	public float farField = 2F;
	public Matrix4x4 originalProjection;

	public float width = 640F, height = 480F;

	private MatOfPoint2f imagePoints;
    private Mat calc_A;
    private Mat calc_b;
    private Mat calc_Htemp;
    private Mat calc_H;

    private Mat camImageMat;
	private byte[] texData;

	void Start () {
		imageTarget = GameObject.Find ("ImageTarget");

		image_controller = imageTarget.GetComponent<ImageTargetBehaviour>();

		imagePoints = new MatOfPoint2f();
		imagePoints.alloc(4);

        calc_A = new Mat(8, 8,CvType.CV_64FC1);
        calc_b = new Mat(8, 1, CvType.CV_64FC1);
        calc_Htemp = new Mat(8, 1, CvType.CV_64FC1);
        calc_H = new Mat(3, 3, CvType.CV_64FC1);
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
			newPoints.put(0, 0, 640, 0);
			newPoints.put(1, 0, 640, 480);
			newPoints.put(2, 0, 0, 480);
			newPoints.put(3, 0, 0, 0);


            // Extract data for homography

            var x0_prime = newPoints.get(0, 0)[0];
            var y0_prime = newPoints.get(0, 0)[1];
            var x1_prime = newPoints.get(1, 0)[0];
            var y1_prime = newPoints.get(1, 0)[1];
            var x2_prime = newPoints.get(2, 0)[0];
            var y2_prime = newPoints.get(2, 0)[1];
            var x3_prime = newPoints.get(3, 0)[0];
            var y3_prime = newPoints.get(3, 0)[1];

            var x0_src = imagePoints.get(0, 0)[0];
            var y0_src = imagePoints.get(0, 0)[1];
            var x1_src = imagePoints.get(1, 0)[0];
            var y1_src = imagePoints.get(1, 0)[1];
            var x2_src = imagePoints.get(2, 0)[0];
            var y2_src = imagePoints.get(2, 0)[1];
            var x3_src = imagePoints.get(3, 0)[0];
            var y3_src = imagePoints.get(3, 0)[1];

            // Initialize Matrix A, 
            // First point
            calc_A.put(0, 0, x0_src);
            calc_A.put(0, 1, y0_src);
            calc_A.put(0, 2, 1);
            calc_A.put(0, 6, -x0_prime*x0_src);
            calc_A.put(0, 7, -x0_prime*y0_src);

            calc_A.put(1, 3, x0_src);
            calc_A.put(1, 4, y0_src);
            calc_A.put(1, 5, 1);
            calc_A.put(1, 6, -y0_prime * x0_src);
            calc_A.put(1, 7, -y0_prime * y0_src);

            // Second point
            calc_A.put(2, 0, x1_src);
            calc_A.put(2, 1, y1_src);
            calc_A.put(2, 2, 1);
            calc_A.put(2, 6, -x1_prime * x1_src);
            calc_A.put(2, 7, -x1_prime * y1_src);

            calc_A.put(3, 3, x1_src);
            calc_A.put(3, 4, y1_src);
            calc_A.put(3, 5, 1);
            calc_A.put(3, 6, -y1_prime * x1_src);
            calc_A.put(3, 7, -y1_prime * y1_src);

            // Third point
            calc_A.put(4, 0, x2_src);
            calc_A.put(4, 1, y2_src);
            calc_A.put(4, 2, 1);
            calc_A.put(4, 6, -x2_prime * x2_src);
            calc_A.put(4, 7, -x2_prime * y2_src);

            calc_A.put(5, 3, x2_src);
            calc_A.put(5, 4, y2_src);
            calc_A.put(5, 5, 1);
            calc_A.put(5, 6, -y2_prime * x2_src);
            calc_A.put(5, 7, -y2_prime * y2_src);

            // Forth point
            calc_A.put(6, 0, x3_src);
            calc_A.put(6, 1, y3_src);
            calc_A.put(6, 2, 1);
            calc_A.put(6, 6, -x3_prime * x3_src);
            calc_A.put(6, 7, -x3_prime * y3_src);

            calc_A.put(7, 3, x3_src);
            calc_A.put(7, 4, y3_src);
            calc_A.put(7, 5, 1);
            calc_A.put(7, 6, -y3_prime * x3_src);
            calc_A.put(7, 7, -y3_prime * y3_src);
            
            // Initialize the b vector 
            calc_b.put(0, 0, x0_prime);
            calc_b.put(1, 0, y0_prime);
            calc_b.put(2, 0, x1_prime);
            calc_b.put(3, 0, y1_prime);
            calc_b.put(4, 0, x2_prime);
            calc_b.put(5, 0, y2_prime);
            calc_b.put(6, 0, x3_prime);
            calc_b.put(7, 0, y3_prime);
            
            // Solve the Ax=b 
            Core.solve(calc_A, calc_b, calc_Htemp);

            // Reallocate values to a 3x3 matrix
            calc_H.put(0, 0, calc_Htemp.get(0, 0));
            calc_H.put(0, 1, calc_Htemp.get(1, 0));
            calc_H.put(0, 2, calc_Htemp.get(2, 0));
            calc_H.put(1, 0, calc_Htemp.get(3, 0));
            calc_H.put(1, 1, calc_Htemp.get(4, 0));
            calc_H.put(1, 2, calc_Htemp.get(5, 0));
            calc_H.put(2, 0, calc_Htemp.get(6, 0));
            calc_H.put(2, 1, calc_Htemp.get(7, 0));
            calc_H.put(2, 2, 1); // Normalize

            Mat destPoints = new Mat ();// New mat as destination from warp

            //var findHomography = Calib3d.findHomography (imagePoints, newPoints); // Finding the image

            Imgproc.warpPerspective (camImageMat, destPoints, calc_H, new Size (camImageMat.width(), camImageMat.height()));

			Texture2D unwarpedTexture = new Texture2D (destPoints.cols(), destPoints.rows(), TextureFormat.RGBA32, false); // Her går det nok galt

			MatDisplay.MatToTexture (destPoints, ref unwarpedTexture); // Tag output og lav til texture...
			skull.GetComponent<Renderer> ().material.mainTexture = unwarpedTexture; // Set textur på element


			MatDisplay.DisplayMat(destPoints, MatDisplaySettings.BOTTOM_LEFT);

			MatDisplay.DisplayMat(camImageMat, MatDisplaySettings.FULL_BACKGROUND);
		}
	}
}
