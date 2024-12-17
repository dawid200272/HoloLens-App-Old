using MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorDetectorPanel : MonoBehaviour
{

	public RawImage AnimationDisplay;
	public GameObject AnimationDisplayPanel;

	private const string AnimationName = "hero.jpg";
	private const string AnimationFolderPath = "Assets/Images";

	private Texture2D imageTexture = null;
	//private Sprite image = null;

	public UnityEvent PreviewClosed;

	public void ClearPanel()
	{
		Debug.Log("Clear animation display panel");

		PreviewClosed?.Invoke();
	}

	public void ShowImage()
	{
		if (AnimationDisplay == null)
		{
			Debug.LogError($"{nameof(AnimationDisplay)} cannot be null to show image");
			return;
		}

		var imagePath = $"{AnimationFolderPath}/{AnimationName}";

		imageTexture = LoadImageIntoTexture(imagePath);

		AnimationDisplay.texture = imageTexture;
	}

	public void ShowImage(string imagePath)
	{
		if (AnimationDisplay == null)
		{
			Debug.LogError($"{nameof(AnimationDisplay)} cannot be null to show image");
			return;
		}

		imageTexture = LoadImageIntoTexture(imagePath);

		AnimationDisplay.texture = imageTexture;
	}

	public void ClearAnimation()
	{
		AnimationDisplay.texture = null;
	}

	public void HidePanel()
	{
		if (AnimationDisplayPanel == null)
		{
			Debug.LogError($"{nameof(AnimationDisplayPanel)} cannot be null to hide animation panel");
			return;
		}

		AnimationDisplayPanel.SetChildrenActive(false);
		AnimationDisplayPanel.SetActive( false );
	}

	public void ShowPanel()
	{
		if (AnimationDisplayPanel == null)
		{
			Debug.LogError($"{nameof(AnimationDisplayPanel)} cannot be null to show animation panel");
			return;
		}

		AnimationDisplayPanel.SetChildrenActive(true);
		AnimationDisplayPanel.SetActive(true);
	}

	private Texture2D LoadImageIntoTexture(string imagePath)
	{
		//image = Resources.Load<Sprite>($"{AnimationFolderPath}/{AnimationName}");

		var imageBytes = GetImageBytes(imagePath);

		var tempTexture = new Texture2D(2, 2);
		tempTexture = new Texture2D(2, 2);

		tempTexture.LoadImage(imageBytes);

		return tempTexture;
	}

	private byte[] GetImageBytes(string path)
	{
		const int BufferSize = 2048;

		MemoryStream destination = new();

		FileInfo fileInfo = new FileInfo(path);

		using (Stream source = fileInfo.OpenRead())
		{
			byte[] buffer = new byte[BufferSize];
			int bytesRead;

			while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
			{
				destination.Write(buffer, 0, bytesRead);
			}
		}

		byte[] imageBytes = destination.ToArray();

		return imageBytes;
	}

	public void OnColorDetected(string colorName)
	{
		Debug.LogWarning($"Color {colorName} detected");

		const string PlaceHolderFolderName = "Placeholders";

		string PlaceHolderName = colorName + "_placeholder.jpg";

		var imagePath = $"{AnimationFolderPath}/{PlaceHolderFolderName}/{PlaceHolderName}";

		ShowImage(imagePath);
	}
}
