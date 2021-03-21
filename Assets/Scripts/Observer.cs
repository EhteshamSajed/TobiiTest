using System;

public class Observer
{
    public string observerId;
    public string observerName;
    //public string trialId;  //  may be safely removed
    public Observer(string _observerId, string _observerName){
        observerId = _observerId;
        observerName = _observerName;
    }
    /*public Observer(string _observerId, string _observerName, string _trialId){
        observerId = _observerId;
        observerName = _observerName;
        trialId = _trialId; //  load respective trial
    }*/
}

public enum ObserversPrediction {NotGiven, True, False};