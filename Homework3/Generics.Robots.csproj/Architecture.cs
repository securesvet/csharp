using System;
using System.Collections.Generic;

namespace Generics.Robots
{
    // RobotAI - parent
    // childs: ShooterAI, BuilderAI
    // Device - parent
    // childs: mover, shootermover
    public interface IRobotAI<out IMoveCommand>
    {
        IMoveCommand GetCommand();
    }

    public class ShooterAI : IRobotAI<ShooterCommand>
    {
        private int _counter = 1;
        
        public ShooterCommand GetCommand()
        {
           return ShooterCommand.ForCounter(_counter++); 
        }
    }

    public class BuilderAI : IRobotAI<BuilderCommand>
    {
        private int _counter = 1;

        public BuilderCommand GetCommand()
        {
            return BuilderCommand.ForCounter(_counter++);
        }
    }

    public interface IDevice<in IMoveCommand>
    {
        string ExecuteCommand(IMoveCommand command);
    }

    public class Mover : IDevice<IMoveCommand>
    {
        // В аргументы лучше передавать _command или command? А то непонятно получается:
        // Если есть свойство private - пишем _property, Не хочется то же самое здесь
        // использовать
        public string ExecuteCommand(IMoveCommand _command)
        {
            // Убрал приведение к IMoveCommand, сделал checkvarfornull
            var command = _command ?? throw new ArgumentNullException(nameof(_command));
            return $"MOV {command.Destination.X}, {command.Destination.Y}";
        }
    }

    public class ShooterMover : IDevice<IMoveCommand>
    {
        public string ExecuteCommand(IMoveCommand _command)
        {
            var command = _command as IShooterMoveCommand ?? throw new ArgumentNullException(nameof(_command));
            var hide = command.ShouldHide ? "YES" : "NO";
            return $"MOV {command.Destination.X}, {command.Destination.Y}, USE COVER {hide}";
        }
    }

    public class Robot<T> where T: IMoveCommand
    {
        private readonly IRobotAI<T> _ai;
        private readonly IDevice<T> _device;

        public Robot(IRobotAI<T> ai, IDevice<T> executor)
        {
            this._ai = ai;
            this._device = executor;
        }

        public IEnumerable<string> Start(int steps)
        {
            for (var i = 0; i < steps; i++)
            {
                var command = _ai.GetCommand();
                if (command == null)
                    break;
                yield return _device.ExecuteCommand(command);
            }
        }
    }

    public static class Robot
    {
        public static Robot<T> Create<T>(IRobotAI<T> ai, IDevice<T> executor) where T : IMoveCommand
        {
            return new Robot<T>(ai, executor);
        }
    }
}
