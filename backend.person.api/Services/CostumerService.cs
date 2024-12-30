namespace backend.person.api.Services;

public class CostumerService: ICostumerService
{
    public string SimulatedMethod()
    {
        throw new Exception("Simulated fail");
    }
}