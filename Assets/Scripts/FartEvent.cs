public class FartEvent
{
    public float Force { get; private set; }
    public float Angle { get; private set; }
    public FartEvent(float force, float angle)
    {
        Force = force;
        Angle = angle;
    }
}