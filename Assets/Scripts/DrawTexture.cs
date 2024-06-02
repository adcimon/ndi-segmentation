using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class DrawTexture : MonoBehaviour
{
	private RawImage image;

	private void Awake()
	{
		image = this.GetComponent<RawImage>();
	}

	public void Draw(Texture texture)
	{
		image.texture = texture;
		image.SetMaterialDirty();
	}

	public void Draw(Texture2D texture)
	{
		image.texture = texture;
		image.SetMaterialDirty();
	}

	public void Draw(RenderTexture texture)
	{
		image.texture = texture;
		image.SetMaterialDirty();
	}
}