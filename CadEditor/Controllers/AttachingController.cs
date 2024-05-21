using CadEditor.MeshObjects;
using SharpGL.SceneGraph;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadEditor
{
    public class AttachingController
    {
        private MeshObject3D[] attachingObjects;
        private Plane[] attachingFacets;
        public Color AttachingObjectColor { get; set; } = Color.Green;
        public Color TargetObjectColor { get; set; } = Color.Blue;

        public AttachingController()
        {
            attachingObjects = new MeshObject3D[2];
            attachingFacets = new Plane[2];
        }

        public void DoAttach(ISceneObject obj)
        {
            attachingObjects[0] = (MeshObject3D)obj;
            attachingObjects[0].EdgeSelectedColor = AttachingObjectColor;
            attachingObjects[0].EdgeNonSelectedColor = AttachingObjectColor;
        }

        public void DoDetach()
        {
            attachingObjects[0].SetDefaultColors();
            attachingObjects[0] = null;
        }

        public void DoSetTarget(ISceneObject obj)
        {
            attachingObjects[1] = (MeshObject3D)obj;
            attachingObjects[1].EdgeSelectedColor = TargetObjectColor;
            attachingObjects[1].EdgeNonSelectedColor = TargetObjectColor;
        }

        public void DoNotSetTarget()
        {
            attachingObjects[1].SetDefaultColors();
            attachingObjects[1] = null;
        }

        public bool IsAttaching(ISceneObject obj)
        {
            return attachingObjects[0] == obj;
        }

        public bool IsTarget(ISceneObject obj)
        {
            return attachingObjects[1] == obj;
        }

        public bool IsEmpty()
        {
            return attachingObjects[0] == null && attachingObjects[1] == null;
        }

        public MeshObject3D GetAttachingObject()
        {
            return attachingObjects[0];
        }

        public MeshObject3D GetTargetObject()
        {
            return attachingObjects[1];
        }

        public void Clear()
        {
            DoDetach();
            DoNotSetTarget();
        }

        public void AddAttachingFacet(Plane facet)
        {
            attachingFacets[0] = facet;
        }

        public void AddTargetFacet(Plane facet)
        {
            attachingFacets[1] = facet;
        }
        public void AddAttachingCube(MeshObject3D cube)
        {
            attachingObjects[0] = cube;
        }

        public void AddTargetCube(MeshObject3D cube)
        {
            attachingObjects[1] = cube;
        }

        public Plane GetAttachingFacet()
        {
            return attachingFacets[0];
        }

        public Plane GetTargetFacet()
        {
            return attachingFacets[1];
        }

        public bool IsFacetsInitialized()
        {
            return attachingFacets[0] != null && attachingFacets[1] != null;
        }

        public (int, Vector) GetClosestDistanceToAttach()
        {
            double minDistance = 0;
            Vector minVector = null;
            int indexOfMinPoint = -1;
            for (int i = 0; i < GetTargetFacet().Points.Count; i++)
            {
                Point3D p = GetTargetFacet().Points[i];
                Vector distanceVector = GetAttachingFacet().GetCenterPoint() - p;
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

        public void AttachFacets()
        {
            Mesh targetMesh = GetTargetObject().Mesh;
            Mesh attachingMesh = GetAttachingObject().Mesh;

            for (int i = 0; i < GetAttachingFacet().Points.Count; i++)
            {
                int index1 = targetMesh.GetIndexOfPoint(GetTargetFacet()[i]);
                Point3D targetPoint = targetMesh.Vertices[index1];

                int index2 = attachingMesh.GetIndexOfPoint(GetAttachingFacet()[i]);
                Point3D attachingPoint = attachingMesh.Vertices[index2];

                Vector v = attachingPoint - targetPoint;
                attachingPoint.Move(v * (-1));
                int coef = targetMesh.Vertices[index1].Coefficient;
                //targetMesh.Vertices[index1] = attachingPoint;
                //targetMesh.Vertices[index1].Coefficient += coef;

                attachingMesh.Vertices[index2] = targetMesh.Vertices[index1];
                attachingMesh.Vertices[index2].Coefficient += 1;
            }
        }

        public void UpdateObjects()
        {
            ((ComplexCube)GetTargetObject()).UpdateObject();
            ((ComplexCube)GetAttachingObject()).UpdateObject();

            int targetFacetIndex = GetTargetObject().Mesh.GetIndexOfFacet(GetTargetFacet());
            GetTargetObject().Mesh.attachedFacets.Add(targetFacetIndex);

            int attachingFacetIndex = GetAttachingObject().Mesh.GetIndexOfFacet(GetAttachingFacet());
            GetAttachingObject().Mesh.attachedFacets.Add(attachingFacetIndex);
        }

        public void SetAttachingObjectToAxis(Axis axis)
        {
            Point3D pointToMove = axis.P2;
            MeshObject3D attachingObject = GetAttachingObject();

            //Create AttachingFacetsPair
            foreach (Plane facet in GetTargetObject().Mesh.Facets)
            {
                if (facet.GetCenterPoint() == axis.P1)
                {
                    facet.IsAttached = true;
                    AddTargetFacet(facet);
                    break;
                }
            }

            CoordinateAxisType oppositeType = AxisSystem.GetOppositeAxisType(GetTargetFacet().AxisType);
            foreach (Plane facet in GetAttachingObject().Mesh.Facets)
            {
                if (facet.AxisType == oppositeType)
                {
                    facet.IsAttached = true;
                    AddAttachingFacet(facet);
                    break;
                }
            }

            Vector distanceVector = attachingObject.GetCenterPoint() - pointToMove;
            attachingObject.Move(distanceVector * (-1));
        }

        public ComplexStructure AttachCubes()
        {
            if (IsFacetsInitialized())
            {
                Plane targetFacet = GetTargetFacet();
                Plane attachingFacet = GetAttachingFacet();

                ComplexCube targetCube = ((ComplexCube)GetTargetObject());
                ComplexCube attachingCube = ((ComplexCube)GetAttachingObject());

                //find closest point to attaching cube
                (int, Vector) closestDistance = GetClosestDistanceToAttach();
                int indexOfMinPoint = closestDistance.Item1;
                Vector minVector = closestDistance.Item2;

                //move to target cube
                Vector pointToPoint = attachingFacet.Points[indexOfMinPoint] - targetFacet.Points[indexOfMinPoint];
                Vector centerToPoint = minVector - pointToPoint;
                Point3D resultCenterPoint = new Point3D(targetFacet.Points[indexOfMinPoint] + new Point3D(centerToPoint));
                Vector resultVector = attachingFacet.GetCenterPoint() - resultCenterPoint;
                GetAttachingObject().Move(resultVector * (-1));

                //attach facet
                AttachFacets();
                UpdateObjects();
                //Clear();

                //Creating complex structure or adding cube to it
                ComplexStructure structure = ComplexStructureController.GetInstance().AddCubes(attachingCube, targetCube);
                structure.AttachingDetailsList.Add(new ComplexStructure.AttachingDetails(

                    targetCube,
                    targetFacet,
                    attachingCube,
                    attachingFacet
                ));

                for (int j = 0; j < targetFacet.Points.Count; j++)
                {
                    //int posInCube = attachingFacet[j].PositionInCube;
                    for (int k = 0; k < attachingCube.OuterVertices.Length; k++)
                    {
                        if (attachingCube.OuterVertices[k] == attachingFacet[j])
                        {
                            attachingCube.OuterVertices[k] = targetFacet[j];
                        }
                    }
                }

                return structure;
            }

            return null;
        }
    }
}
