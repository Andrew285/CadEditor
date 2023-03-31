using SharpGL;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Raytracing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadEditor
{
	public class Ray
	{
		private Vector origin;
		private Vector direction;
		private OpenGL gl;
		private double nearDistance;

		public Ray(OpenGL _gl)
		{
			gl = _gl;
		}
		public Ray(Vector start, Vector end, OpenGL _gl)
		{
			origin = start;
			direction = end;
			gl = _gl;
		}

		public Vector Origin { get { return origin; } set { origin = value; } }
		public Vector Direction { get { return direction; } set { direction = value; } }


		public double? RayIntersectsFacet(Facet facet, Ray lineRay)
		{

			Vector intersectionPoint;
			Vector intersectionPoint2;
			Vertex facetCenterPoint = facet.GetCenterPoint();

			//Calculate planeNormal
			Vector planeNormal = facet.CalculateNormal();

			// Calculate the dot product of the ray direction and the plane normal
			double dotProduct = direction * planeNormal;

			// Check if the ray and plane are parallel
			if (Math.Abs(dotProduct) < double.Epsilon)
			{
				return null;
			}

			// Calculate the distance from the ray origin to the plane
			double distance2 = ((planeNormal - origin) * planeNormal) / dotProduct;
			double distance = ((new Vector(facetCenterPoint) - origin) * planeNormal) / dotProduct;
			//double distance2 = (planeNormal - origin) / direction;


			//Check if distance is the minimum
			if (distance != 0.0)
			{
				if (distance < nearDistance)
				{
					nearDistance = distance;
				}
				else
				{
					nearDistance = distance;
				}
			}
			else
			{
				nearDistance = distance;
			}

			// Check if the intersection point is behind the ray origin
			if (distance < 0)
			{
				return null;
			}

			// Calculate the intersection point
			intersectionPoint = origin + distance * direction;
			intersectionPoint2 = origin + distance2 * direction;
			lineRay.Direction = intersectionPoint;

			//Check if the intersection point is in the current facet
			if (!facet.Contains(new Vertex(intersectionPoint)))
			{
				return null;
			}

			Console.WriteLine("\n\n-----------------\nIntersection Point: {0}\nFacet: {1}\nNormal: {2}", intersectionPoint, facet, planeNormal);
			return distance;
		}

		public double? RayIntersectsEdge(Edge edge, out Vertex intersectionPoint)
		{
			intersectionPoint = null;
			// Calculate the direction vector of the line
			Vector lineDirection = new Vector(edge.V2) - new Vector(edge.V1);

			// Calculate the normal of the plane that contains the line and the ray
			Vector planeNormal = lineDirection.Cross(Direction);

			// Calculate the distance between the line and the ray
			double denominator = planeNormal * planeNormal;
			double numerator = planeNormal * (Origin - new Vector(edge.V1));
			double distance = -numerator / denominator;

			// Check if the intersection point lies on the line segment
			float epsilon = 0.0001f;
			if (distance < -epsilon || distance > 1 + epsilon)
			{
				return null;
			}

			// Calculate the intersection point
			intersectionPoint = new Vertex(new Vector(edge.V1) + lineDirection * distance);

			if (!edge.Contains(intersectionPoint))
			{
				return null;
			}

			return distance;
		}

		public double? RayIntersectsVertex(Vertex vertex)
		{
			// Calculate the components of the direction vector.
			double dx = Direction[0];
			double dy = Direction[1];
			double dz = Direction[2];

			// Calculate the parameter values for each component.
			double tx = (vertex.X - Origin[0]) / dx;
			double ty = (vertex.Y - Origin[1]) / dy;
			double tz = (vertex.Z - Origin[2]) / dz;

			// Check if all the parameter values are equal (within some tolerance).
			double accuracy = 0.1;
			if (Math.Abs(tx - ty) < accuracy && Math.Abs(ty - tz) < accuracy)
			{
				// The point lies on the ray.
				return Math.Sqrt(Math.Pow((vertex.X - Origin[0]), 2) + Math.Pow((vertex.X - Origin[1]), 2) + Math.Pow((vertex.X - Origin[2]), 2));
			}
			else
			{
				// The point does not lie on the ray.
				return null;
			}
		}
	}
}
