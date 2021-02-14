using System;
[Serializable]
public class PupilDataBaseline : PupilData
{
    float standardDeviation;
    float mean;
    public PupilDataBaseline(
        float[] _pupilDiameter,
        int _stimuliId,
        long _startTimeStamp,
        long _durationInTicks) : base(_pupilDiameter,
                                   _stimuliId,
                                   _startTimeStamp,
                                   _durationInTicks)
    {
        calculateBaseline();
    }

    public float StandardDeviation {
        get {
            if (standardDeviation.Equals(null))
                calculateBaseline();
            return standardDeviation;
        }
    }
    public float Mean { 
        get {
            if (mean.Equals(null))
                calculateBaseline();
            return mean;
        }
     }

    private void calculateBaseline()    //  need to calculate with std. daviation
    {
        if (pupilDiameter.Length == 0) return;
        
        float sum = 0;
        Array.ForEach(pupilDiameter, x =>
        {
            sum += x;
        });
        mean = sum / pupilDiameter.Length;
        sum = 0;
        Array.ForEach(pupilDiameter, x =>
        {
            sum += (x - Mean) * (x - Mean);
        });
        standardDeviation = (float)Math.Sqrt(sum / pupilDiameter.Length);        
    }
}
