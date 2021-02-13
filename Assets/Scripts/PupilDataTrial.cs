using System;

[Serializable]
public class PupilDataTrial : PupilData
{
    //  add a new variable question type Question
    public Question question;
    //public string questionString;   //  to be moved too Question class
    public int perticipantAnswer;   //  use Question.Answer data type
    //public int realAnswer;   //  to be moved too Question class
    public int observersPrediction;
    public PupilDataTrial(
        float[] _pupilDiameter,
        int _stimuliId,
        long _startTimeStamp,
        long _durationInTicks,
        Question _question,
        int _perticipantAnswer) : base(_pupilDiameter,
                               _stimuliId,
                               _startTimeStamp,
                               _durationInTicks)
    {
        question = _question;
    }

    public PupilDataTrial(
        float[] _pupilDiameter,
        int _stimuliId,
        long _startTimeStamp,
        long _durationInTicks,
        Question _question,
        int _perticipantAnswer,
        int _observersPrediction) : this(_pupilDiameter,
                               _stimuliId,
                               _startTimeStamp,
                               _durationInTicks,
                               _question,
                               _perticipantAnswer)
    {
        perticipantAnswer = _perticipantAnswer;
        observersPrediction = _observersPrediction;
    }
}