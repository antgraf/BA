using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowEntity
{
	public enum CoordinateType
	{
		Absolute,
		Relative,
		Stretched
	}

	public struct Point 
	{
		public int X;
		public int Y;
	}

	public struct StretchedPoint 
	{
		public double X;
		public double Y;
	}

	public class Coordinate
	{
		private CoordinateType pType = CoordinateType.Relative;
		private double pX = -1;
		private double pY = -1;

		public Coordinate()
		{}

		public Coordinate(CoordinateType type, Point point)
		{
			if(type == CoordinateType.Stretched) throw new ArgumentException("Use ctor w/ StretchedPoint instead.", "type");
			pType = type;
			pX = point.X;
			pY = point.Y;
		}

		public Coordinate(StretchedPoint point)
		{
			pType = CoordinateType.Stretched;
			pX = point.X;
			pY = point.Y;
		}

		private void CheckInit()
		{
			if(pX < 0.0 || pY < 0.0) throw new InitializationException("Coordinate was not initialized properly.");
			if(pType == CoordinateType.Stretched && (pX > 1.0 || pY > 1.0))
				throw new CoordinatesOutOfRangeException("Stretched coordinates must be <= 1.0");
		}

		public Point ToAbsolute(Window window)
		{
			CheckInit();
			Point p = new Point();
			switch(pType)
			{
				case CoordinateType.Absolute:
				{
					p.X = (int)pX;
					p.Y = (int)pY;
					break;
				}
				case CoordinateType.Relative:
				{
					if(window == null) throw new NullReferenceException("Relative point calculation needs Window set.");
					p.X = (int)pX + window.X;
					p.Y = (int)pY + window.Y;
					break;
				}
				case CoordinateType.Stretched:
				{
					if(window == null) throw new NullReferenceException("Stretched point calculation needs Window set.");
					p.X = (int)(pX * window.Width) + window.X;
					p.Y = (int)(pY * window.Height) + window.Y;
					break;
				}
			}
			return p;
		}

		public Point ToRelative(Window window)
		{
			CheckInit();
			Point p = new Point();
			switch(pType)
			{
				case CoordinateType.Absolute:
				{
					if(window == null) throw new NullReferenceException("Absolute point calculation needs Window set.");
					p.X = (int)pX - window.X;
					p.Y = (int)pY - window.Y;
					break;
				}
				case CoordinateType.Relative:
				{
					p.X = (int)pX;
					p.Y = (int)pY;
					break;
				}
				case CoordinateType.Stretched:
				{
					if(window == null) throw new NullReferenceException("Stretched point calculation needs Window set.");
					p.X = (int)(pX * window.Width);
					p.Y = (int)(pY * window.Height);
					break;
				}
			}
			return p;
		}

		public StretchedPoint ToStretched(Window window)
		{
			CheckInit();
			StretchedPoint p = new StretchedPoint();
			switch(pType)
			{
				case CoordinateType.Absolute:
				{
					if(window == null) throw new NullReferenceException("Absolute point calculation needs Window set.");
					p.X = (pX - window.X) / window.Width;
					p.Y = (pY - window.Y) / window.Height;
					break;
				}
				case CoordinateType.Relative:
				{
					if(window == null) throw new NullReferenceException("Relative point calculation needs Window set.");
					p.X = pX / window.Width;
					p.Y = pY / window.Height;
					break;
				}
				case CoordinateType.Stretched:
				{
					p.X = pX;
					p.Y = pY;
					break;
				}
			}
			return p;
		}

		public WindowEntity.CoordinateType Type
		{
			get { return pType; }
			set { pType = value; }
		}

		public double X
		{
			get { return pX; }
			set { pX = value; }
		}

		public double Y
		{
			get { return pY; }
			set { pY = value; }
		}
	}
}
