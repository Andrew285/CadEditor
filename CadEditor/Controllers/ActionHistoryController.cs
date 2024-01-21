using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadEditor.Controllers
{
    public enum SceneAction
    {
        SELECT,
        DESELECT
    }

    public class ActionHistoryController
    {
        private static ActionHistoryController instance;
        private List<Tuple<SceneAction, object[]>> actions;
        private int currentPosition;

        public static ActionHistoryController GetInstance()
        {
            if (instance == null)
            {
                instance = new ActionHistoryController();
            }

            return instance;
        }

        public ActionHistoryController()
        {
            actions = new List<Tuple<SceneAction, object[]>>();
            currentPosition = -1;
        }

        public void AddAction(SceneAction action, params object[] objects)
        {
            actions.Add(Tuple.Create(action, objects));
            currentPosition++;
        }

        public void InvokePreviousAction()
        {
            if (actions.Count != 0 && currentPosition > -1)
            {
                //int pos = actions.Count - 1 - currentPosition
                int pos = currentPosition;
                SceneAction action = actions[pos].Item1;
                object[] objects = actions[pos].Item2;

                SceneAction oppositeAction = GetOppositeAction(action);

                if (oppositeAction == SceneAction.DESELECT && objects.Length == 1) 
                {
                    ((ISceneObject)objects[0]).Deselect();
                }
                else if (oppositeAction == SceneAction.SELECT && objects.Length == 1)
                {
                    ((ISceneObject)objects[0]).Select();
                }

                currentPosition--;
            }
        }

        private SceneAction GetOppositeAction(SceneAction action)
        {
            switch(action)
            {
                case SceneAction.SELECT: return SceneAction.DESELECT;
                case SceneAction.DESELECT: return SceneAction.SELECT;
            }

            return SceneAction.SELECT;
        }
    }
}
