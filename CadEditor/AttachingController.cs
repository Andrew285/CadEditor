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
                targetMesh.Vertices[index1] = attachingPoint;
                targetMesh.Vertices[index1].Coefficient += 1;
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
    }
}
