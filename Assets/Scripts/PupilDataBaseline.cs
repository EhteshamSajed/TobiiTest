using System;
[Serializable]
public class PupilDataBaseline : PupilData
{
    float baseLine;
    public PupilDataBaseline(
        float[] _pupilDiameter,
        int _stimuliId,
        long _startTimeStamp,
        long _durationInTicks) : base(_pupilDiameter,
                                   _stimuliId,
                                   _startTimeStamp,
                                   _durationInTicks)
    {
    }

    private float calculateBaseline()    //  need to calculate with std. daviation
    {
        if (pupilDiameter.Length == 0) return 0;
        /*float total = 0.0f;
        Array.ForEach(pupilDiameter, val => val += total);
        baseLine = total / pupilDiameter.Length;*/

        //mode = Mode.None;
        float sum = 0;
        Array.ForEach(pupilDiameter, x =>
        {
            sum += x;
        });
        float mean = sum / pupilDiameter.Length;
        sum = 0;
        Array.ForEach(pupilDiameter, x =>
        {
            sum += (x - mean) * (x - mean);
        });
        baseLine = (float)Math.Sqrt(sum / pupilDiameter.Length);
        return baseLine;
    }

    public float BaseLine
    {
        get { return baseLine; }
    }
}
