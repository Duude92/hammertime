using System;
using System.Numerics;

namespace Sledge.Common
{
	/// <summary>
	/// Common math-related functions
	/// </summary>
	public static class MathHelper
	{
		/// <summary>
		/// Convert degrees to radians
		/// </summary>
		/// <param name="degrees">The angle in degrees</param>
		/// <returns>The angle in radians</returns>
		public static double DegreesToRadians(double degrees)
		{
			return degrees * Math.PI / 180;
		}
		/// <summary>
		/// Convert degrees to radians
		/// </summary>
		/// <param name="degrees">The angle in degrees</param>
		/// <returns>The angle in radians</returns>
		public static float DegreesToRadians(float degrees)
		{
			return (float)(degrees * Math.PI / 180);
		}

		/// <summary>
		/// Convert radians to degrees
		/// </summary>
		/// <param name="radians">The angle in radians</param>
		/// <returns>The angle in degrees</returns>
		public static double RadiansToDegrees(double radians)
		{
			return radians * 180 / Math.PI;
		}

		/// <summary>
		/// Convert radians to degrees
		/// </summary>
		/// <param name="radians">The angle in radians</param>
		/// <returns>The angle in degrees</returns>
		public static float RadiansToDegrees(float radians)
		{
			return (float)(radians * 180 / Math.PI);
		}
		/// <summary>
		/// Extract Euler angles from a matrix
		/// </summary>
		/// <param name="matrix">The matrix to extract from</param>
		/// <returns></returns>
		public static Vector3 ExtractEulerAngles(Matrix4x4 matrix)
		{
			float x, y, z;

			// Extract rotation around Y axis
			y = (float)Math.Asin(matrix.M13);

			// Handle special cases for pitch near +-90 degrees
			if ((float)Math.Abs(matrix.M13) < 0.99999)
			{
				// Extract rotation around X and Z axes
				x = (float)Math.Atan2(-matrix.M23, matrix.M33);
				z = (float)Math.Atan2(-matrix.M12, matrix.M11);
			}
			else
			{
				// Gimbal lock case: rotation around X axis is set to 0, and extract rotation around Z axis
				x = 0;
				z = (float)Math.Atan2(matrix.M21, matrix.M22);
			}

			return new Vector3(x, y, z);
		}
	}
}
