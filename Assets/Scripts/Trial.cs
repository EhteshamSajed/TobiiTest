using System;

[Serializable]
public class Trial
{    
    //public string observerName;
    //public int observerId;    
    public PupilDataBaseline[] pupilDataBaselines;
    public PupilDataTrial[] pupilDataTrials;
    public FeedbackType feedbackType;
    public long trialId;
    public Observer observer;

    public Trial(long _trialId, FeedbackType _trialType, PupilDataBaseline[] _pupilDataBaselines, PupilDataTrial[] _pupilDataTrials){
        trialId = _trialId;
        feedbackType = _trialType;
        pupilDataBaselines = _pupilDataBaselines;
        pupilDataTrials = _pupilDataTrials;
    }
}

public enum FeedbackType{Single, Double}
