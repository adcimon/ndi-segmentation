using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public WebcamSource webcamSource;
    public Compositor compositor;
    public Dropdown webcamDropdown;
    public Dropdown modeDropdown;

    private void Start()
    {
        InitializeWebcamDropdown();
        InitializeModeDropdown();
    }

    private void InitializeWebcamDropdown()
    {
        webcamDropdown.options.Clear();

        List<string> webcams = WebCamTexture.devices.Select(d => d.name).ToList();
        for( int i = 0; i < webcams.Count; i++ )
        {
            string webcam = webcams[i];
            webcamDropdown.options.Add(new Dropdown.OptionData() { text = webcam });
        }

        webcamDropdown.onValueChanged.AddListener(delegate { OnWebcamItemSelected(webcamDropdown); });
    }

    private void OnWebcamItemSelected( Dropdown dropdown )
    {
        int index = dropdown.value;
        string webcam = dropdown.options[index].text;

        webcamSource.Stop();
        webcamSource.deviceName = webcam;
        webcamSource.Play();
    }

    private void InitializeModeDropdown()
    {
        modeDropdown.options.Clear();

        string[] modes = Enum.GetNames(typeof(Compositor.OutputMode));
        for( int i = 0; i < modes.Length; i++ )
        {
            string mode = modes[i];
            modeDropdown.options.Add(new Dropdown.OptionData() { text = mode });
        }

        modeDropdown.value = (int)compositor.outputMode;

        modeDropdown.onValueChanged.AddListener(delegate { OnModeItemSelected(modeDropdown); });
    }

    private void OnModeItemSelected( Dropdown dropdown )
    {
        int index = dropdown.value;
        compositor.outputMode = (Compositor.OutputMode)index;
    }
}