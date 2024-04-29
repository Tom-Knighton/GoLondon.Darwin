namespace GoTravel.Darwin.Domain.Interfaces;

public interface IDarwinConsumer
{
    public DateTime LastReceived { get; set; }

    public Task Start();
}