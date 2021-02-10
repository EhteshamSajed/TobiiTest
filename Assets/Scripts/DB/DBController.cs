using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DBController
{
    public List<ParticipantsRecord> participantsRecordList = new List<ParticipantsRecord>();
    public DBController(){
    }
    public DBController(List<ParticipantsRecord> _participantsRecordList){
        participantsRecordList = _participantsRecordList;
    }
    public static DBController LoadDBController()
    {
        string jsonString = "";  //  default, if file not found
        if (JsonUtility.FromJson<DBController>(jsonString) == null)
            return new DBController();
        return JsonUtility.FromJson<DBController>(jsonString);
    }
    public static List<ParticipantsRecord> LoadParticipantsList()
    {
        return LoadDBController().participantsRecordList;
    }
    public static void SaveParticipantsList(DBController _dBController)
    {
        string jsonString = JsonUtility.ToJson(_dBController);
        Debug.Log(jsonString);
    }
    public void SaveParticipantsList()
    {
        string jsonString = JsonUtility.ToJson(this);
        Debug.Log(jsonString);
    }
    public void AddParticipantsRecord(ParticipantsRecord participantsRecord)
    {
        if (participantsRecordList.Exists(item => item.participantName == participantsRecord.participantName))
        {
            participantsRecordList[participantsRecordList.FindIndex(item => item.participantName == participantsRecord.participantName)] = participantsRecord;
        }
        else
        {
            participantsRecordList.Add(participantsRecord);
        }
    }

    public static void TestJSON(){
        ParticipantsRecord participantsRecord1 = new ParticipantsRecord("Sajed", "1");
        participantsRecord1.AddTrialsRecord(new TrialsRecord(123456, TrialType.Blind, true));
        ParticipantsRecord participantsRecord2 = new ParticipantsRecord("Arup", "2");
        participantsRecord2.AddTrialsRecord(new TrialsRecord(987654, TrialType.Feedback, false));

        DBController dBController = new DBController();
        dBController.participantsRecordList.Add(participantsRecord1);
        dBController.participantsRecordList.Add(participantsRecord2);

        Debug.Log(dBController.participantsRecordList[1].trialsRecords[0].id);

        string dBControllerString = JsonUtility.ToJson(dBController);
        Debug.Log(dBControllerString);

        DBController dBController1 = JsonUtility.FromJson<DBController>(dBControllerString);
        Debug.Log(dBController1.participantsRecordList[1].trialsRecords[0].id);
    }
}
