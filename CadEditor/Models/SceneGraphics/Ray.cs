using System;

namespace CadEditor
{
	public class Ray
	{
		private Vector origin;
		private Vector direction;

		public Ray()
		{
		}
		public Ray(Vector start, Vector end)
		{
			origin = start;
			direction = end;
		}

		public Vector Origin { get { return origin; } set { origin = value; } }
		public Vector Direction { get { return direction; } set { direction = value; } }


		//public Point3D Intersects(ISceneObject obj)
		//{
		//	if (obj != null)
		//	{
		//		if(obj is Plane)
		//		{
		//			return RayIntersectsPlane((Plane)obj);
		//		}
		//		else if (obj is Line)
		//		{
		//			return RayIntersectsLine((Line)obj);
		//		}
		//	}

		//	return null;
		//} 

		public Point3D RayIntersectsPlane(Plane facet)
		{
			Vector intersectionPoint;
			Point3D facetCenterPoint = facet.GetCenterPoint();

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
			double distance = ((new Vector(facetCenterPoint) - origin) * planeNormal) / dotProduct;
			//double distance = ((new Vector(facet.Vertices[0]) - origin) * planeNormal) / dotProduct;
			
			// Check if the intersection point is behind the ray origin
			if (distance < 0)
			{
				return null;
			}

			// Calculate the intersection point
			intersectionPoint = origin + distance * direction;

			return new Point3D(intersectionPoint);
		}

		public Point3D RayIntersectsLine(Line edge)
		{
			double coPlanerThreshold = 0.1;

			Vector da = Direction;
			Vector db = edge.P2 - edge.P1;
			Vector dc = new Vector(edge.P1) - Origin;

			double dd = Math.Abs(dc * da.Cross(db));

			if (dd >= coPlanerThreshold)
			{
				return null;
			}

			double s = (dc.Cross(db) * da.Cross(db)) / da.Cross(db).LengthSquared(); // find distance

			if (s >= 0.0)   // Means we have an intersection
			{
				Vector intersection = Origin + s * da;
				return new Point3D(intersection);
			}
			else
			{
				return null;
			}

		}

		//public Point3D RayIntersectsVertex(Point3D vertex)
		//{
		//	// Calculate the components of the direction vector.
		//	double dx = Direction[0];
		//	double dy = Direction[1];
		//	double dz = Direction[2];

		//	// Calculate the parameter values for each component.
		//	double tx = (vertex.X - Origin[0]) / dx;
		//	double ty = (vertex.Y - Origin[1]) / dy;
		//	double tz = (vertex.Z - Origin[2]) / dz;

		//	// Check if all the parameter values are equal (within some tolerance).
		//	double accuracy = 1.4;
		//	if (Math.Abs(tx - ty) < accuracy && Math.Abs(ty - tz) < accuracy)
		//	{
		//		// The point lies on the ray.
		//		return new Point3D(tx, ty, tz);
		//	}
		//	else
		//	{
		//		// The point does not lie on the ray.
		//		return null;
		//	}
		//}
	}
}
