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
		private bool selectedObject;
		private double nearDistance;

		public Ray(OpenGL _gl)
		{
			gl = _gl;
			selectedObject = false;
		}

		public Ray(Vector start, Vector end, OpenGL _gl)
		{
			origin = start;
			direction = end;
			gl = _gl;
			selectedObject = false;
		}

		public Vector GetStartPosition() { return origin; }
		public Vector GetEndPosition() { return direction; }
		public Vector Origin { get { return origin; } set { origin = value; } }
		public Vector Direction { get { return direction; } set { direction = value; } }

		public void Normalize()
		{
			double length = Math.Sqrt(direction[0] * direction[0] + direction[1] * direction[1] + direction[2] * direction[2]);
			direction[0] /= length;
			direction[1] /= length;
			direction[2] /= length;
		}

		//public bool IntersectsCube(CustomCube cube)
		//{
		//	List<Vector> cubeVertices = cube.CreateListOfVertices();

		//	for (int i = 0; i < cubeVertices.Count; i += 4)
		//	{
		//		Vector v0 = cubeVertices[i];
		//		Vector v1 = cubeVertices[i + 1];
		//		Vector v2 = cubeVertices[i + 2];
		//		Vector v3 = cubeVertices[i + 3];

		//		// Calculate the normal vector of the face
		//		Vector faceNormal = Vector.Cross(v1 - v0, v2 - v0);

		//		// Check if the ray is parallel to the plane
		//		double dot = Vector.Dot(faceNormal, direction);
		//		if (dot == 0)
		//			continue;

		//		// Calculate the point of intersection between the ray and the plane
		//		double t = Vector.Dot(v0 - origin, faceNormal) / dot;
		//		Vector intersectionPoint = origin + t * direction;

		//		// Check if the intersection point is inside the boundaries of the face
		//		if (Vector.Dot(Vector.Cross(v1 - v0, intersectionPoint - v0), faceNormal) >= 0 &&
		//			Vector.Dot(Vector.Cross(v2 - v1, intersectionPoint - v1), faceNormal) >= 0 &&
		//			Vector.Dot(Vector.Cross(v3 - v2, intersectionPoint - v2), faceNormal) >= 0 &&
		//			Vector.Dot(Vector.Cross(v0 - v3, intersectionPoint - v3), faceNormal) >= 0)
		//		{
		//			return true;
		//		}
		//	}

		//	return false;
		//}


		//public bool IntersectFacet(Facet facet)
		//{
		//	// Compute the normal vector of the quad
		//	Vector quadNormal = Vector.Cross(facet.Vertices[1] - facet.Vertices[0], quadVertices[2] - quadVertices[0]);

		//	// Compute the dot product of the ray direction vector with the quad normal vector
		//	float dotProduct = Vector.Dot(rayDirection, quadNormal);

		//	if (dotProduct == 0)
		//	{
		//		// Ray is parallel to the quad
		//		return false;
		//	}

		//	// Compute the distance from the ray origin to the plane defined by the quad
		//	float distance = Vector.Dot(quadNormal, (quadVertices[0] - rayOrigin)) / dotProduct;

		//	if (distance < 0)
		//	{
		//		// Ray is pointing away from the quad
		//		return false;
		//	}

		//	// Compute the point of intersection between the ray and the plane defined by the quad
		//	Vector intersectionPoint = rayOrigin + distance * rayDirection;

		//	// Check if the intersection point lies within the bounds of the quad
		//	float u, v, w;
		//	ComputeBarycentricCoordinates(quadVertices, intersectionPoint, out u, out v, out w);

		//	return u >= 0 && u <= 1 && v >= 0 && v <= 1 && w >= 0 && w <= 1;
		//}


		public double? RayIntersectsPlane(Facet facet, Ray lineRay)
		{

			Vector intersectionPoint;
			Vector intersectionPoint2;

			//Calculate planeNormal
			Vector planeNormal = CalculatePlaneNormal(facet);

			// Calculate the dot product of the ray direction and the plane normal
			double dotProduct = direction * planeNormal;

			// Check if the ray and plane are parallel
			if (Math.Abs(dotProduct) < double.Epsilon)
			{
				return null;
			}

			// Calculate the distance from the ray origin to the plane
			//double distance = VectorDotProduct((new Vector(facet.Vertices[0]) - origin), planeNormal) / dotProduct;
			double distance = ((planeNormal - origin) * planeNormal) / dotProduct;
			//double distance = (planeNormal - origin) / direction;
			double distance2 = (planeNormal - origin) / direction;


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
			intersectionPoint2 = origin + distance2 * direction;
			lineRay.Direction = intersectionPoint;

			//Check if intersection point belong to current facet
			//if (!BelongPointToFacet(facet, planeNormal, intersectionPoint))
			//{
			//	return false;
			//}

			//if (!PointFaceWorldCoords(facet, intersectionPoint))
			//{
			//	return false;
			//}

			//if (!IsPointInPolygon4(facet, new Vertex(intersectionPoint), gl))
			//{
			//	return false;
			//}

			//Vertex[] worldVertices =
			//{
			//facet.Vertices[0].GetWorldCoordinates(gl),
			//facet.Vertices[1].GetWorldCoordinates(gl),
			//facet.Vertices[2].GetWorldCoordinates(gl),
			//facet.Vertices[3].GetWorldCoordinates(gl)
			//};

			//if (!IsInPolygon(worldVertices, new Vertex(intersectionPoint).GetWorldCoordinates(gl)))
			//{
			//	return false;
			//}

			if(!IsInPolygon3(facet, new Vertex(intersectionPoint)))
			{
				return null;
			}

			Console.WriteLine("\nIntersection Point: {0}\nFacet: {1}\nDistance: {2}", intersectionPoint, facet, distance);
			return distance;


		}

		public bool BelongPointToFacet(Facet facet, Vector facetNormal, Vector intersectionPoint)
		{
			//float halfSize = 2.0f / 2.0f;
			////double xDiff = Math.Abs(intersectionPoint[0] - (double)(new Vector(facet.Vertices[0])[0]));
			////double yDiff = Math.Abs(intersectionPoint[1] - (double)(new Vector(facet.Vertices[0])[1]));
			////double zDiff = Math.Abs(intersectionPoint[2] - (double)(new Vector(facet.Vertices[0])[2]));

			double projectionX = VectorDotProduct(intersectionPoint - facetNormal, new Vector(new double[] { 1.0f, 0.0f, 0.0f }));
			double projectionY = VectorDotProduct(intersectionPoint - facetNormal, new Vector(new double[] { 0.0f, 1.0f, 0.0f }));
			double projectionZ = VectorDotProduct(intersectionPoint - facetNormal, new Vector(new double[] { 0.0f, 0.0f, 1.0f }));


			double xDiff = Math.Abs(projectionX);
			double yDiff = Math.Abs(projectionY);
			double zDiff = Math.Abs(projectionZ);

			//if (facetNormal == new Vector(new double[] { 1.0f, 0.0f, 0.0f }))
			//{
			//	return yDiff <= halfSize && zDiff <= halfSize;
			//}
			//else if (facetNormal == new Vector(new double[] { 0.0f, 1.0f, 0.0f }))
			//{
			//	return xDiff <= halfSize && zDiff <= halfSize;
			//}
			//else if (facetNormal == new Vector(new double[] { 0.0f, 0.0f, 1.0f }))
			//{
			//	return xDiff <= halfSize && yDiff <= halfSize;
			//}

			//return false;


			float halfSize = 2.0f / 2;
			//return Math.Abs(intersectionPoint[0] - (double)facetNormal[0]) <= halfSize &&
			//	   Math.Abs(intersectionPoint[1] - (double)facetNormal[1]) <= halfSize &&
			//	   Math.Abs(intersectionPoint[2] - (double)facetNormal[2]) <= halfSize;

			// Check if the projected coordinates lie within the range of valid values for each axis.
			if (Math.Abs(xDiff) > halfSize || Math.Abs(yDiff) > halfSize || Math.Abs(zDiff) > halfSize)
			{
				// The intersection point is outside the bounds of the face.
				return false;
			}

			return true;
		}

		//public bool PointFaceWorldCoords(Facet facet, Vector intersectionPoint)
		//{
		//	var v1 = facet.Vertices[0].GetWorldCoordinates(gl);
		//	var v2 = facet.Vertices[1].GetWorldCoordinates(gl);
		//	var v3 = facet.Vertices[2].GetWorldCoordinates(gl);
		//	var v4 = facet.Vertices[3].GetWorldCoordinates(gl);

		//	if(p.IsLower(v1))
		//	return true;
		//}

		public static bool IsPointInPolygon4(Facet polygon, Vertex p, OpenGL gl)
		{
			bool result = false;
			int j = polygon.Vertices.Count() - 1;

			var testPoint = new Vertex(gl.UnProject(p.X, p.Y, p.Z));

			for (int i = 0; i < polygon.Vertices.Count(); i++)
			{

				var v = polygon.Vertices[i].GetWorldCoordinates(gl);
				if (v.Y < testPoint.Y && v.Y >= testPoint.Y || v.Y < testPoint.Y && v.Y >= testPoint.Y)
				{
					if (v.X + (testPoint.Y - v.Y) / (v.Y - v.Y) * (v.X - v.X) < testPoint.X)
					{
						result = !result;
					}
				}
				j = i;
			}
			return result;
		}

		public static bool IsInPolygon(Point[] poly, Point p)
		{
			Point p1, p2;
			bool inside = false;

			if (poly.Length < 3)
			{
				return inside;
			}

			var oldPoint = new Point(
				poly[poly.Length - 1].X, poly[poly.Length - 1].Y);

			for (int i = 0; i < poly.Length; i++)
			{
				var newPoint = new Point(poly[i].X, poly[i].Y);

				if (newPoint.X > oldPoint.X)
				{
					p1 = oldPoint;
					p2 = newPoint;
				}
				else
				{
					p1 = newPoint;
					p2 = oldPoint;
				}

				if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
					&& (p.Y - (long)p1.Y) * (p2.X - p1.X)
					< (p2.Y - (long)p1.Y) * (p.X - p1.X))
				{
					inside = !inside;
				}

				oldPoint = newPoint;
			}

			return inside;
		}

		public static bool IsInPolygon(Vertex[] poly, Vertex point)
		{
			var coef = poly.Skip(1).Select((p, i) =>
											(point.Y - poly[i].Y) * (p.X - poly[i].X)
										  - (point.X - poly[i].X) * (p.Y - poly[i].Y))
									.ToList();

			if (coef.Any(p => p == 0))
				return true;

			for (int i = 1; i < coef.Count(); i++)
			{
				if (coef[i] * coef[i - 1] < 0)
					return false;
			}
			return true;
		}

		public static bool IsInPolygon3(Facet facet, Vertex point)
		{
			Vertex fp1 = facet[0]; 
			Vertex fp2 = facet[2]; 
			if(point.X >= fp1.X && point.Y >= fp1.Y && point.Z >= fp1.Z &&
				point.X <= fp2.X && point.Y <= fp2.Y && point.Z <= fp2.Z)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public Vector CalculatePlaneNormal(Facet facet)
		{
			// Calculate two vectors lying on the plane
			Vector p1 = new Vector(facet.Vertices[0]);
			Vector p2 = new Vector(facet.Vertices[1]);
			Vector p3 = new Vector(facet.Vertices[2]);

			Vector v1 = p2 - p1;
			Vector v2 = p3 - p1;

			// Calculate the cross product of the two vectors
			Vector normal = VectorCrossProduct(v1, v2);

			// Normalize the normal vector
			normal = VectorNormalize(normal);

			return normal;
		}

		private Vector VectorCrossProduct(Vector vector1, Vector vector2)
		{
			double x = vector1[1] * vector2[2] - vector1[2] * vector2[1];
			double y = vector1[2] * vector2[0] - vector1[0] * vector2[2];
			double z = vector1[0] * vector2[1] - vector1[1] * vector2[0];
			return new Vector(new double[] { x, y, z });
		}

		private Vector VectorNormalize(Vector vector)
		{
			double length = Math.Sqrt(vector[0] * vector[0] + vector[1] * vector[1] + vector[2] * vector[2]);
			return new Vector(new double[] { vector[0] / length, vector[1] / length, vector[2] / length });
		}

		//Скалярний добуток
		private double VectorDotProduct(Vector vector1, Vector vector2)
		{
			return vector1[0] * vector2[0] + vector1[1] * vector2[1] + vector1[2] * vector2[2];
		}

		private Vector VectorScale(Vector vector, double scalar)
		{
			return new Vector(new double[] { vector[0] * scalar, vector[1] * scalar, vector[2] * scalar });
		}

	}
}
