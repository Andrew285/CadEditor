using SharpGL;
using SharpGL.SceneGraph.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


		public double? RayIntersectsPlane(Facet facet, Ray lineRay)
		{

			Vector intersectionPoint;

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
			double distance = ((planeNormal - origin) * planeNormal) / dotProduct;
			//double distance2 = (planeNormal - origin) / direction;


			//Check if distance is the minimum
			if (distance != 0.0)
			{
				if (distance < nearDistance)
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
			lineRay.Direction = intersectionPoint;

			//Check if the intersection point is in the current facet
			if(!facet.Contains(new Vertex(intersectionPoint)))
			{
				return null;
			}

			//Console.WriteLine("\nIntersection Point: {0}\nFacet: {1}\nDistance: {2}", intersectionPoint, facet, distance);
			return distance;
		}

	}
}
