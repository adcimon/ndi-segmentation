using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WebcamSource : MonoBehaviour
{
    private enum State { Stopped, Initializing, Playing }
    private State state = State.Stopped;

    public bool playOnStart = true;

    [Header("Webcam")]
    public string deviceName = "";
    public int width = 640;
    public int height = 480;
    public int fps = 30;

    [Header("Render")]
    public bool screenRender = true;
    public bool verticalFlip = false;
    public Material material;
    public string textureName = "_MainTex";

    private WebCamTexture webcamTexture;
    public RenderTexture renderTexture { get; private set; }
    public UnityEvent<RenderTexture> onRenderTexture;

    private void Start()
    {
        if( playOnStart )
        {
            Play();
        }
    }

    private void Update()
    {
        if( webcamTexture && webcamTexture.isPlaying && webcamTexture.didUpdateThisFrame )
        {
            if( state == State.Initializing )
            {
                width = webcamTexture.width;
                height = webcamTexture.height;
                fps = (int)webcamTexture.requestedFPS;
                Debug.Log("Capturing " + webcamTexture.deviceName + " " + webcamTexture.width + "x" + webcamTexture.height + " at " + webcamTexture.requestedFPS + "fps");

                state = State.Playing;
            }

            if( screenRender )
            {
                if( !renderTexture || renderTexture.width != Screen.width || renderTexture.height != Screen.height )
                {
                    renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
                    onRenderTexture?.Invoke(renderTexture);
                }
            }
            else
            {
                if( !renderTexture )
                {
                    renderTexture = new RenderTexture(webcamTexture.width, webcamTexture.height, 0);
                    onRenderTexture?.Invoke(renderTexture);
                }
            }

            Blit(webcamTexture, renderTexture, verticalFlip);
        }
    }

    private void OnDestroy()
    {
        Stop();
        ReleaseTextures();
    }

    /// <summary>
    /// Play the webcam device.
    /// </summary>
    public bool Play()
    {
        if( state != State.Stopped )
        {
            return false;
        }

        try
        {
            deviceName = IsWebcam(deviceName) ? deviceName : WebCamTexture.devices[0].name;

            if( width <= 0 || height <= 0 || fps <= 0 )
            {
                webcamTexture = new WebCamTexture(deviceName);
            }
            else
            {
                webcamTexture = new WebCamTexture(deviceName, width, height, fps);
            }

            webcamTexture.Play();

            if( material )
            {
                material.SetTexture(textureName, webcamTexture);
            }

            state = State.Initializing;

            return true;
        }
        catch( Exception exception )
        {
            Debug.Log(exception.Message);
            return false;
        }
    }

    /// <summary>
    /// Stop the webcam device.
    /// </summary>
    public bool Stop()
    {
        if( state == State.Stopped )
        {
            return false;
        }

        ReleaseTextures();

        state = State.Stopped;

        return true;
    }

    /// <summary>
    /// Blit a source texture into the destination render texture with aspect ratio compensation.
    /// </summary>
    private void Blit( Texture source, RenderTexture destination, bool verticalFlip = false )
    {
        if( source == null || destination == null )
        {
            return;
        }

        float aspect1 = (float)source.width / source.height;
        float aspect2 = (float)destination.width / destination.height;

        Vector2 scale = new Vector2(aspect2 / aspect1, aspect1 / aspect2);
        scale = Vector2.Min(Vector2.one, scale);
        if( verticalFlip ) scale.y *= -1;

        Vector2 offset = (Vector2.one - scale) / 2;

        RenderTexture.active = destination;
        Graphics.Blit(source, destination, scale, offset);
    }

    /// <summary>
    /// Release the texture resources.
    /// </summary>
    private void ReleaseTextures()
    {
        if( webcamTexture )
        {
            webcamTexture.Stop();
            webcamTexture = null;
        }

        if( renderTexture )
        {
            renderTexture.Release();
            renderTexture = null;
        }
    }

    /// <summary>
    /// Get the list of webcam devices' names.
    /// </summary>
    public List<string> GetWebcams()
    {
        return WebCamTexture.devices.Select(d => d.name).ToList();
    }

    /// <summary>
    /// Is the device a webcam?
    /// </summary>
    private bool IsWebcam( string deviceName )
    {
        for( int i = 0; i < WebCamTexture.devices.Length; i++ )
        {
            WebCamDevice device = WebCamTexture.devices[i];
            if( device.name.Equals(deviceName) )
            {
                return true;
            }
        }

        return false;
    }
}