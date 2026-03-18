namespace Fort.MG.Network;

public class ServerGameContext : IGameContext
{
    public float Delta { get; internal set; }
    public float TotalGameTime { get; internal set; }
    public double Delta64 { get; internal set; }
    public double TotalGameTime64 { get; internal set; }
}