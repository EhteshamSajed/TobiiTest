
using Tobii.Research;

public class DummyPupilData : Tobii.Research.PupilData
{
    public DummyPupilData(float pupilDiameter, Validity validity) : base(pupilDiameter, validity)
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
