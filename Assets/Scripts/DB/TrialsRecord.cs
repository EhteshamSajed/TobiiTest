using System;

[Serializable]
public class TrialsRecord{
    public long id;
    public TrialType type;
    public bool observed;
    public TrialsRecord(long _id, TrialType _type, bool _observed){
        id = _id;
        type = _type;
        observed = _observed;
    }    
}
