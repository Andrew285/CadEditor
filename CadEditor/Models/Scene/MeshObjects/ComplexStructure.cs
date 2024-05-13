using CadEditor.Controllers;
using CadEditor.MeshObjects;
using CadEditor.Models.Scene;
using CadEditor.Models.Scene.MeshObjects;
using System;
using System.Collections.Generic;


namespace CadEditor
{
    public class ComplexStructure: IDivideable, IExportable, IUniqueable, IRotateable
    {
        private List<ComplexCube> cubes;
        public List<AttachingDetails> AttachingDetailsList { get; set; }
        public string Name { get; set; }

        public ISceneObject ParentObject { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDivided { get; set; }

        public float xRotation { get; set; } = 0.0f;
        public float yRotation { get; set; } = 0.0f;

        public class AttachingDetails : IExportable
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

            public ComplexCube Contains(ComplexCube cube)
            {
                if (cube == attachingCube) return attachingCube;
                else if (cube == targetCube) return targetCube;
                else return null;
            }

            public ComplexCube GetAttachedTo(ComplexCube cube)
            {
                if (cube == attachingCube) return targetCube;
                else if (cube == targetCube) return attachingCube;
                else return null;
            }

            public Plane GetAttachingFacet(ComplexCube cube)
            {
                if (cube == attachingCube) return attachingFacet;
                else if (cube == targetCube) return targetFacet;
                else return null;
            }

            public string Export()
            {
                return targetCube.Name + " " + targetCube.Mesh.GetIndexOfFacet(targetFacet) + " " +
                       attachingCube.Name + " " + attachingCube.Mesh.GetIndexOfFacet(targetFacet) + "\n";
            }
        }

        public ComplexStructure()
        {
            cubes = new List<ComplexCube>();
            AttachingDetailsList = new List<AttachingDetails>();
            ParentObject = null;
            IsSelected = false;
            Name = NameController.GetNextStructureName();
        }

        public ComplexStructure(ComplexStructure source)
        {
            List<ICloneable> cubes = new List<ICloneable>(source.cubes.Count);

            source.cubes.ForEach((item) =>
            {
                cubes.Add((ICloneable)item.Clone());
            });

            ParentObject = source.ParentObject.Clone();
            IsSelected = source.IsSelected;
            Name = source.Name;

            //TODO: AttachingDetailsList doesn't clone
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
            double x = 0, y = 0, z = 1;

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
            GraphicsGL.Control.Invalidate();
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

        public (ISceneObject, double) CheckSelected()
        {
            ISceneObject resObject = null;
            double minDistance = 0;

            foreach (ComplexCube cube in cubes)
            {
                (ISceneObject, double) sceneObject = cube.CheckSelected();
                
                if (sceneObject.Item1 != null)
                {
                    if (resObject == null && minDistance == 0)
                    {
                        resObject = sceneObject.Item1;
                        minDistance = sceneObject.Item2;
                    }
                    else
                    {
                        if (sceneObject.Item2 < minDistance)
                        {
                            minDistance = sceneObject.Item2;
                            resObject = sceneObject.Item1;
                        }
                    }
                }
            }

            return (resObject, minDistance);
        }

        public void Draw()
        {
            foreach (ComplexCube cube in cubes)
            {
                cube.Draw();
            }
        }

        public ISceneObject Clone()
        {
            //ComplexStructure cloneStructure = new ComplexStructure();

            //foreach (ComplexCube cube in cubes)
            //{
            //    cube.Clone();
            //    cloneStructure.AddCube(cube);
            //}

            //return cloneStructure;

            return new ComplexStructure(this);
        }

        public void Divide(Vector v)
        {
            foreach (ComplexCube cube in cubes)
            {
                cube.Divide(v);
            }
        }

        public void Unite()
        {
            foreach (ComplexCube cube in cubes)
            {
                cube.Unite();
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

        public List<ComplexCube> GetAttachedCubesByAxis(ComplexCube cube, CoordinateAxis axis)
        {
            (CoordinateAxisType, CoordinateAxisType) axisTypes = AxisSystem.GetAxisTypesFromAxis(axis);
            List<ComplexCube> resultCubes = new List<ComplexCube>();
            ComplexCube currentCube = cube;
            CoordinateAxisType currentAxis = axisTypes.Item1;
            bool isAnyAttached = true;
            bool allTypesPassed = false;

            while (isAnyAttached)
            {
                ComplexCube attachedToCurrent = GetAttachedCubeByAxis(currentCube, currentAxis);
                if (attachedToCurrent != null)
                {
                    resultCubes.Add(attachedToCurrent);
                    currentCube = attachedToCurrent;
                }
                else
                {
                    if (!allTypesPassed)
                    {
                        currentAxis = axisTypes.Item2;
                        allTypesPassed = !allTypesPassed;
                        currentCube = cube;
                    }
                    else
                    {
                        allTypesPassed = !allTypesPassed;
                        isAnyAttached = !isAnyAttached;
                    }
                }
            }
            return resultCubes;
        }

        public ComplexCube GetAttachedCubeByAxis(ComplexCube cube, CoordinateAxisType axis)
        {
            for (int i = 0; i < AttachingDetailsList.Count; i++)
            {
                AttachingDetails details = AttachingDetailsList[i];
                ComplexCube attachedCube = details.Contains(cube);
                Plane attachingFacet = details.GetAttachingFacet(cube);
                if (attachedCube != null && attachingFacet != null && attachingFacet.AxisType == axis)
                {
                    return details.GetAttachedTo(attachedCube);
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
