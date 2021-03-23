using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    GameObject liveFeedbackPanel;
    [SerializeField]
    Button button1, button2;
    [SerializeField]
    GameObject introPanel;
    [SerializeField]
    GameObject observerPanel;
    [SerializeField]
    GameObject hudPanel, bottomHudPanel;
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
    [SerializeField]
    float pupilsizeFactor;
    Observer observer;
    Participant participant;
    List<float> diameterList = new List<float>();
    float currentPupilDiameter;
    long startTimeStamp;
    long startTicks;
    int questionId = -1;
    List<PupilDataTrial> pupilDataTrials = new List<PupilDataTrial>();
    List<PupilDataBaseline> pupilDataBaselines = new List<PupilDataBaseline>();
    public static DBController dBController;
    static ParticipantsController participantsController;
    Text questionNumberText;
    Text remainingTimeText;
    Slider timerSlider;
    Button savedStudiesCancelButton;
    ScrollRect savedStudiesScrollRect;
    Text timerText;
    Text circleText;
    LineRenderer pupilReferenceLineRenderer;
    LineRenderer liveFeedbackLineRenderer;
    Trial trial;
    TrialsRecord trialsRecord;
    long elapseTicksToAnswer;
    int participantAnswer;
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("ConductTrial");
        if (EyeTrackingOperations.FindAllEyeTrackers().Count != 0)
            eyeTracker = EyeTrackingOperations.FindAllEyeTrackers()[0];
        else
            eyeTracker = gameObject.GetComponent<DummyEyeTracker>();
        dBController = new DBController(DBController.LoadParticipantsList());
        participantsController = new ParticipantsController();
        initUI();
    }
    private void DrawCircle(LineRenderer _lineRenderer, float _lineWidth, float _radius)
    {
        int vertexCount = 40;
        _lineRenderer.widthMultiplier = _lineWidth;

        float deltaTheta = (2.1f * Mathf.PI) / vertexCount;
        float theta = 0f;

        _lineRenderer.positionCount = vertexCount;
        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            Vector3 pos = new Vector3(_radius * Mathf.Cos(theta), _radius * Mathf.Sin(theta), 0f);
            _lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
    void initUI()
    {
        isObserverToggle.onValueChanged.AddListener((isObserver) => ToggleParticipantMode(isObserver));
        proceedButton.onClick.AddListener(StartTest);
        button1.onClick.AddListener(() => SaveAnswer(1));
        button2.onClick.AddListener(() => SaveAnswer(2));
        questionNumberText = hudPanel.transform.Find("Question Number Text").GetComponent<Text>();
        timerSlider = hudPanel.transform.Find("Timer Panel").Find("Timer Slider").GetComponent<Slider>();
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
        timerText = hudPanel.transform.Find("Timer Panel").Find("Timer Text").GetComponent<Text>();
        circleText = participantsPanel.transform.Find("Circle Text").GetComponent<Text>();
        pupilReferenceLineRenderer = liveFeedbackPanel.transform.Find("Pupil Reference LineRenderer").GetComponent<LineRenderer>();
        liveFeedbackLineRenderer = liveFeedbackPanel.transform.Find("Live Feedback LineRenderer").GetComponent<LineRenderer>();
        bottomHudPanel.transform.Find("Replay Button").GetComponent<Button>().onClick.AddListener(() => { questionId--; NextQuestion(); });
        bottomHudPanel.transform.Find("Next Button").GetComponent<Button>().onClick.AddListener(() => { SaveObserversPrediction(); NextQuestion(); });
        introPanel.transform.Find("Exit Button").GetComponent<Button>().onClick.AddListener(() => Application.Quit());
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
            Transform item = (Transform)Instantiate(scrollViewPanelTemplate);
            item.Find("Serial Text").GetComponent<Text>().text = (i + 1).ToString();
            item.Find("ID Text").GetComponent<Text>().text = trialsRecords[i].id.ToString();
            item.Find("Time Text").GetComponent<Text>().text = (UnixTimeStampToDateTime(trialsRecords[i].id)).ToString();
            item.Find("Type Text").GetComponent<Text>().text = trialsRecords[i].type.ToString();
            item.Find("Observed Image").GetComponent<Image>().sprite = crossTickSprites[trialsRecords[i].observed ? 1 : 0];
            int copy = i;
            item.GetComponent<Button>().onClick.AddListener(() => LoadTrialForObserver(trialsRecords[copy]));
            item.parent = savedStudiesScrollRect.content.transform;
            item.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            item.gameObject.SetActive(true);
        }

    }
    private void onGazeDataReceived(object sender, GazeDataEventArgs e)
    {
        if (isObserver) return;

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
            DrawCircle(liveFeedbackLineRenderer, 0.05f, currentPupilDiameter * pupilsizeFactor);
        }
    }
    private IEnumerator SimulatePupilDiameter(float[] pupilDiameter, long duration)
    {
        double wait = Math.Round(TimeSpan.FromTicks(duration).TotalSeconds / pupilDiameter.Length, 2);
        for (int i = 0; i < pupilDiameter.Length; i++)
        {
            DrawCircle(liveFeedbackLineRenderer, 0.05f, pupilDiameter[i] * pupilsizeFactor);
            yield return new WaitForSeconds((float)wait);
        }
        yield return null;
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
        participantsPanel.transform.Find("Participants Answer Text").gameObject.SetActive(_isObserver);
        trialTypeDropdown.interactable = !_isObserver;
        if (_isObserver)
        {
            button1.transform.GetComponentInChildren<Text>().text = "True";
            button2.transform.GetComponentInChildren<Text>().text = "False";

        }
        else
        {
            button1.transform.GetComponentInChildren<Text>().text = "Yes";
            button2.transform.GetComponentInChildren<Text>().text = "No";
        }
    }

    void StartRealTest()
    {
    }
    void StartDemoTest()
    {
    }
    bool CreateParticipantOrObserver()
    {
        string name = nameInputField.text;
        if (String.IsNullOrEmpty(name))
        {
            return false;
        }

        if (isObserver)
        {
            string trialId = trialIdInputField.text;
            if (String.IsNullOrEmpty(trialId))
            {
                return false;
            }
            observer = new Observer(DateTimeToUnixTimeStamp(System.DateTime.Now).ToString(), name);
            Debug.Log("new observer created");
            trial.observer = observer;
            DrawCircle(pupilReferenceLineRenderer, trial.pupilDataBaselines[0].StandardDeviation * 5, trial.pupilDataBaselines[0].Mean * pupilsizeFactor);
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
        return true;
    }
    void LoadTrialForObserver(TrialsRecord _trialsRecord)
    {
        if (_trialsRecord.observed) return;
        Debug.Log(_trialsRecord.id);
        trialsRecord = _trialsRecord;
        savedStudiesPanel.SetActive(false);
        trialIdInputField.text = _trialsRecord.id.ToString();
        trial = ParticipantsController.LoadParticipantbyParticipantId(dBController.GetParticipantsRecordByTrialId(_trialsRecord.id).participantId).GetTrial(_trialsRecord.id);
        Debug.Log(trial.trialId);
    }
    public void StartTest()
    {
        if (!isObserver)
        {
            if (eyeTracker == null)
                Debug.LogError("No Eye tracker found!");
            eyeTracker.GazeDataReceived -= onGazeDataReceived;
            eyeTracker.GazeDataReceived += onGazeDataReceived;
        }
        questionId = -1;
        pupilDataBaselines.Clear();
        pupilDataTrials.Clear();
        if (CreateParticipantOrObserver())
        {
            NextQuestion();
        }
    }
    void NextQuestion()
    {
        introPanel.SetActive(false);
        questionId++;
        if (questionId < questions.Length)
        {
            if (isObserver)
            {
                if (trial.pupilDataTrials[questionId].participantAnswer == Answer.NotGiven)
                {
                    NextQuestion();
                    return;
                }
                StartCoroutine(ShowObserverPanel());
            }
            else
            {
                if (questions[questionId].calculateBaseline)
                    StartCoroutine(ShowBaselinePanel());
                else
                    StartCoroutine(ShowParticipantsQuestion());
            }
        }
        else
        {
            Debug.Log("Completed");
            mode = Mode.None;
            SaveSession();
        }
    }
    IEnumerator ShowBaselinePanel()
    {
        mode = Mode.Baseine;
        startTimeStamp = DateTimeToUnixTimeStamp(System.DateTime.Now);
        startTicks = System.DateTime.Now.Ticks;
        participantsPanel.SetActive(false);
        baselinePanel.SetActive(true);
        hudPanel.transform.Find("Timer Panel").gameObject.SetActive(false);
        float timeLeft = durationForBaseline;
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        SaveBaseline();
        StartCoroutine(ShowParticipantsQuestion());
    }
    void SaveBaseline()
    {
        mode = Mode.None;
        long durationInTicks = System.DateTime.Now.Ticks - startTicks;
        PupilDataBaseline pupilDataBaseline = new PupilDataBaseline(diameterList.ToArray(), questionId, startTimeStamp, durationInTicks);
        diameterList.Clear();
        pupilDataBaselines.Add(pupilDataBaseline);
        DrawCircle(pupilReferenceLineRenderer, pupilDataBaseline.StandardDeviation * 5, pupilDataBaseline.Mean * pupilsizeFactor);
    }
    IEnumerator ShowParticipantsQuestion()
    {
        mode = Mode.None;
        participantAnswer = (int)Answer.NotGiven;
        elapseTicksToAnswer = 0;
        SetupParticipantAndObserverPanel(1);
        SetupQuestionColor(questions[questionId]);
        float timerToShowCircleText = 2.00f;
        while (timerToShowCircleText > 0 && !String.IsNullOrEmpty(questions[questionId].circleString))
        {
            timerToShowCircleText -= Time.deltaTime;
            yield return null;
        }
        startTicks = System.DateTime.Now.Ticks;
        mode = Mode.Normal;
        circleText.text = questions[questionId].circleString;
        SetupParticipantAndObserverPanel(2);
        timerSlider.value = 1;
        float timeLeft = questions[questionId].durationForQuestion;
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerSlider.value = timeLeft / questions[questionId].durationForQuestion;
            timerText.text = Math.Ceiling(timeLeft).ToString() + "/" + questions[questionId].durationForQuestion.ToString();
            yield return null;
        }
        mode = Mode.None;
        long durationInTicks = System.DateTime.Now.Ticks - startTicks;
        PupilDataTrial pupilDataTrial = new PupilDataTrial(diameterList.ToArray(), questionId, startTimeStamp, durationInTicks, questions[questionId], (Answer)participantAnswer, elapseTicksToAnswer);
        diameterList.Clear();
        pupilDataTrials.Add(pupilDataTrial);
        Debug.Log("pupilDataTrial " + pupilDataTrial.ToString());
        NextQuestion();
    }
    void SaveAnswer(int _participantAnswer = 0)
    {
        button1.interactable = false;
        button2.interactable = false;
        Button b = _participantAnswer == 1 ? button1 : button2;
        b.image.color = Color.blue;
        elapseTicksToAnswer = System.DateTime.Now.Ticks - startTicks;
        participantAnswer = _participantAnswer;
    }
    void SaveSession()
    {
        if (!isObserver)
            trial = new Trial(DateTimeToUnixTimeStamp(System.DateTime.Now), feedbackType, pupilDataBaselines.ToArray(), pupilDataTrials.ToArray());
        else
            participant = ParticipantsController.GetParticipantByTrial(trial.trialId);
        participant.AddTrial(trial);
        participantsController.AddParticipant(participant);
        ParticipantsController.SaveParticipant(participant);

        if (!isObserver)
            trialsRecord = new TrialsRecord(trial.trialId, feedbackType, isObserver);
        trialsRecord.observed = isObserver;
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
    void SetupParticipantAndObserverPanel(int stage)
    {
        if (stage == 1)
        {
            participantsPanel.SetActive(true);
            baselinePanel.SetActive(false);
            liveFeedbackPanel.SetActive(false);
            button1.gameObject.SetActive(false);
            button2.gameObject.SetActive(false);
            button1.interactable = false;
            button2.interactable = false;
            hudPanel.transform.Find("Timer Panel").gameObject.SetActive(false);
            circleText.transform.Find("Live Feedback").gameObject.SetActive(false);
            circleText.text = "";
            questionNumberText.text = "Question <b>" + (questionId + 1) + "</b>/" + questions.Length;
            bottomHudPanel.SetActive(false);
        }
        else if (stage == 2)
        {
            liveFeedbackPanel.SetActive(feedbackType == FeedbackType.Double || isObserver);
            circleText.transform.Find("Live Feedback").gameObject.SetActive(!liveFeedbackPanel.activeSelf);
            hudPanel.transform.Find("Timer Panel").gameObject.SetActive(true);
            button1.gameObject.SetActive(true);
            button2.gameObject.SetActive(true);
            button1.image.color = Color.white;
            button2.image.color = Color.white;
            button1.interactable = true;
            button2.interactable = true;
        }
        circleText.gameObject.SetActive(circleText.text != "");
    }
    void SetupQuestionColor(Question question)
    {
        participantsPanel.transform.Find("Question Text").GetComponent<Text>().text = question.questionText;
        if (question.condition == Condition.Free)
            participantsPanel.transform.Find("Question Text").GetComponent<Text>().color = Color.black;
        else if (question.condition == Condition.True)
            participantsPanel.transform.Find("Question Text").GetComponent<Text>().color = Color.blue;
        else
            participantsPanel.transform.Find("Question Text").GetComponent<Text>().color = Color.red;
    }
    IEnumerator ShowObserverPanel()
    {
        SetupParticipantAndObserverPanel(1);
        participantsPanel.transform.Find("Participants Answer Text").GetComponent<Text>().text = trial.pupilDataTrials[questionId].participantAnswer.ToString();
        participantsPanel.transform.Find("Participants Answer Text").GetComponent<Text>().color = Color.yellow;
        SetupQuestionColor(trial.pupilDataTrials[questionId].question);
        float timerToShowCircleText = 2.00f;
        while (timerToShowCircleText > 0 && !String.IsNullOrEmpty(trial.pupilDataTrials[questionId].question.circleString))
        {
            timerToShowCircleText -= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(SimulatePupilDiameter(trial.pupilDataTrials[questionId].pupilDiameter, trial.pupilDataTrials[questionId].durationInTicks));
        circleText.text = trial.pupilDataTrials[questionId].question.circleString;
        SetupParticipantAndObserverPanel(2);
        timerSlider.value = 1;
        float timeLeft = trial.pupilDataTrials[questionId].question.durationForQuestion;
        StartCoroutine(ChangeQuesrtionColor(TimeSpan.FromTicks(trial.pupilDataTrials[questionId].elapseTicksToAnswer).TotalSeconds));
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerSlider.value = timeLeft / trial.pupilDataTrials[questionId].question.durationForQuestion;
            timerText.text = Math.Ceiling(timeLeft).ToString() + "/" + trial.pupilDataTrials[questionId].question.durationForQuestion.ToString();
            yield return null;
        }
        bottomHudPanel.SetActive(true);
    }
    IEnumerator ChangeQuesrtionColor(double timeLeft)
    {
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        participantsPanel.transform.Find("Participants Answer Text").GetComponent<Text>().color = Color.blue;
    }
    void SaveObserversPrediction(){
        trial.pupilDataTrials[questionId].observersPrediction = (ObserversPrediction)participantAnswer;        
        Debug.Log("pupilDataTrial " + trial.pupilDataTrials[questionId].ToString());
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