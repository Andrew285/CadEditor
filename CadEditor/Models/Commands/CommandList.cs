using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Models.Commands
{
    public class CommandList: ICommand
    {
        private List<ICommand> commands;

        public CommandList() 
        {
            commands = new List<ICommand>();
        }

        public CommandList(IEnumerable<ICommand> commands)
        {
            this.commands = new List<ICommand>(commands);
        }

        public void Add(ICommand command)
        {
            commands.Add(command);
        }

        public void AddRange(IEnumerable<ICommand> commands)
        {
            this.commands.AddRange(commands);
        }

        public bool Execute()
        {
            foreach (var command in commands)
            {
                command.Execute();
            }

            return true;
        }

        public void Undo()
        {
            for (int i = commands.Count-1; i >= 0; i--) 
            {
                commands[i].Undo();
            }
        }

        public void Redo()
        {
            Execute();
        }

        public bool IsEmpty()
        {
            return commands.Count == 0;
        }

        public ICommand this[int key]
        {
            get => commands[key];
            set => commands[key] = value;
        }
    }
}
