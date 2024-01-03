
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
        object Clone();
        ISceneObject CheckSelected();
    }
}
