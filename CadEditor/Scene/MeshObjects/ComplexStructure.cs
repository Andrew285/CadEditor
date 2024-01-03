using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public class ComplexStructure: ISceneObject
    {
        private List<ComplexCube> cubes;

        public ISceneObject ParentObject { get; set; }
        public bool IsSelected { get; set; }

        public ComplexStructure()
        {
            cubes = new List<ComplexCube>();
            ParentObject = null;
            IsSelected = false;
        }

        public void AddCube(ComplexCube cube)
        {
            cubes.Add(cube);
        }

        public void RemoveCube(ComplexCube cube)
        {
            cubes.Remove(cube);
        }

        public bool Contains(ComplexCube cube)
        {
            return cubes.Contains(cube);
        }

        public Point3D GetCenterPoint()
        {
            double x = 0, y = 0, z = 0;

            foreach (ComplexCube cube in cubes)
            {
                Point3D centerPoint = cube.GetCenterPoint();
                x += centerPoint.X;
                y += centerPoint.Y;
                z += centerPoint.Z;
            }

            return new Point3D(x/cubes.Count, y/cubes.Count, z/cubes.Count);
        }

        public void Move(Vector v)
        {
            foreach (ComplexCube cube in cubes)
            {
                cube.Move(v);
            }
        }

        public void Select()
        {
            foreach (ComplexCube cube in cubes)
            {
                cube.Select();
            }
        }

        public void Deselect()
        {
            foreach (ComplexCube cube in cubes)
            {
                cube.Deselect();
            }
        }

        public ISceneObject CheckSelected()
        {
            foreach (ComplexCube cube in cubes)
            {
                ISceneObject sceneObject = cube.CheckSelected();
                
                if (sceneObject != null)
                {
                    return sceneObject;
                }
            }

            return null;
        }

        public void Draw()
        {
            foreach (ComplexCube cube in cubes)
            {
                cube.Draw();
            }
        }

        public object Clone()
        {
            ComplexStructure cloneStructure = new ComplexStructure();

            foreach (ComplexCube cube in cubes)
            {
                cube.Clone();
                cloneStructure.AddCube(cube);
            }

            return cloneStructure;
        }
    }
}
