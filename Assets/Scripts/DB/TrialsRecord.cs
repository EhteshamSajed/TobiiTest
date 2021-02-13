using System;

[Serializable]
public class TrialsRecord{
    public long id;
    public FeedbackType type;
    public bool observed;
    public TrialsRecord(long _id, FeedbackType _type, bool _observed){
        id = _id;
        type = _type;
        observed = _observed;
    }    
}
