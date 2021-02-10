using UnityEngine;
using Tobii.Research.Unity;
using Tobii.Research;
using System;
using System.ComponentModel;
using System.Collections;

public class DummyEyeTracker : MonoBehaviour, IEyeTracker
{
    float minPupilSize = 0.15f;
    float maxPupilSize = 0.50f;
    public Uri Address => throw new NotImplementedException();

    public string DeviceName => throw new NotImplementedException();

    public string SerialNumber => throw new NotImplementedException();

    public string Model => throw new NotImplementedException();

    public string FirmwareVersion => throw new NotImplementedException();

    public string RuntimeVersion => throw new NotImplementedException();

    public Capabilities DeviceCapabilities => throw new NotImplementedException();

    public event EventHandler<GazeDataEventArgs> GazeDataReceived;
    public event EventHandler<UserPositionGuideEventArgs> UserPositionGuideReceived;
    public event EventHandler<HMDGazeDataEventArgs> HMDGazeDataReceived;
    public event EventHandler<TimeSynchronizationReferenceEventArgs> TimeSynchronizationReferenceReceived;
    public event EventHandler<ExternalSignalValueEventArgs> ExternalSignalReceived;
    public event EventHandler<EventErrorEventArgs> EventErrorOccurred;
    public event EventHandler<EyeImageEventArgs> EyeImageReceived;
    public event EventHandler<EyeImageRawEventArgs> EyeImageRawReceived;
    public event EventHandler<GazeOutputFrequencyEventArgs> GazeOutputFrequencyChanged;
    public event EventHandler<CalibrationModeEnteredEventArgs> CalibrationModeEntered;
    public event EventHandler<CalibrationModeLeftEventArgs> CalibrationModeLeft;
    public event EventHandler<CalibrationChangedEventArgs> CalibrationChanged;
    public event EventHandler<DisplayAreaEventArgs> DisplayAreaChanged;
    public event EventHandler<ConnectionLostEventArgs> ConnectionLost;
    public event EventHandler<ConnectionRestoredEventArgs> ConnectionRestored;
    public event EventHandler<TrackBoxEventArgs> TrackBoxChanged;
    public event EventHandler<EyeTrackingModeChangedEventArgs> EyeTrackingModeChanged;
    public event EventHandler<DeviceFaultsEventArgs> DeviceFaults;
    public event EventHandler<DeviceWarningsEventArgs> DeviceWarnings;
    public event PropertyChangedEventHandler PropertyChanged;

    public void ApplyCalibrationData(CalibrationData calibrationData)
    {
        throw new NotImplementedException();
    }

    public void ClearAppliedLicenses()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public EyeTrackingModeCollection GetAllEyeTrackingModes()
    {
        throw new NotImplementedException();
    }

    public GazeOutputFrequencyCollection GetAllGazeOutputFrequencies()
    {
        throw new NotImplementedException();
    }

    public DisplayArea GetDisplayArea()
    {
        throw new NotImplementedException();
    }

    public string GetEyeTrackingMode()
    {
        throw new NotImplementedException();
    }

    public float GetGazeOutputFrequency()
    {
        throw new NotImplementedException();
    }

    public HMDLensConfiguration GetHMDLensConfiguration()
    {
        throw new NotImplementedException();
    }

    public TrackBox GetTrackBox()
    {
        throw new NotImplementedException();
    }

    public CalibrationData RetrieveCalibrationData()
    {
        throw new NotImplementedException();
    }

    public void SetDeviceName(string deviceName)
    {
        throw new NotImplementedException();
    }

    public void SetDisplayArea(DisplayArea displayArea)
    {
        throw new NotImplementedException();
    }

    public void SetEyeTrackingMode(string eyeTrackingMode)
    {
        throw new NotImplementedException();
    }

    public void SetGazeOutputFrequency(float gazeOutputFrequency)
    {
        throw new NotImplementedException();
    }

    public void SetHMDLensConfiguration(HMDLensConfiguration hmdLensConfiguration)
    {
        throw new NotImplementedException();
    }

    public bool TryApplyLicenses(LicenseCollection licenses, out FailedLicenseCollection failedLicenses)
    {
        throw new NotImplementedException();
    }

    protected virtual void OnGazeDataReceived(GazeDataEventArgs gazeDataEventArgs)
    {
        GazeDataReceived?.Invoke(this, gazeDataEventArgs);
    }

    void Start()
    {
        Debug.Log("DummyEyeTracker");
        StartCoroutine(GenerateDummyPupilSize());
    }

    IEnumerator GenerateDummyPupilSize()
    {
        while (true)
        {
            if (GazeDataReceived != null)
            {
                System.Random random = new System.Random();
                float pupilSize = (float)(random.NextDouble() * (maxPupilSize - minPupilSize) + minPupilSize);
                Tobii.Research.PupilData pupilData = new DummyPupilData(pupilSize, Validity.Valid);
                Tobii.Research.EyeData eyeData = new DummyEyeData(null, pupilData, null);
                GazeDataEventArgs gazeDataEventArgs = new GazeDataEventArgs(eyeData, eyeData, System.DateTime.Now.Ticks, System.DateTime.Now.Ticks);
                OnGazeDataReceived(gazeDataEventArgs);
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}