using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Research.Unity;
using Tobii.Research;
using UnityEngine.UI;
using System;

public class ConductTrial : MonoBehaviour
{
    enum Mode { Baseine, Normal, None };
    Mode mode = Mode.None;
    static bool readPupilDiameter = true;
    IEyeTracker eyeTracker;
    bool isObserver = false;

    [SerializeField]
    Toggle isObserverToggle;
    [SerializeField]
    InputField trialIdInputField;
    [SerializeField]
    Button trialListButton;
    [SerializeField]
    InputField nameInputField;
    [SerializeField]
    InputField idInputField;
    [SerializeField]
    Dropdown trialTypeDropdown;
    [SerializeField]
    Button proceedButton;
    [SerializeField]
    GameObject baselinePanel;
    [SerializeField]
    GameObject participantsPanel;
    [SerializeField]
    GameObject savedStudiesPanel;
    [SerializeField]
    Image liveFeedback;
    [SerializeField]
    Button button1, button2;
    [SerializeField]
    GameObject introPanel;
    [SerializeField]
    GameObject observerPanel;
    [SerializeField]
    GameObject hudPanel;
    [SerializeField]
    GameObject endPanel;
    [SerializeField]
    string[] questionStrings;
    [SerializeField]
    Question[] questions;
    [SerializeField]
    float durationForQuestion;
    [SerializeField]
    float durationForBaseline;
    [SerializeField]
    FeedbackType feedbackType;
    [SerializeField]
    Sprite[] crossTickSprites;
    Observer observer;
    Participant participant;
    List<float> diameterList = new List<float>();
    float currentPupilDiameter;
    long startTimeStamp;
    long startTicks;
    float timeLeft;
    int questionId = 0;
    List<PupilDataTrial> pupilDataTrials = new List<PupilDataTrial>();
    List<PupilDataBaseline> pupilDataBaselines = new List<PupilDataBaseline>();
    static DBController dBController;
    static ParticipantsController participantsController;
    Text questionNumberText;
    Text remainingTimeText;
    Slider timerSlider;
    Button savedStudiesCancelButton;
    ScrollRect savedStudiesScrollRect;
    Text timerText;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("ConductTrial");
        if (EyeTrackingOperations.FindAllEyeTrackers().Count != 0)
            eyeTracker = EyeTrackingOperations.FindAllEyeTrackers()[0];
        else
            eyeTracker = gameObject.GetComponent<DummyEyeTracker>();
        if (eyeTracker != null)
            eyeTracker.GazeDataReceived += onGazeDataReceived;
        dBController = new DBController(DBController.LoadParticipantsList());
        participantsController = new ParticipantsController();
        initUI();
    }
    void initUI()
    {
        isObserverToggle.onValueChanged.AddListener((isObserver) => ToggleParticipantMode(isObserver));
        proceedButton.onClick.AddListener(StartTest);
        button1.onClick.AddListener(() => SaveAnswer(1));
        button2.onClick.AddListener(() => SaveAnswer(2));
        questionNumberText = hudPanel.transform.Find("Question Number Text").GetComponent<Text>();
        timerSlider = hudPanel.transform.Find("Timer Slider").GetComponent<Slider>();
        savedStudiesCancelButton = savedStudiesPanel.transform.Find("Cancel Button").GetComponent<Button>();
        savedStudiesScrollRect = savedStudiesPanel.transform.Find("Scroll View").GetComponent<ScrollRect>();
        savedStudiesCancelButton.onClick.AddListener(() => savedStudiesPanel.SetActive(false));
        trialListButton.onClick.AddListener(() =>
        {
            AddItemsInSavedStudiesScrollRect(dBController);
            savedStudiesPanel.gameObject.SetActive(true);
        });
        endPanel.transform.Find("CloseEndPanel Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            endPanel.SetActive(false);
            introPanel.SetActive(true);
        });
        timerText = hudPanel.transform.Find("Timer Text").GetComponent<Text>();
    }
    void AddItemsInSavedStudiesScrollRect(DBController dBController)
    {
        List<TrialsRecord> trialsRecords = new List<TrialsRecord>();
        dBController.participantsRecordList.ForEach((participantsRecord) =>
        {
            trialsRecords.AddRange(participantsRecord.trialsRecords);
        });
        trialsRecords.Sort((a, b) => a.id.CompareTo(b.id));
        foreach (Transform item in savedStudiesScrollRect.content.transform)
        {
            if (item.gameObject.activeSelf)
            {
                Destroy(item.gameObject);
            }
        }
        Transform scrollViewPanelTemplate = savedStudiesScrollRect.content.Find("Panel").GetComponent<Transform>();
        for (int i = 0; i < trialsRecords.Count; i++)
        {
            Transform item = (Transform)Instantiate(scrollViewPanelTemplate, new Vector3(), Quaternion.identity);
            item.Find("Serial Text").GetComponent<Text>().text = (i + 1).ToString();
            item.Find("ID Text").GetComponent<Text>().text = trialsRecords[i].id.ToString();
            item.Find("Time Text").GetComponent<Text>().text = (UnixTimeStampToDateTime(trialsRecords[i].id)).ToString();
            item.Find("Type Text").GetComponent<Text>().text = trialsRecords[i].type.ToString();
            item.Find("Observed Image").GetComponent<Image>().sprite = crossTickSprites[trialsRecords[i].observed ? 1 : 0];
            int copy = i;
            item.GetComponent<Button>().onClick.AddListener(() => LoadTrialForObserver(trialsRecords[copy].id));
            item.parent = savedStudiesScrollRect.content.transform;
            item.gameObject.SetActive(true);
        }

    }
    private void onGazeDataReceived(object sender, GazeDataEventArgs e)
    {
        /*if (readPupilDiameter)
            Debug.Log("Left pupil diameter: " + e.LeftEye.Pupil.PupilDiameter);*/

        if (mode != Mode.None)
        {
            if (e.LeftEye.Pupil.Validity == Validity.Valid && e.RightEye.Pupil.Validity == Validity.Valid)         //  ! check further for invalid data
            {
                diameterList.Add((e.LeftEye.Pupil.PupilDiameter + e.RightEye.Pupil.PupilDiameter) / 2);
            }
            else if (e.LeftEye.Pupil.Validity == Validity.Valid && e.RightEye.Pupil.Validity == Validity.Invalid)
            {
                diameterList.Add(e.LeftEye.Pupil.PupilDiameter);
            }
            else if (e.LeftEye.Pupil.Validity == Validity.Invalid && e.RightEye.Pupil.Validity == Validity.Valid)
            {
                diameterList.Add(e.RightEye.Pupil.PupilDiameter);
            }
            currentPupilDiameter = diameterList[diameterList.Count - 1];
            liveFeedback.rectTransform.sizeDelta = new Vector2(500 * currentPupilDiameter, 500 * currentPupilDiameter);
        }
    }
    void OnApplicationQuit()
    {
        if (eyeTracker != null)
            eyeTracker.GazeDataReceived -= onGazeDataReceived;
    }
    void ToggleParticipantMode(bool _isObserver)
    {
        isObserver = _isObserver;
        trialIdInputField.interactable = isObserver;
        trialListButton.interactable = isObserver;
    }

    void StartRealTest()
    {
    }
    void StartDemoTest()
    {
    }
    void CreateParticipantOrObserver()
    {
        string name = nameInputField.text;
        string id = idInputField.text;

        if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(id))
        {
            return;
        }

        if (isObserver)
        {
            string trialId = trialIdInputField.text;
            if (String.IsNullOrEmpty(trialId))
            {
                return;
            }
            //observer = new Observer(id, name, trialId);
            observer = new Observer(id, name);      //load respective trial and assign this onto that trial
            Debug.Log("new observer created");
        }
        else
        {
            participant = participantsController.GetParticipantByName(name);
            if (participant == null)
            {
                participant = new Participant(DateTimeToUnixTimeStamp(System.DateTime.Now).ToString(), name);
                Debug.Log("new participant created");
            }
            feedbackType = (FeedbackType)trialTypeDropdown.value;
        }
    }
    void LoadTrialForObserver(long id)
    {
        Debug.Log(id);
        savedStudiesPanel.SetActive(false);
        trialIdInputField.text = id.ToString();
    }
    public void StartTest()
    {
        questionId = 0;
        pupilDataBaselines.Clear();
        pupilDataTrials.Clear();
        CreateParticipantOrObserver();
        NextQuestion();
        /*CreateParticipantOrObserver();
        StartDemoTest();
        StartRealTest();*/
    }
    void NextQuestion()
    {
        introPanel.SetActive(false);
        // if (questionId < questionStrings.Length)
        // {
        //     questionNumberText.text = "Question <b>" + (questionId + 1) + "</b>/" + questionStrings.Length;
        //     StartCoroutine("ShowBaselinePanel");
        // }
        if (questionId < questions.Length)
        {
            questionNumberText.text = "Question <b>" + (questionId + 1) + "</b>/" + questions.Length;
            StartCoroutine("ShowBaselinePanel");
        }
        else
        {
            Debug.Log("Completed");
            mode = Mode.None;
            if (isObserver)
            {
            }
            else
            {
                Trial trial = new Trial(DateTimeToUnixTimeStamp(System.DateTime.Now), feedbackType, pupilDataBaselines.ToArray(), pupilDataTrials.ToArray());
                participant.AddTrial(trial);
                participantsController.AddParticipant(participant);
                ParticipantsController.SaveParticipant(participant);

                TrialsRecord trialsRecord = new TrialsRecord(trial.trialId, feedbackType, isObserver);
                ParticipantsRecord participantsRecord = dBController.GetParticipantsRecordByName(participant.participantName);
                if (participantsRecord == null)
                    participantsRecord = new ParticipantsRecord(participant.participantName, participant.participantsId);
                participantsRecord.AddTrialsRecord(trialsRecord);
                dBController.AddParticipantsRecord(participantsRecord);
                dBController.SaveParticipantsList();

                introPanel.SetActive(false);
                participantsPanel.SetActive(false);
                endPanel.SetActive(true);
            }
        }
    }
    IEnumerator ShowBaselinePanel()
    {
        /*Debug.Log(UnixTimeStampToDateTime(DateTimeToUnixTimeStamp(new DateTime(2021, 6, 2, 15, 2,2))));
        Debug.Log(UnixTimeStampToDateTime(DateTimeToUnixTimeStamp(System.DateTime.Now)));
        Debug.Log("Wait for " + durationForBaseline);
        Debug.Log("Time starts! " + System.DateTime.Now);
        Debug.Log(System.DateTime.Now.Ticks);
        Debug.Log(DateTimeToUnixTimeStamp(System.DateTime.Now));
        Debug.Log(DateTimeToUnixTimeStamp(DateTime.Now));
        Debug.Log(DateTimeToUnixTimeStamp(DateTime.UtcNow));
        Debug.Log(DateTimeOffset.Now.ToUnixTimeSeconds());
        Debug.Log((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);*/

        mode = Mode.Baseine;
        startTimeStamp = DateTimeToUnixTimeStamp(System.DateTime.Now);
        startTicks = System.DateTime.Now.Ticks;
        participantsPanel.SetActive(false);
        baselinePanel.SetActive(true);
        timerSlider.value = 1;
        float timeLeft = durationForBaseline;
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerSlider.value = timeLeft / durationForBaseline;
            timerText.text = Math.Ceiling(timeLeft).ToString() + "/" + durationForBaseline.ToString();
            yield return null;
        }
        /*Debug.Log("Time up! " + System.DateTime.Now);
        Debug.Log(System.DateTime.Now.Ticks);
        Debug.Log(DateTimeToUnixTimeStamp(System.DateTime.Now));
        Debug.Log(UnixTimeStampToDateTime(DateTimeToUnixTimeStamp(System.DateTime.Now)));*/
        SaveBaseline();
        StartCoroutine("ShowParticipantsQuestion");
    }
    IEnumerator ShowParticipantsQuestion()
    {
        /*Debug.Log("Wait for " + durationForQuestion);
        Debug.Log("Time starts! " + System.DateTime.Now);*/
        mode = Mode.Normal;
        participantsPanel.SetActive(true);
        baselinePanel.SetActive(false);
        mode = Mode.Normal;
        participantsPanel.transform.Find("Question Text").GetComponent<Text>().text = questions[questionId].questionText;
        timerSlider.value = 1;
        float timeLeft = durationForQuestion;
        while (timeLeft > 0 && mode == Mode.Normal)
        {
            timeLeft -= Time.deltaTime;
            timerSlider.value = timeLeft / durationForQuestion;
            timerText.text = Math.Ceiling(timeLeft).ToString() + "/" + durationForQuestion.ToString();
            yield return null;
        }
        //Debug.Log("Time up! " + System.DateTime.Now);
        if (mode == Mode.Normal)
            SaveAnswer(0);
        questionId++;
        NextQuestion();
    }
    void CalculateBaseline()
    {
        mode = Mode.None;
        float sum = 0;
        diameterList.ForEach(x =>
        {
            sum += x;
        });
        float mean = sum / diameterList.Count;
        sum = 0;
        diameterList.ForEach(x =>
        {
            sum += (x - mean) * (x - mean);
        });
        float stdDeviation = (float)Math.Sqrt(sum / diameterList.Count);
        Debug.Log("stdDeviation: " + stdDeviation);
        diameterList.Clear();
    }
    void SaveBaseline()
    {
        mode = Mode.None;
        long durationInTicks = System.DateTime.Now.Ticks - startTicks;
        PupilDataBaseline pupilDataBaseline = new PupilDataBaseline(diameterList.ToArray(), questionId, startTimeStamp, durationInTicks);
        diameterList.Clear();
        pupilDataBaselines.Add(pupilDataBaseline);
    }

    void SaveAnswer(int participantAnswer = 0)
    {
        Debug.Log(participantAnswer);
        mode = Mode.None;
        long durationInTicks = System.DateTime.Now.Ticks - startTicks;
        //PupilDataTrial pupilDataTrial = new PupilDataTrial(diameterList.ToArray(), questionId, startTimeStamp, durationInTicks, questionStrings[questionId], participantAnswer);
        PupilDataTrial pupilDataTrial = new PupilDataTrial(diameterList.ToArray(), questionId, startTimeStamp, durationInTicks, questions[questionId], participantAnswer);
        diameterList.Clear();
        pupilDataTrials.Add(pupilDataTrial);
    }
    static long DateTimeToUnixTimeStamp(DateTime dateTime)
    {
        //return (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        //return DateTimeOffset.Now.ToUnixTimeSeconds();
        DateTimeOffset dto = new DateTimeOffset(dateTime);
        return dto.ToUnixTimeSeconds();
    }
    static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }
}