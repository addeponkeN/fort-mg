namespace Fort.MG;

public interface IGameContext
{
    float Delta { get; }
    float TotalGameTime { get; }
    double Delta64 { get; }
    double TotalGameTime64 { get; }
}
