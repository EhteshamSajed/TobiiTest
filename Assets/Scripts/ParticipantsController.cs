using UnityEngine;
using System.Collections.Generic;
public class ParticipantsController
{
    //  todo  handle participants id internally
    static List<Participant> participants;
    public ParticipantsController()
    {
        participants = new List<Participant>();
    }
    public ParticipantsController(List<Participant> _participants)
    {
        participants = _participants;
    }
    public Participant GetParticipantByName(string name)
    {
        if (participants.Exists((participant) => participant.participantName == name))
        {
            return participants.Find((participant) => participant.participantName == name);
        }
        ParticipantsRecord participantsRecord = ConductTrial.dBController.GetParticipantsRecordByName(name);
        if (participantsRecord != null)
            return LoadParticipantbyParticipantId(participantsRecord.participantId);
        return null;
    }
    // public Participant GetParticipantByTrial(long id){
    //     return participants.Find(participant => participant.TrialExists(id));
    // }
    public void AddParticipant(Participant _participant)
    {
        if (participants.Exists((participant) => participant.participantName == _participant.participantName))
        {
            participants[participants.FindIndex((participant) => participant.participantName == _participant.participantName)] = _participant;
            return;
        }
        participants.Add(_participant);
    }
    public static void SaveParticipant(Participant _participant)
    {
        string jsonString = JsonUtility.ToJson(_participant);
        Debug.Log(jsonString);
        FileManager.SaveFile(_participant.participantsId + ".dat", jsonString);
    }
    public static Participant LoadParticipantbyParticipantId(string id)
    {
        if (participants.Exists((participant) => participant.participantsId == id))
        {
            Debug.Log("Found in the cache");
            return participants.Find((participant) => participant.participantsId == id); 
        }
        Participant participant = JsonUtility.FromJson<Participant>(FileManager.LoadFile(id + ".dat"));
        Debug.Log("Loading from file...");
        participants.Add(participant);
        return participant;
    }
}
