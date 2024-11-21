using OpenCvSharp;
using OpenCvSharp.Demo;
using System.Collections.Generic;
using UnityEngine;

public class ColorDetector : WebCamera
{
	private Mat srcImage;
	private Mat hsvImage = new();
	private Mat mask = new();
	private Mat colorResult = new();

	private Point[][] contours;
	private HierarchyIndex[] hierarchy;

	private readonly List<Color> _colors = new()
	{
		new Color("red", new Scalar(136, 87, 111), new Scalar(180, 255, 255)),
		new Color("green", new Scalar(25, 52, 72), new Scalar(102, 255, 255)),
		new Color("blue", new Scalar(94, 80, 2), new Scalar(120, 255, 255)),
	};

	[SerializeField]
	private double _minArea = 1000;

	[SerializeField] private int _colorIndex = 0;

	protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
	{
		srcImage = OpenCvSharp.Unity.TextureToMat(input);

		var color = _colors[_colorIndex];

		if (DetectColor(srcImage, color, ref output))
		{
			Debug.Log($"Detected color: {color.name}");
		}

		return true;
	}

	public bool DetectColor(Mat srcImage, Color color, ref Texture2D output)
	{
		Cv2.CvtColor(srcImage, hsvImage, ColorConversionCodes.BGR2HSV);

		Scalar lowerLimit = color.lowerLimit;
		Scalar upperLimit = color.upperLimit;

		Cv2.InRange(hsvImage,
			new Scalar(lowerLimit.Val0, lowerLimit.Val1, lowerLimit.Val2),
			new Scalar(upperLimit.Val0, upperLimit.Val1, upperLimit.Val2),
			mask);

		colorResult = new Mat();

		Cv2.BitwiseAnd(srcImage, srcImage, colorResult, mask);

		const float threshold = 94.4f;
		Cv2.Threshold(mask, mask, threshold, 255, ThresholdTypes.Binary);

		Cv2.FindContours(mask, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

		if (output == null)
		{
			output = OpenCvSharp.Unity.MatToTexture(mask);
		}
		else
		{
			OpenCvSharp.Unity.MatToTexture(mask, output);
		}

		foreach (var contour in contours)
		{
			var area = Cv2.ContourArea(contour);

			if (area >= _minArea)
			{
				return true;
			}
		}

		return false;
	}

	public class Color
	{
		public string name;
		public Scalar lowerLimit;
		public Scalar upperLimit;

		public Color(string name, Scalar lowerLimit, Scalar upperLimit)
		{
			this.name = name;
			this.lowerLimit = lowerLimit;
			this.upperLimit = upperLimit;
		}
	}
}
