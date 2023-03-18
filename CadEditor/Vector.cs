using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadEditor
{
	public class Vector
	{
		private double[] values;
		public int Size { get; private set; }

		public Vector(int _Size)
		{
			Size = _Size;
			values = new double[Size];
		}

		public Vector(double[] _values)
		{
			values = _values;
			Size = values.Length;
		}

		public Vector(Vertex vertex)
		{
			Size = 3;
			values = new double[] {vertex.X, vertex.Y, vertex.Z};
		}

		public int GetSize() 
		{
			return Size;
		}

		public double this[int index]
		{
			get
			{
				if (index < Size && index >= 0)
				{
					return values[index];
				}
				else
				{
					throw new ArgumentException("Wrong index!");
				}
			}
			set 
			{
				if(index < Size && index >= 0)
				{
					values[index] = value;
				}
				else
				{
					throw new ArgumentException("Wrong index!");
				}
			}
		}

		public static Vector operator -(Vector a, Vector b)
		{
			int sizeA = a.Size;
			int sizeB = b.Size;

			if (sizeA == sizeB)
			{
				double[] result = new double[sizeA];

				for (int i = 0; i < sizeA; i++)
				{

					result[i] = a[i] - b[i];
				}

				return new Vector(result);
			}
			else
			{
				throw new ArgumentException("The vectors have different dimensions");
			}
		}

		public static Vector Cross(Vector v1, Vector v2)
		{
			double x = v1[1] * v2[2] - v1[2] * v2[1];
			double y = v1[2] * v2[0] - v1[0] * v2[2];
			double z = v1[0] * v2[1] - v1[1] * v2[0];

			return new Vector(new double[] {x, y, z});
		}

		public static double operator *(Vector a, Vector b)
		{
			return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
		}

		public static double operator /(Vector a, Vector b)
		{
			return a[0] / b[0] + a[1] / b[1] + a[2] / b[2];
		}

		public static Vector operator *(double a, Vector v)
		{
			double[] result = new double[v.Size];
			for (int i = 0; i < v.Size; i++)
			{
				result[i] = v[i] * a;
			}
			return new Vector(result);
		}


		public static Vector operator +(Vector a, Vector b)
		{
			int sizeA = a.Size;
			int sizeB = b.Size;

			if (sizeA == sizeB)
			{
				double[] result = new double[sizeA];

				for (int i = 0; i < sizeA; i++)
				{

					result[i] = a[i] + b[i];
				}

				return new Vector(result);
			}
			else
			{
				throw new ArgumentException("The vectors have different dimensions");
			}
		}

		public static bool operator ==(Vector a, Vector b)
		{
			return a[0] == b[0] && a[1] == b[1] && a[2] == b[2];
		}

		public static bool operator !=(Vector a, Vector b)
		{
			return a[0] != b[0] || a[1] != b[1] || a[2] != b[2];
		}

		public override string ToString()
		{
			return String.Format("({0}, {1}, {2})", values[0], values[1], values[2]);
		}

		public static Vector Normalize(Vector v)
		{
			double[] values = v.values;
			double length = Math.Sqrt(values[0] * values[0] + values[1] * values[1] + values[2] * values[2]);
			values[0] /= length;
			values[1] /= length;
			values[2] /= length;

			return new Vector(values);
		}
	}
}
