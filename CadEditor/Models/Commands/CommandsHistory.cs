using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Models.Commands
{
    public class CommandsHistory
    {
        private Stack<ICommand> commands;
        private int index;

        public CommandsHistory() 
        {
            commands = new Stack<ICommand>();
            index = 0;
        }

        public void Push(ICommand command)
        {
            if (index != 0)
            {
                for (int i = index; i > 0; i--)
                {
                    commands.Pop();
                }
            }

            commands.Push(command);
            index = 0;
        }

        public ICommand Peek()
        {
            if (commands.Count > 0 && index >= 0 && index < commands.Count)
            {
                return commands.ElementAt(index);
            }
            return null;
        }

        public void StepForward()
        {
            if (index - 1 >= -1)
            {
                index--;
            }
        }

        public void StepBackward()
        {
            if (index + 1 < commands.Count+1)
            {
                index++;
            }
        }

        public bool IsEmpty()
        {
            return commands.Count == 0;
        }

        public bool IsNotEmpty()
        {
            return !IsEmpty();
        }

        public bool IsFirstCommand()
        {
            return index == 0;
        }

        public bool IsLastCommand()
        {
            return index == commands.Count - 1;
        }

        public ICommand GetPreviousCommand()
        {
            if (commands.Count > 0 && index >= 0 && index < commands.Count)
            {
                return commands.Peek();
            }
            return null;
        }
    }
}
