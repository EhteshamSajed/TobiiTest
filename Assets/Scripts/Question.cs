using System;
[Serializable]
public class Question
{
    public string questionText;
    public string circleString;
    public Condition condition;
    public Answer realAnswer;
    public bool calculateBaseline = false;
    public float durationForQuestion = 5.0f;
}
public enum Condition {Free, True, Lie}
public enum Answer {NotGiven, Yes, No}