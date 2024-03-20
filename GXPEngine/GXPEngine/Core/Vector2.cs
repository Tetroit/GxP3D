using System;

namespace GXPEngine.Core
{
	public struct Vector2
	{
		public float x;
		public float y;
		
		public Vector2 (float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public static Vector2 operator + (Vector2 v1, Vector2 v2) => new Vector2(v1.x+v2.x, v1.y+v2.y);
		public static Vector2 operator -(Vector2 v) => new Vector2(-v.x, -v.y);
		public static Vector2 operator -(Vector2 v1, Vector2 v2) => v1 + -v2;
		public static Vector2 operator *(Vector2 v, float s) => new Vector2(v.x*s, v.y*s);
		
		override public string ToString() {
			return "[Vector2 " + x + ", " + y + "]";
		}
	}
}

