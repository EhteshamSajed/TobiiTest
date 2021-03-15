using System;
using UnityEngine;

[Serializable]
public abstract class PupilData
{
    public  float[] pupilDiameter;
    
    public  int stimuliId;
    public  long startTimeStamp;
    public  long durationInTicks;
    public PupilData(float[] _pupilDiameter, int _stimuliId, long _startTimeStamp, long _durationInTicks)
    {
        pupilDiameter = _pupilDiameter;
        stimuliId = _stimuliId;
        startTimeStamp = _startTimeStamp;
        durationInTicks = _durationInTicks;
    }    
    public override string ToString(){
        return JsonUtility.ToJson(this);
    }    
}
