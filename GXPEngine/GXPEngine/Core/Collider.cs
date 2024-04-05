using System;

namespace GXPEngine.Core
{
	public class Collider
	{
		public bool isTrigger {
			get {
				return _isTrigger;
			}
			set {
				_isTrigger = value;
			}
		}
		bool _isTrigger=false;

		public Collider ()
		{
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														HitTest()
		//------------------------------------------------------------------------------------------------------------------------		
		/// <summary>
		/// Returns <c>true</c> if this collider is currently overlapping with the collider other.
		/// </summary>
		public virtual bool HitTest (Collider other) {
			return false;
		}
		
		//------------------------------------------------------------------------------------------------------------------------
		//														HitTest()
		//------------------------------------------------------------------------------------------------------------------------		
		/// <summary>
		/// Returns <c>true</c> if this collider is currently overlapping with the point x,y.
		/// </summary>
		public virtual bool HitTestPoint (float x, float y, float z) {
			return false;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RayCastTest()
        //------------------------------------------------------------------------------------------------------------------------		
        /// <summary>
        /// Returns <c>true</c> if this collider is currently overlapping with the ray from point start to end.
        /// </summary>
        public virtual bool RayCastTest(Vector3 start, Vector3 end)
        {
            return false;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RayCastTest()
        //------------------------------------------------------------------------------------------------------------------------		
        /// <summary>
        /// If this collider is currently overlapping with the ray from point start to end 
		/// returns <c>true</c>, <paramref name="normal"/> of the collision and the <paramref name="distance"/> to the point of impact.
        /// </summary>
        public virtual bool RayCast(Vector3 p1, Vector3 p2, out float distance, out Vector3 normal, GameObject worldSpace = null)
        {
			distance = float.MaxValue;
			normal = Vector3.zero;
            return false;
        }

        /// <summary>
        /// If this collider would collide with collider other after moving by vx,vy,
        /// then this method returns the time of impact of the collision, which is a number between 
        /// 0 (=immediate collision, or already overlapping) and 1 (=collision after moving exactly by vx,vy).
        /// Otherwise, a number larger than 1 (e.g. float.MaxValue) is returned.
        /// In addition, the collision normal is returned, in case of a collision.
        /// </summary>
        /// <returns>The time of impact.</returns>
        /// <param name="other">Another collider.</param>
        /// <param name="vx">x velocity or translation amount.</param>
        /// <param name="vy">y velocity or translation amount.</param>
        /// <param name="normal">The collision normal.</param>
        public virtual float TimeOfImpact (Collider other, float vx, float vy, float vz, out Vector3 normal) {
			normal = new Vector3 ();
			return float.MaxValue;
		}

		/// <summary>
		/// If this collider and the collider other are overlapping, this method returns useful collision info such as
		/// the collision normal, the point of impact, and the penetration depth, 
		/// contained in a Collision object (the time of impact field will always be zero).
		/// 
		/// If they are not overlapping, this method returns null.
		/// </summary>
		public virtual Collision GetCollisionInfo (Collider other)
		{
			return null;
		}			
	}
}

