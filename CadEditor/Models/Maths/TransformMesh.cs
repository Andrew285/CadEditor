﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor.Maths
{
	public static class TransformMesh
	{
		public static double PhiAngle(Point3D v1, Point3D v2)
		{
			return (double)1 / 8 * (1 + v1[0] * v2[0]) * (1 + v1[1] * v2[1]) * (1 + v1[2] * v2[2]) *
				(v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2] - 2);
		}

		public static double PhiEdge(Point3D v1, Point3D v2)
		{
			return (double)1 / 4 * (1 + v1[0] * v2[0]) * (1 + v1[1] * v2[1]) * (1 + v1[2] * v2[2]) *
				(1 - Math.Pow((v1[0] * v2[1] * v2[2]), 2) - Math.Pow((v1[1] * v2[0] * v2[2]), 2) - Math.Pow((v1[2] * v2[0] * v2[1]), 2));
		}

        public static float DegreesToRadians(float degrees)
        {
            return (float)(degrees * Math.PI / 180.0);
        }
    }
}
