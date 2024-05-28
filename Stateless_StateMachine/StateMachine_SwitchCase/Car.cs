using Car_States_Actions;
using Action = Car_States_Actions.Action;

namespace Stateless_StateMachine;

public class Car
{
    private State State { get; set; }
    public Action Action { get; set; }

    public void TakeAction(Action action)
    {
        State = (State, action) switch
        {
            (State.Stopped, Action.Start) => State = State.Started,
            (State.Started, Action.Accelerate) => State = State.Running,
            (State.Started, Action.Stop) => State = State.Stopped,
            (State.Running, Action.Stop) => State = State.Stopped,
            _ => State
        };
    }
    
}