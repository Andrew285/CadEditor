using CadEditor.Controllers;
using CadEditor.MeshObjects;
using CadEditor.Models.Scene;
using CadEditor.Models.Scene.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;


namespace CadEditor
{
    public class ComplexStructure: IDivideable, IExportable, IUniqueable, IRotateable
    {
        private Dictionary<CellPosition, ComplexCube> cells;
        private Dictionary<(ComplexCube, ComplexCube), (Plane, Plane)> connections;
        public bool DrawFacets = false;

        public string Name { get; set; }
        public ISceneObject ParentObject { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDivided { get; set; }
        public float xRotation { get; set; } = 0.0f;
        public float yRotation { get; set; } = 0.0f;

        public struct CellPosition 
        {
            public int X, Y, Z;

            public CellPosition(CellPosition pos)
            {
                X = pos.X;
                Y = pos.Y;
                Z = pos.Z;
            }

            public CellPosition(int x, int y, int z)
            {
                X = x; Y = y; Z = z;
            }

            public CellPosition Clone()
            {
                return new CellPosition(this);
            }
        }

        public ComplexStructure()
        {
            //cubes = new List<ComplexCube>();
            //AttachingDetailsList = new List<AttachingDetails>();
            ParentObject = null;
            IsSelected = false;
            Name = ModelNameProvider.GetInstance().GetNextName(ModelTypes.COMPLEX_STRUCTURE);
            cells = new Dictionary<CellPosition, ComplexCube>();
            connections = new Dictionary<(ComplexCube, ComplexCube), (Plane, Plane)>();
        }

        public ComplexStructure(ComplexStructure source)
        {
            //List<ICloneable> cubes = new List<ICloneable>(source.cells.Count);

            //source.cells.Values.ForEach((item) =>
            //{
            //    cubes.Add((ICloneable)item.Clone());
            //});

            //ParentObject = source.ParentObject.Clone();
            //IsSelected = source.IsSelected;
            //Name = source.Name;

            //TODO: AttachingDetailsList doesn't clone
        }

        public ComplexStructure(ComplexCube target, CoordinateAxisType targetAxis,
                                ComplexCube attaching, CoordinateAxisType attachingAxis) : this()
        {
            AttachCubes(target, targetAxis, attaching, attachingAxis);
        }

        public List<ComplexCube> GetCubes()
        {
            return cells.Values.ToList();
        }

        public void AttachStructure(ComplexCube target, CoordinateAxisType targetAxis,
                                ComplexCube attaching, CoordinateAxisType attachingAxis, ComplexStructure attachingStructure)
        {
            CellPosition? targetPosition = GetCellPositionByCube(target);
            if (targetPosition == null)
            {
                targetPosition = new CellPosition(0, 0, 0);
                cells[(CellPosition)targetPosition] = target;
            }

            CellPosition attachingPosition = new CellPosition((CellPosition)targetPosition);
            switch (targetAxis)
            {
                case CoordinateAxisType.PlusZ: attachingPosition.Z += 1; break;
                case CoordinateAxisType.MinusZ: attachingPosition.Z -= 1; break;
                case CoordinateAxisType.PlusY: attachingPosition.Y += 1; break;
                case CoordinateAxisType.MinusY: attachingPosition.Y -= 1; break;
                case CoordinateAxisType.PlusX: attachingPosition.X += 1; break;
                case CoordinateAxisType.MinusX: attachingPosition.X -= 1; break;
            }

            CellPosition? realAttachPos = attachingStructure.GetCellPositionByCube(attaching);
            if (realAttachPos != null)
            {
                int newX = attachingPosition.X - ((CellPosition)realAttachPos).X;
                int newY = attachingPosition.Y - ((CellPosition)realAttachPos).Y;
                int newZ = attachingPosition.Z - ((CellPosition)realAttachPos).Z;
                CellPosition difference = new CellPosition(newX, newY, newZ);

                foreach (var attachingCell in attachingStructure.GetCells())
                {
                    int X = attachingCell.Key.X + difference.X;
                    int Y = attachingCell.Key.Y + difference.Y;
                    int Z = attachingCell.Key.Z + difference.Z;
                    CellPosition realPos = new CellPosition(X, Y, Z);

                    //Get all neighbours of attachingPosition to find all connections
                    Dictionary<CellPosition, ComplexCube> neighbourCells = GetClosestCubesToPosition(realPos);

                    foreach (var cell in neighbourCells)
                    {
                        (CoordinateAxisType, CoordinateAxisType) axisTypes = GetAxesFromCellPositions(cell.Key, realPos);
                        Plane targetFacet = cell.Value.Mesh.GetFacetByCoordinateAxisType(axisTypes.Item1);
                        Plane attachingFacet = attachingCell.Value.Mesh.GetFacetByCoordinateAxisType(axisTypes.Item2);

                        if (targetFacet.IsAttached && attachingFacet.IsAttached)
                        {
                            cells[realPos] = attachingCell.Value;
                            connections[(cell.Value, attachingCell.Value)] = (targetFacet, attachingFacet);
                            continue;
                        }
                        else
                        {
                            Attach(cell.Value, targetFacet, attachingCell.Value, attachingFacet, attachingStructure);
                            cells[realPos] = attachingCell.Value;
                            connections[(cell.Value, attachingCell.Value)] = (targetFacet, attachingFacet);
                        }
                    }
                }
            }
        }

        public void AttachCubes(ComplexCube target, CoordinateAxisType targetAxis,
                                ComplexCube attaching, CoordinateAxisType attachingAxis)
        {
            CellPosition? targetPosition = GetCellPositionByCube(target);
            if (targetPosition == null) 
            {
                targetPosition = new CellPosition(0, 0, 0);
                cells[(CellPosition)targetPosition] = target;
            }

            CellPosition attachingPosition = new CellPosition((CellPosition)targetPosition);
            switch (targetAxis)
            {
                case CoordinateAxisType.PlusZ: attachingPosition.Z += 1; break;
                case CoordinateAxisType.MinusZ: attachingPosition.Z -= 1; break;
                case CoordinateAxisType.PlusY: attachingPosition.Y += 1; break;
                case CoordinateAxisType.MinusY: attachingPosition.Y -= 1; break;
                case CoordinateAxisType.PlusX: attachingPosition.X += 1; break;
                case CoordinateAxisType.MinusX: attachingPosition.X -= 1; break;
            }


            //Get all neighbours of attachingPosition to find all connections
            Dictionary<CellPosition, ComplexCube> neighbourCells = GetClosestCubesToPosition(attachingPosition);

            foreach (var cell in neighbourCells)
            {
                (CoordinateAxisType, CoordinateAxisType) axisTypes = GetAxesFromCellPositions(cell.Key, attachingPosition);
                Plane targetFacet = cell.Value.Mesh.GetFacetByCoordinateAxisType(axisTypes.Item1);
                Plane attachingFacet = attaching.Mesh.GetFacetByCoordinateAxisType(axisTypes.Item2);
                Attach(cell.Value, targetFacet, attaching, attachingFacet, null);
                cells[attachingPosition] = attaching;
                connections[(cell.Value, attaching)] = (targetFacet, attachingFacet);
            }
        }

        private Dictionary<CellPosition, ComplexCube> GetClosestCubesToPosition(CellPosition pos)
        {
            // Define the possible movements in 3D space as direction vectors
            (int, int, int)[] directions = new (int, int, int)[]
            {
            (1, 0, 0),
            (-1, 0, 0),
            (0, 1, 0),
            (0, -1, 0),
            (0, 0, 1),
            (0, 0, -1)
            };

            // Iterate through each direction vector and compute the neighbor's position
            Dictionary<CellPosition, ComplexCube> results = new Dictionary<CellPosition, ComplexCube>();
            int x = pos.X, y = pos.Y, z = pos.Z;
            foreach (var direction in directions)
            {
                int newX = x + direction.Item1;
                int newY = y + direction.Item2;
                int newZ = z + direction.Item3;

                CellPosition newPosition = new CellPosition(newX, newY, newZ);
                if (cells.ContainsKey(newPosition))
                {
                    results.Add(newPosition, cells[newPosition]);
                }
            }
            return results;
        }

        private (CoordinateAxisType, CoordinateAxisType) GetAxesFromCellPositions(CellPosition pos1, CellPosition pos2)
        {
            int resultX = (pos1.X - pos2.X) % 2;
            int resultY = (pos1.Y - pos2.Y) % 2;
            int resultZ = (pos1.Z - pos2.Z) % 2;

            if (resultX == 1) return (CoordinateAxisType.MinusX, CoordinateAxisType.PlusX);
            else if (resultX == -1) return (CoordinateAxisType.PlusX, CoordinateAxisType.MinusX);
            else if (resultY == 1) return (CoordinateAxisType.MinusY, CoordinateAxisType.PlusY);
            else if (resultY == -1) return (CoordinateAxisType.PlusY, CoordinateAxisType.MinusY);
            else if (resultZ == 1) return (CoordinateAxisType.MinusZ, CoordinateAxisType.PlusZ);
            else if (resultZ == -1) return (CoordinateAxisType.PlusZ, CoordinateAxisType.MinusZ);
            else return (CoordinateAxisType.MinusX, CoordinateAxisType.MinusX);
        }

        private void Attach(ComplexCube targetCube, Plane targetFacet, ComplexCube attachingCube, Plane attachingFacet, ComplexStructure attachingStructure)
        {
            //find closest point to attaching cube
            (int, Vector) closestDistance = GetClosestDistanceToAttach(targetFacet, attachingFacet);
            int indexOfMinPoint = closestDistance.Item1;
            Vector minVector = closestDistance.Item2;

            //move to target cube
            Vector pointToPoint = attachingFacet.Points[indexOfMinPoint] - targetFacet.Points[indexOfMinPoint];
            Vector centerToPoint = minVector - pointToPoint;
            Point3D resultCenterPoint = new Point3D(targetFacet.Points[indexOfMinPoint] + new Point3D(centerToPoint));
            Vector resultVector = attachingFacet.GetCenterPoint() - resultCenterPoint;
            if (attachingStructure != null)
            {
                attachingStructure.Move(resultVector * (-1));
            }
            else
            {
                attachingCube.Move(resultVector * (-1));
            }

            //attach facet
            AttachFacets(targetCube, targetFacet, attachingCube, attachingFacet);
            UpdateObjects(targetCube, targetFacet, attachingCube, attachingFacet);

            for (int j = 0; j < targetFacet.Points.Count; j++)
            {
                for (int k = 0; k < attachingCube.OuterVertices.Length; k++)
                {
                    if (attachingCube.OuterVertices[k] == attachingFacet[j])
                    {
                        attachingCube.OuterVertices[k] = targetFacet[j];
                    }
                }
            }
        }

        private void UpdateObjects(ComplexCube targetCube, Plane targetFacet, ComplexCube attachingCube, Plane attachingFacet)
        {
            targetCube.UpdateObject();
            attachingCube.UpdateObject();

            int targetFacetIndex = targetCube.Mesh.GetIndexOfFacet(targetFacet);
            targetCube.Mesh.attachedFacets.Add(targetFacetIndex);

            int attachingFacetIndex = attachingCube.Mesh.GetIndexOfFacet(attachingFacet);
            attachingCube.Mesh.attachedFacets.Add(attachingFacetIndex);
        }

        private void AttachFacets(ComplexCube targetCube, Plane targetFacet, ComplexCube attachingCube, Plane attachingFacet)
        {
            Mesh targetMesh = targetCube.Mesh;
            Mesh attachingMesh = attachingCube.Mesh;

            for (int i = 0; i < targetFacet.Points.Count; i++)
            {
                int index1 = targetMesh.GetIndexOfPoint(targetFacet[i]);
                Point3D targetPoint = targetMesh.Vertices[index1];

                int index2 = attachingMesh.GetIndexOfPoint(attachingFacet[i]);
                Point3D attachingPoint = attachingMesh.Vertices[index2];

                Vector v = attachingPoint - targetPoint;
                attachingPoint.Move(v * (-1));
                int coef = targetMesh.Vertices[index1].Coefficient;
                attachingMesh.Vertices[index2] = targetMesh.Vertices[index1];
                attachingMesh.Vertices[index2].Coefficient += 1;

                targetFacet.IsAttached = true;
                attachingFacet.IsAttached = true;
            }
        }

        private (int, Vector) GetClosestDistanceToAttach(Plane targetFacet, Plane attachingFacet)
        {
            double minDistance = 0;
            Vector minVector = null;
            int indexOfMinPoint = -1;
            for (int i = 0; i < targetFacet.Points.Count; i++)
            {
                Point3D p = targetFacet.Points[i];
                Vector distanceVector = attachingFacet.GetCenterPoint() - p;
                double distance = distanceVector.Length();

                if (minDistance == 0 || distance < minDistance)
                {
                    minDistance = distance;
                    minVector = distanceVector;
                    indexOfMinPoint = i;
                }
            }

            return (indexOfMinPoint, minVector);
        }

        public Dictionary<CellPosition, ComplexCube> GetCells()
        {
            return cells;
        }

        private CellPosition? GetCellPositionByCube(ComplexCube cube)
        {
            if (cube != null)
            {
                foreach (var item in cells)
                {
                    if (item.Value == cube)
                    {
                        return item.Key;
                    }
                }
            }

            return null;
        }

        public void AddCell(CellPosition position, ComplexCube cube)
        {
            cells.Add(position, cube);
        }

        public void AddConnection()
        {

        }

        public bool Contains(ComplexCube cube)
        {
            return cells.Values.Contains(cube);
        }

        public Point3D GetCenterPoint()
        {
            double x = 0, y = 0, z = 1;

            foreach (ComplexCube cube in cells.Values)
            {
                Point3D centerPoint = cube.GetCenterPoint();
                x += centerPoint.X;
                y += centerPoint.Y;
                z += centerPoint.Z;
            }

            return new Point3D(x/cells.Count, y/ cells.Count, z/ cells.Count);
        }

        public void Move(Vector v)
        {
            foreach (ComplexCube cube in cells.Values)
            {
                cube.Move(v);
            }
            GraphicsGL.Control.Invalidate();
        }

        public void Select()
        {
            foreach (ComplexCube cube in cells.Values)
            {
                cube.Select();
            }
        }

        public void Deselect()
        {
            foreach (ComplexCube cube in cells.Values)
            {
                cube.Deselect();
            }
        }

        public (ISceneObject, double) CheckSelected(int x, int y)
        {
            ISceneObject resObject = null;
            double minDistance = 0;

            foreach (ComplexCube cube in cells.Values)
            {
                (ISceneObject, double) sceneObject = cube.CheckSelected(x, y);
                
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
            foreach (ComplexCube cube in cells.Values)
            {
                cube.DrawFacets = this.DrawFacets;
                cube.Draw();
            }
        }

        public ISceneObject Clone()
        {
            return new ComplexStructure(this);
        }

        public void Divide(Vector v)
        {
            foreach (ComplexCube cube in cells.Values)
            {
                cube.Divide(v);
            }
        }

        public void Divide(ComplexCube target, Vector v)
        {
            CellPosition targetPosition = new CellPosition();
            foreach (var cell in cells)
            {
                if (cell.Value.Name == target.Name)
                {
                    targetPosition = cell.Key;
                }
            }

            Vector resultVector;
            foreach (var cell in cells)
            {
                int x = Math.Abs(targetPosition.X - cell.Key.X);
                int y = Math.Abs(targetPosition.Y - cell.Key.Y);
                int z = Math.Abs(targetPosition.Z - cell.Key.Z);

                int resX = x > 0 ? (int)v[0] : 1;
                int resY = y > 0 ? (int)v[1] : 1;
                int resZ = z > 0 ? (int)v[2] : 1;
                if (resX == 1 && resY == 1 && resZ == 1)
                {
                    resultVector = new Vector(1, 1, 1);
                    continue;
                }
                else
                {
                    resultVector = new Vector(resX, resY, resZ);
                }
                cell.Value.Divide(resultVector);
            }
        }

        public void Unite()
        {
            foreach (ComplexCube cube in cells.Values)
            {
                cube.Unite();
            }
        }

        public bool IsEqual(ISceneObject obj)
        {
            if (obj != null && obj is ComplexStructure)
            {
                ComplexStructure complexStructure = (ComplexStructure)obj;

                for (int i = 0; i < complexStructure.cells.Count; i++)
                {
                    if (!this.cells.ElementAt(i).Value.IsEqual(complexStructure.cells.ElementAt(i).Value))
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
            foreach (ISceneObject obj in cells.Values)
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
            CellPosition cubePosition = ((CellPosition)GetCellPositionByCube(cube)).Clone();
            switch (axis)
            {
                case CoordinateAxisType.PlusZ: cubePosition.Z += 1; break;
                case CoordinateAxisType.MinusZ: cubePosition.Z -= 1; break;
                case CoordinateAxisType.PlusY: cubePosition.Y += 1; break;
                case CoordinateAxisType.MinusY: cubePosition.Y -= 1; break;
                case CoordinateAxisType.PlusX: cubePosition.X += 1; break;
                case CoordinateAxisType.MinusX: cubePosition.X -= 1; break;
            }

            ComplexCube resultCube;
            cells.TryGetValue(cubePosition, out resultCube);
            return resultCube;
        }

        public string Export()
        {
            string stringToExport = "";

            stringToExport += ModelNameProvider.GetInstance().GetNextName(ModelTypes.COMPLEX_STRUCTURE) + "\n";
            foreach (ComplexCube cube in cells.Values)
            {
                stringToExport += cube.Export();
            }
            stringToExport += "\n";

            StringBuilder cellStringBuilder = new StringBuilder();
            foreach (var cell in cells)
            {
                CellPosition pos = cell.Key;
                ComplexCube cube = cell.Value;
                cellStringBuilder.Append($"Cell {pos.X} {pos.Y} {pos.Z} {cube.Name}\n");
            }
            stringToExport += cellStringBuilder.ToString() + "\n";

            StringBuilder connectionsStringBuilder = new StringBuilder();
            foreach (var connection in connections)
            {
                connectionsStringBuilder.Append($"Connection " +
                                                $"{connection.Key.Item1.Name} {connection.Key.Item2.Name} " +
                                                $"{connection.Value.Item1.AxisType} {connection.Value.Item2.AxisType}\n");
            }
            stringToExport += connectionsStringBuilder.ToString();
            stringToExport += "End of Structure\n";
            return stringToExport;
        }
    }
}
