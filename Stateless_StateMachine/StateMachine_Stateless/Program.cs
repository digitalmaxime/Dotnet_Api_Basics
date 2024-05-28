using Car_States_Actions;
using Action = Car_States_Actions.Action;

var externalState = State.Stopped; // Taken from database

// var car = new Stateless.StateMachine<State, Action>(State.Stopped); // Simple way

// Based on external state storage
var car = new Stateless.StateMachine<State, Action>(
    () => externalState,
    (state) => externalState = state); 

car.Configure(State.Stopped)
    .Permit(Action.Start, State.Started)
    .Ignore(Action.Stop) // --> same as .PermitReentry(Action.Stop)
    ;

car.Configure(State.Started)
    .Permit(Action.Accelerate, State.Running)
    .Permit(Action.Stop, State.Stopped)
    .OnEntry(state =>
        Console.WriteLine(
            $"OnEntry\n State Source : {state.Source}, State Trigger : {state.Trigger}, State destination : {state.Destination}"))
    .OnExit(state =>
        Console.WriteLine(
            $"OnExit\n State Source : {state.Source}, State Trigger : {state.Trigger}, State destination : {state.Destination}"))
    ;

var accelerateWithParam = car.SetTriggerParameters<int>(Action.Accelerate);

car.Configure(State.Running)
    .SubstateOf(State.Started)
    .OnEntryFrom(accelerateWithParam, speed => Console.WriteLine($"Speed is {speed}"))
    .Permit(Action.Stop, State.Stopped)
    .InternalTransition(Action.Start, () => Console.WriteLine("Entered 'Start' while Running"));


// car.Fire(Action.Accelerate);
car.Fire(Action.Start);
// car.Fire(Action.Start);
car.Fire(Action.Stop);
car.Fire(Action.Stop);
car.Fire(Action.Start);
car.Fire(accelerateWithParam, 100);
// car.Fire(Action.Start);
car.Fire(Action.Stop);