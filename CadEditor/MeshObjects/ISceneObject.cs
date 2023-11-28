
namespace CadEditor
{
    public interface ISceneObject
    {
        ISceneObject ParentObject { get; set; }
        bool IsSelected { get; set; }

        Point3D GetCenterPoint();
        void Move(Vector v);
        void Draw();
        void Select();
        void Deselect();
        ISceneObject CheckSelected(Ray ray);
    }
}
