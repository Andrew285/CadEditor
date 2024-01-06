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

		public Vector(double X, double Y, double Z)
		{
			Size = 3;
			values = new double[] { X, Y, Z };
		}

		public Vector(Point3D point)
		{
			Size = 3;
			values = new double[] { point.X, point.Y, point.Z};
		}

        public double Length()
        {
            return Math.Sqrt(values[0] * values[0] + values[1] * values[1] + values[2] * values[2]);
        }

        public int GetSize() 
		{
			return Size;
		}

		public override string ToString()
		{
			StringBuilder result = new StringBuilder();
			result.Append("(");
			for (int i = 0; i < Size; i++)
			{
				result.Append(String.Format("{0}", values[i]));

				if (i == Size - 1)
				{
					result.Append(")");
				}
				else
				{
					result.Append(", ");
				}
			}
			return result.ToString();
		}

		#region --- Overriding operators ---
		public static double operator *(Vector a, Vector b)
		{
			return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
		}

		public static double operator /(Vector a, Vector b)
		{
			return a[0] / b[0] + a[1] / b[1] + a[2] / b[2];
		}

        public static Vector operator /(Vector a, int b)
        {
            return new Vector(a[0] / b, a[1] / b, a[2] / b);
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

		public static Vector operator *(Vector v, double a)
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

        public static Vector operator -(Vector a, double b)
        {
			int sizeA = a.Size;

            double[] result = new double[sizeA];

            for (int i = 0; i < sizeA; i++)
            {

                result[i] = a[i] - b;
            }

            return new Vector(result);
        }

        public static bool operator ==(Vector a, Vector b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}
			if (object.ReferenceEquals(a, null) ||
			object.ReferenceEquals(b, null))
			{
				return false;
			}
			return a[0] == b[0] && a[1] == b[1] && a[2] == b[2];
		}

		public static bool operator !=(Vector a, Vector b)
		{
			return !(a == b);
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
				if (index < Size && index >= 0)
				{
					values[index] = value;
				}
				else
				{
					throw new ArgumentException("Wrong index!");
				}
			}
		}

		#endregion

		public Vector Cross(Vector v2)
		{
			double x = this[1] * v2[2] - this[2] * v2[1];
			double y = this[2] * v2[0] - this[0] * v2[2];
			double z = this[0] * v2[1] - this[1] * v2[0];

			return new Vector(new double[] { x, y, z });
		}

		public Vector Normalize()
		{
			double[] values = this.values;
			double length = Math.Sqrt(values[0] * values[0] + values[1] * values[1] + values[2] * values[2]);
			values[0] /= length;
			values[1] /= length;
			values[2] /= length;

			return new Vector(values);
		}

        public double LengthSquared()
        {
            return this[0] * this[0] + this[1] * this[1] + this[2] * this[2];
        }
    }
}
