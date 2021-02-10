using System;

[Serializable]
public class Trial
{    
    //public string observerName;
    //public int observerId;    
    public PupilDataBaseline[] pupilDataBaselines;
    public PupilDataTrial[] pupilDataTrials;
    public TrialType trialType;
    public long trialId;
    public Observer observer;

    public Trial(long _trialId, TrialType _trialType, PupilDataBaseline[] _pupilDataBaselines, PupilDataTrial[] _pupilDataTrials){
        trialId = _trialId;
        trialType = _trialType;
        pupilDataBaselines = _pupilDataBaselines;
        pupilDataTrials = _pupilDataTrials;
    }
}

public enum TrialType{Blind, Feedback}
