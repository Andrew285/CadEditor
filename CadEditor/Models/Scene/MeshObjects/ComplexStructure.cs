using CadEditor.Controllers;
using CadEditor.MeshObjects;
using CadEditor.Models.Scene.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
    public class ComplexStructure: ISceneObject, IDivideable, IExportable, IUniqueable
    {
        private List<ComplexCube> cubes;
        public List<AttachingDetails> AttachingDetailsList { get; set; }
        public string Name { get; set; }

        public class AttachingDetails: IExportable
        {
            public ComplexCube attachingCube;
            public ComplexCube targetCube;

            public Plane attachingFacet;
            public Plane targetFacet;

            public AttachingDetails(ComplexCube tC, Plane tF, ComplexCube aC, Plane aF)
            {
                attachingCube = aC;
                attachingFacet = aF;
                targetCube = tC;
                targetFacet = tF;
            }

            public string Export()
            {
                return targetCube.Name + " " + targetCube.Mesh.GetIndexOfFacet(targetFacet) + " " +
                       attachingCube.Name + " " + attachingCube.Mesh.GetIndexOfFacet(targetFacet) + "\n";
            }
        }

        public ISceneObject ParentObject { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDivided { get; set; }

        public ComplexStructure()
        {
            cubes = new List<ComplexCube>();
            AttachingDetailsList = new List<AttachingDetails>();
            ParentObject = null;
            IsSelected = false;
            Name = NameController.GetNextStructureName();
        }

        public List<ComplexCube> GetCubes()
        {
            return cubes;
        }

        public void AddCube(ComplexCube cube)
        {
            cube.ParentObject = this;
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

        public void Divide(Vector v)
        {
            foreach (ComplexCube cube in cubes)
            {
                cube.Divide(v);
            }
        }

        public bool IsEqual(ISceneObject obj)
        {
            if (obj != null && obj is ComplexStructure)
            {
                ComplexStructure complexStructure = (ComplexStructure)obj;

                for (int i = 0; i < complexStructure.cubes.Count; i++)
                {
                    if (!this.cubes[i].IsEqual(complexStructure.cubes[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public ISceneObject GetObjectByName(string name)
        {
            foreach (ISceneObject obj in cubes)
            {
                if (((MeshObject3D)obj).Name == name)
                {
                    return obj;
                }
            }

            return null;
        }

        public string Export()
        {
            string stringToExport = "";

            stringToExport += NameController.GetNextStructureName() + "\n";
            foreach (ComplexCube cube in cubes)
            {
                stringToExport += cube.Export();
            }

            //Export Attaching Details
            foreach (AttachingDetails details in AttachingDetailsList)
            {
                stringToExport += "Attaching ";
                stringToExport += details.Export();
            }

            stringToExport += "End of Structure\n";

            return stringToExport;
        }
    }
}
