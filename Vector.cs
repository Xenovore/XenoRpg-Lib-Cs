using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRpg
{
	public class Vector
	{
		public float m_fX;
		public float m_fY;
		public float m_fZ;

		//TODO: Vector ops as necessary
		public Vector( float x, float y, float z )
		{
			m_fX = x;
			m_fY = y;
			m_fZ = z;
		}
	}
}
