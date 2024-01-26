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
        DESELECT,
        MOVE,
    }

    public class ActionHistoryController
    {
        private static ActionHistoryController instance;
        private List<Tuple<SceneAction, object[]>> actions;
        private int currentPosition;
        public Moving MovingInstance { get; set; }
        
        public class Moving
        {
            public Point3D StartPoint { get; set; }
            public Point3D EndPoint { get; set; }

            public Vector GetMovingVector()
            {
                return EndPoint - StartPoint;
            }
        }

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
            MovingInstance = new Moving();
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
                else if (oppositeAction == SceneAction.MOVE && objects.Length == 2)
                {
                    Vector oppositeVector = (Vector)objects[1] * (-1);
                    ((ISceneObject)objects[0]).Move(oppositeVector);
                    MovingInstance = new Moving();
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
                case SceneAction.MOVE: return SceneAction.MOVE;
            }

            return SceneAction.SELECT;
        }
    }
}
