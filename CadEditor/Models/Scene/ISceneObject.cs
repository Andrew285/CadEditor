
using CadEditor.Models.Scene.MeshObjects;

namespace CadEditor
{
    public interface IDrawable
    {
        void Draw();
    }


    public interface ISceneObject: IDrawable
    {
        ISceneObject ParentObject { get; set; }
        bool IsSelected { get; set; }

        Point3D GetCenterPoint();
        void Move(Vector v);
        void Select();
        void Deselect();
        ISceneObject Clone();
        (ISceneObject, double) CheckSelected(int x, int y);
        bool IsEqual(ISceneObject obj);
    }
}
