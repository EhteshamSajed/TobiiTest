
using Tobii.Research;

public class DummyEyeData : Tobii.Research.EyeData
{
    public DummyEyeData(GazePoint gazePoint, Tobii.Research.PupilData pupil, GazeOrigin gazeOrigin) : base(gazePoint, pupil, gazeOrigin)
    {
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
