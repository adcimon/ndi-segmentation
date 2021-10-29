using UnityEngine;
using UnityEngine.Events;
using MediaPipe.Selfie;

public class Compositor : MonoBehaviour
{
    public enum OutputMode { Source, Mask, Color, Image }

    public OutputMode outputMode = OutputMode.Image;
    public Color backgroundColor = Color.green;
    public Texture2D backgroundImage;
    public ResourceSet resourceSet;
    public Material material;

    private SegmentationFilter filter;
    private Texture sourceTexture;
    public RenderTexture compositedTexture { get; private set; }
    public UnityEvent<RenderTexture> onRenderTexture;

    private void Start()
    {
        filter = new SegmentationFilter(resourceSet);
    }

    private void Update()
    {
        if( !sourceTexture || !material )
        {
            return;
        }

        if( !compositedTexture )
        {
            compositedTexture = new RenderTexture(sourceTexture.width, sourceTexture.height, 0);
            onRenderTexture?.Invoke(compositedTexture);
        }

        filter.ProcessImage(sourceTexture);

        material.SetTexture("_SourceTexture", sourceTexture);
        material.SetTexture("_MaskTexture", filter.MaskTexture);
        material.SetColor("_BackgroundColor", backgroundColor);
        material.SetTexture("_BackgroundTexture", backgroundImage);

        Graphics.Blit(null, compositedTexture, material, (int)outputMode);
    }

    private void OnDestroy()
    {
        if( filter != null )
        {
            filter.Dispose();
        }

        if( compositedTexture )
        {
            compositedTexture.Release();
            compositedTexture = null;
        }
    }

    public void SetSourceTexture( RenderTexture renderTexture )
    {
        sourceTexture = renderTexture;

        if( compositedTexture )
        {
            compositedTexture.Release();
            compositedTexture = null;
        }
    }
}