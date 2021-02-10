using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Research.Unity;
using Tobii.Research;

public class TobiiTest : MonoBehaviour
{
    IEyeTracker eyeTracker;
    // Start is called before the first frame update
    void Start()
    {
        if (EyeTrackingOperations.FindAllEyeTrackers().Count != 0)
            eyeTracker = EyeTrackingOperations.FindAllEyeTrackers()[0];
        else {
            eyeTracker = gameObject.GetComponent<DummyEyeTracker>();
        }
        if (eyeTracker != null)
            eyeTracker.GazeDataReceived += onGazeDataReceived;
        TestJSON();
    }

    void TestJSON(){
        float[] pupilDia = {10, 20, 30, 40};
        /*PupilData pupilData = new PupilData(pupilDia, pupilDia, 10, 230);
        Debug.Log (pupilData.StimuliId);
        string jsonString = JsonUtility.ToJson(pupilData);
        Debug.Log (jsonString);
        PupilData pupilData1 = JsonUtility.FromJson<PupilData>(jsonString);
        Debug.Log (pupilData1.StimuliId);*/

        /*Participant participant = new Participant(1001, "Sajed");
        string participantString = JsonUtility.ToJson(participant);
        Debug.Log(participantString);
        Participant participant1 = JsonUtility.FromJson<Participant>(participantString);*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onGazeDataReceived(object sender, GazeDataEventArgs e)
    {
        Debug.Log("Left pupil diameter: " + e.LeftEye.Pupil.PupilDiameter);
        Debug.Log("Right pupil diameter: " + e.RightEye.Pupil.PupilDiameter);
    }

    void OnApplicationQuit()
    {
        if (eyeTracker != null)
            eyeTracker.GazeDataReceived -= onGazeDataReceived;
    }
}
