using System;

[Serializable]
public class PupilDataTrial : PupilData
{
    public string questionString;
    public int perticipantAnswer;
    public int realAnswer;
    public int observersPrediction;
    public PupilDataTrial(
        float[] _pupilDiameter,
        int _stimuliId,
        long _startTimeStamp,
        long _durationInTicks,
        string _questionString,
        int _perticipantAnswer) : base(_pupilDiameter,
                               _stimuliId,
                               _startTimeStamp,
                               _durationInTicks)
    {
        questionString = _questionString;
    }

    public PupilDataTrial(
        float[] _pupilDiameter,
        int _stimuliId,
        long _startTimeStamp,
        long _durationInTicks,
        string _questionString,
        int _perticipantAnswer,
        int _observersPrediction) : this(_pupilDiameter,
                               _stimuliId,
                               _startTimeStamp,
                               _durationInTicks,
                               _questionString,
                               _perticipantAnswer)
    {
        perticipantAnswer = _perticipantAnswer;
        observersPrediction = _observersPrediction;
    }

    public PupilDataTrial(
        float[] _pupilDiameter,
        int _stimuliId,
        long _startTimeStamp,
        long _durationInTicks,
        string _questionString,
        int _perticipantAnswer,
        int _observersPrediction,
        int _realAnswer) : this(_pupilDiameter,
                               _stimuliId,
                               _startTimeStamp,
                               _durationInTicks,
                               _questionString,
                               _perticipantAnswer,
                               _observersPrediction)
    {
        realAnswer = _realAnswer;
    }
}