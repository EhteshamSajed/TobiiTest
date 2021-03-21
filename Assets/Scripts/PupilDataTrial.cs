using System;

[Serializable]
public class PupilDataTrial : PupilData
{
    //  add a new variable question type Question
    public Question question;
    //public string questionString;   //  to be moved too Question class
    public Answer participantAnswer;   //  use Question.Answer data type
    //public int realAnswer;   //  to be moved too Question class
    public ObserversPrediction observersPrediction;
    public long elapseTicksToAnswer;
    public PupilDataTrial(
        float[] _pupilDiameter,
        int _stimuliId,
        long _startTimeStamp,
        long _durationInTicks,
        Question _question,
        Answer _participantAnswer,
        long _elapseTicksToAnswer) : base(_pupilDiameter,
                               _stimuliId,
                               _startTimeStamp,
                               _durationInTicks)
    {
        question = _question;
        participantAnswer = _participantAnswer;
        elapseTicksToAnswer = _elapseTicksToAnswer;
    }
    public PupilDataTrial(
        float[] _pupilDiameter,
        int _stimuliId,
        long _startTimeStamp,
        long _durationInTicks,
        Question _question,
        Answer _participantAnswer,
        long _elapseTicksToAnswer,
        ObserversPrediction _observersPrediction) : this(_pupilDiameter,
                               _stimuliId,
                               _startTimeStamp,
                               _durationInTicks,
                               _question,
                               _participantAnswer,
                               _elapseTicksToAnswer)
    {
        observersPrediction = _observersPrediction;
    }
}