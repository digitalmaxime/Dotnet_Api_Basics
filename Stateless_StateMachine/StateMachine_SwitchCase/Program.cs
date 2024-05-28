using Stateless_StateMachine;
using Action = Car_States_Actions.Action;

var car = new Car();

car.TakeAction(Action.Accelerate);
car.TakeAction(Action.Start);
car.TakeAction(Action.Stop);
car.TakeAction(Action.Start);
car.TakeAction(Action.Accelerate);
car.TakeAction(Action.Start);
car.TakeAction(Action.Stop);

