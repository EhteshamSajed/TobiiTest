using System;
using System.Collections.Generic;

[Serializable]
public class ParticipantsRecord{    
    public string participantName;
    public string participantId;
    public TrialsRecord[] trialsRecords;

    public ParticipantsRecord (string _participantName, string _participantId){
        participantName = _participantName;
        participantId = _participantId;
    }

    public ParticipantsRecord (string _participantName, string _participantId, TrialsRecord[] _trialsRecords){
        participantName = _participantName;
        participantId = _participantId;
        trialsRecords = _trialsRecords;
    }

    public void AddTrialsRecord(TrialsRecord _trialsRecord)
    {
        List<TrialsRecord> trialsRecordsList = new List<TrialsRecord>();
        if (trialsRecords != null)
            trialsRecordsList = new List<TrialsRecord>(trialsRecords);
        if (trialsRecordsList.Exists(item => item.id == _trialsRecord.id))
        {
            trialsRecordsList[trialsRecordsList.FindIndex(item => item.id == _trialsRecord.id)] = _trialsRecord;
        }
        else
        {
            trialsRecordsList.Add(_trialsRecord);
        }
        trialsRecords = trialsRecordsList.ToArray();
    }
}
