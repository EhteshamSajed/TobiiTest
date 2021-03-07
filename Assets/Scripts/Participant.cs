using System;
using System.Collections.Generic;

[Serializable]
public class Participant
{
    public string participantsId;
    public string participantName;
    public Trial[] trials;
    public Participant()
    {
    }

    public Participant(string _participantsId, string _participantName)
    {
        participantsId = _participantsId;
        participantName = _participantName;
    }

    public Participant(string _participantsId, string _participantName, Trial[] _trials)
    {
        participantsId = _participantsId;
        participantName = _participantName;
        trials = _trials;
    }

    /*public Participant(string _participantsId, string _participantName, Trial _trial){
        participantsId = _participantsId;
        participantName = _participantName;
        trial = _trial;
    }*/
    public void AddTrial(Trial _trial)
    {
        List<Trial> trialList =new List<Trial>();
        if (trials !=null)
            trialList = new List<Trial>(trials);
        if (trialList.Exists(item => item.trialId == _trial.trialId))
        {
            trialList[trialList.FindIndex(item => item.trialId == _trial.trialId)] = _trial;
        }
        else
        {
            trialList.Add(_trial);
        }
        trials = trialList.ToArray();
    }
    public Trial GetTrial(long trialId){
        List<Trial> trialList = new List<Trial>(trials);
        return trialList.Find(item => item.trialId == trialId);
    }
    // public bool TrialExists(long trialId){
    //     return Array.Exists(trials, trial => trial.trialId == trialId);
    // }
}
