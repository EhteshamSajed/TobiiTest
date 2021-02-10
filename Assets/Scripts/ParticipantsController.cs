﻿using UnityEngine;
using System.Collections.Generic;
public class ParticipantsController
{
    List<Participant> participants;

    public ParticipantsController()
    {
        participants = new List<Participant>();
        //  load all particpants obj from folder and deserialize it
    }
    public ParticipantsController(List<Participant> _participants)
    {
        participants = _participants;
    }
    public Participant GetParticipantByName(string name)
    {
        return participants.Find((participant) => participant.participantName == name);
    }
    //public Participant GetParticipantByTrial(){}
    public void AddParticipant(Participant _participant)
    {
        if (participants.Exists((participant) => participant.participantName == _participant.participantName))
        {
            participants[participants.FindIndex((participant) => participant.participantName == _participant.participantName)] = _participant;
            return;
        }
        participants.Add(_participant);
    }
}
