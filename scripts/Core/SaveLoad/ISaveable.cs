public interface ISaveable
{
    string ID { get; }
    object CaptureState();
    void RestoreState(object state);
}