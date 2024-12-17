using OpenCvSharp;
using OpenCvSharp.Demo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ColorDetector : WebCamera
{
	private Mat srcImage;
	private Mat hsvImage = new();
	private Mat mask = new();
	private Mat colorResult = new();

	private Point[][] contours;
	private HierarchyIndex[] hierarchy;

	[SerializeField]
	private List<ColorInput> _colorList = new()
	{
		new ColorInput("red", 136, 180, 87, 255, 111, 255),
		new ColorInput("green", 25, 102, 52, 255, 72, 255),
		new ColorInput("blue", 94, 120, 80, 255, 2, 255),
	};

	private readonly List<Color> _colors = new();
	//{
	//	new Color("red", new Scalar(136, 87, 111), new Scalar(180, 255, 255)),
	//	new Color("green", new Scalar(25, 52, 72), new Scalar(102, 255, 255)),
	//	new Color("blue", new Scalar(94, 80, 2), new Scalar(120, 255, 255)),
	//};

	[SerializeField]
	private double _minArea = 1000;

	[SerializeField]
	private bool _colorDetected = false;
	//[SerializeField] private int _colorIndex = 0;

	[SerializeField]
	private Color _testColor;

	[Header("Events")]
	public UnityEvent<string> ColorDetected;

	protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
	{
		srcImage = OpenCvSharp.Unity.TextureToMat(input);

		Cv2.CvtColor(srcImage, hsvImage, ColorConversionCodes.BGR2HSV);

		//var color = _colors[_colorIndex];

		foreach (var color in _colors)
		{
			if (_colorDetected)
			{
				break;
			}

			//var colorDetectionTask = Task
			//	.Run(async () => await DetectColorAsync(color));

			//var result = colorDetectionTask.Result;

			//if (result)
			//{
			//	_colorDetected = true;
			//	Debug.Log($"Detected color: {color.name}");

			//	ColorDetected?.Invoke(color.name);
			//}

			if (DetectColorV2(color, ref output))
			{
				_colorDetected = true;
				//Debug.Log($"Detected color: {color.name}");

				ColorDetected?.Invoke(color.name);
			}
		}

		//OpenCvSharp.Unity.MatToTexture(srcImage, output);

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

	public bool DetectColorV2(Color color, ref Texture2D output)
	{
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

	public async Task<bool> DetectColorAsync(Color color)
	{
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

		await Task.Run(() => Cv2.FindContours(mask, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple));
		;

		//if (output == null)
		//{
		//	output = OpenCvSharp.Unity.MatToTexture(mask);
		//}
		//else
		//{
		//	OpenCvSharp.Unity.MatToTexture(mask, output);
		//}

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

	[Serializable]
	public class ColorInput
	{
		public string name;

		[Header("Hue")]
		public double hueMin;
		public double hueMax;

		[Header("Saturation")]
		public double saturationMin;
		public double saturationMax;

		[Header("Value")]
		public double valueMin;
		public double valueMax;

		public ColorInput(string name, double hueMin, double hueMax, double saturationMin, double saturationMax, double valueMin, double valueMax)
		{
			this.name = name;
			this.hueMin = hueMin;
			this.hueMax = hueMax;
			this.saturationMin = saturationMin;
			this.saturationMax = saturationMax;
			this.valueMin = valueMin;
			this.valueMax = valueMax;
		}
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

	public void OnPreviewClosed()
	{
		_colorDetected = false;

		Debug.Log("Color Detection Resumed");
	}

	public void PauseColorDetection()
	{
		_colorDetected = true;

		Debug.Log("Color Detection Paused");
	}

	public void OnEnable()
	{
		_colors.Clear();
		
		foreach (var colorInput in _colorList)
		{
			var color = new Color(colorInput.name,
				new Scalar(colorInput.hueMin, colorInput.saturationMin, colorInput.valueMin),
				new Scalar(colorInput.hueMax, colorInput.saturationMax, colorInput.valueMax));

			_colors.Add(color);

			Debug.Log($"Color {color.name} added");
		}
	}
}
