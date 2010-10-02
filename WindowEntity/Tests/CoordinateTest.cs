using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace WindowEntity.Tests
{
	[TestFixture]
	class CoordinateTest
	{
		private Point CreateStdPoint()
		{
			return new Point() { X = 100, Y = 200 };
		}

		private StretchedPoint CreateStdStretchedPoint()
		{
			return new StretchedPoint() { X = 0.150, Y = 0.250 };
		}

		private Window CreateStdWindow()
		{
			return new Window() { X = 600, Y = 500, Width = 640, Height = 480, Title = "TEST Window!!" };
		}

		[Test]
		public void Create()
		{
			Point p = new Point();
			Assert.NotNull(p);
			p.X = 100;
			p.Y = 200;
			StretchedPoint sp = new StretchedPoint();
			Assert.NotNull(sp);
			sp.X = 150.0;
			sp.Y = 250.0;
			Coordinate c0 = new Coordinate();
			Coordinate c1 = new Coordinate(CoordinateType.Absolute, p);
			Coordinate c2 = new Coordinate(CoordinateType.Relative, p);
			Coordinate c3 = new Coordinate(sp);
			Assert.NotNull(c0);
			Assert.NotNull(c1);
			Assert.NotNull(c2);
			Assert.NotNull(c3);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void CreateWrong()
		{
			Point p = new Point();
			Coordinate c = new Coordinate(CoordinateType.Stretched, p);
			Assert.Null(c);
		}

		[Test]
		public void WrongCalls()
		{
			Window w = CreateStdWindow();
			Coordinate c0 = new Coordinate();
			Assert.Catch<InitializationException>(delegate { c0.ToAbsolute(null); });
			Assert.Catch<InitializationException>(delegate { c0.ToAbsolute(w); });
			Assert.Catch<InitializationException>(delegate { c0.ToRelative(null); });
			Assert.Catch<InitializationException>(delegate { c0.ToRelative(w); });
			Assert.Catch<InitializationException>(delegate { c0.ToStretched(null); });
			Assert.Catch<InitializationException>(delegate { c0.ToStretched(w); });
		}

		[Test]
		public void WrongPoint()
		{
			StretchedPoint wrong_p1 = new StretchedPoint();
			wrong_p1.X = 100.0;
			wrong_p1.Y = 0.0;
			StretchedPoint wrong_p2 = new StretchedPoint();
			wrong_p2.X = 0.0;
			wrong_p2.Y = 100.0;
			StretchedPoint wrong_p3 = new StretchedPoint();
			wrong_p3.X = 100.0;
			wrong_p3.Y = 200.0;
			Coordinate c1 = new Coordinate(wrong_p1);
			Coordinate c2 = new Coordinate(wrong_p2);
			Coordinate c3 = new Coordinate(wrong_p3);
			Window w = CreateStdWindow();
			Assert.Catch<CoordinatesOutOfRangeException>(delegate { c1.ToAbsolute(w); });
			Assert.Catch<CoordinatesOutOfRangeException>(delegate { c2.ToAbsolute(w); });
			Assert.Catch<CoordinatesOutOfRangeException>(delegate { c3.ToAbsolute(w); });
			Assert.Catch<CoordinatesOutOfRangeException>(delegate { c1.ToRelative(w); });
			Assert.Catch<CoordinatesOutOfRangeException>(delegate { c2.ToRelative(w); });
			Assert.Catch<CoordinatesOutOfRangeException>(delegate { c3.ToRelative(w); });
			Assert.Catch<CoordinatesOutOfRangeException>(delegate { c1.ToStretched(w); });
			Assert.Catch<CoordinatesOutOfRangeException>(delegate { c2.ToStretched(w); });
			Assert.Catch<CoordinatesOutOfRangeException>(delegate { c3.ToStretched(w); });
		}

		[Test]
		public void ToAbsolute()
		{
			// Init
			Point p = CreateStdPoint();
			StretchedPoint sp = CreateStdStretchedPoint();
			Window w = CreateStdWindow();
			Coordinate c0 = new Coordinate();
			Coordinate c1 = new Coordinate(CoordinateType.Absolute, p);
			Coordinate c2 = new Coordinate(CoordinateType.Relative, p);
			Coordinate c3 = new Coordinate(sp);
			// Null argument
			Point a = c1.ToAbsolute(null);
			Assert.AreEqual(a.X, p.X);
			Assert.AreEqual(a.Y, p.Y);
			Assert.Catch<NullReferenceException>(delegate { c2.ToAbsolute(null); });
			Assert.Catch<NullReferenceException>(delegate { c3.ToAbsolute(null); });
			// Normal calls
			Point a1 = c1.ToAbsolute(w);
			Assert.AreEqual(a1.X, p.X);
			Assert.AreEqual(a1.Y, p.Y);
			Point r = c2.ToAbsolute(w);
			Assert.AreEqual(r.X, 700);
			Assert.AreEqual(r.Y, 700);
			Point s = c3.ToAbsolute(w);
			Assert.AreEqual(s.X, 696);
			Assert.AreEqual(s.Y, 620);
		}

		[Test]
		public void ToRelative()
		{
			// Init
			Point p = CreateStdPoint();
			StretchedPoint sp = CreateStdStretchedPoint();
			Window w = CreateStdWindow();
			Coordinate c0 = new Coordinate();
			Coordinate c1 = new Coordinate(CoordinateType.Absolute, p);
			Coordinate c2 = new Coordinate(CoordinateType.Relative, p);
			Coordinate c3 = new Coordinate(sp);
			// Null argument
			Assert.Catch<NullReferenceException>(delegate { c1.ToRelative(null); });
			Point r = c2.ToRelative(null);
			Assert.AreEqual(r.X, p.X);
			Assert.AreEqual(r.Y, p.Y);
			Assert.Catch<NullReferenceException>(delegate { c3.ToRelative(null); });
			// Normal calls
			Point a = c1.ToRelative(w);
			Assert.AreEqual(a.X, -500);
			Assert.AreEqual(a.Y, -300);
			Point r1 = c2.ToRelative(w);
			Assert.AreEqual(r1.X, p.X);
			Assert.AreEqual(r1.Y, p.Y);
			Point s = c3.ToRelative(w);
			Assert.AreEqual(s.X, 96);
			Assert.AreEqual(s.Y, 120);
		}

		[Test]
		public void ToStretched()
		{
			// Init
			Point p = CreateStdPoint();
			StretchedPoint sp = CreateStdStretchedPoint();
			Window w = CreateStdWindow();
			Coordinate c0 = new Coordinate();
			Coordinate c1 = new Coordinate(CoordinateType.Absolute, p);
			Coordinate c2 = new Coordinate(CoordinateType.Relative, p);
			Coordinate c3 = new Coordinate(sp);
			// Null argument
			Assert.Catch<NullReferenceException>(delegate { c1.ToStretched(null); });
			Assert.Catch<NullReferenceException>(delegate { c2.ToStretched(null); });
			StretchedPoint s = c3.ToStretched(null);
			Assert.AreEqual(s.X, sp.X);
			Assert.AreEqual(s.Y, sp.Y);
			// Normal calls
			StretchedPoint a = c1.ToStretched(w);
			Assert.AreEqual(a.X, -0.78125);
			Assert.AreEqual(a.Y, -0.625);
			StretchedPoint r = c2.ToStretched(w);
			Assert.AreEqual(r.X, 0.15625);
			Assert.AreEqual(r.Y, 0.41666666666666666);
			StretchedPoint s1 = c3.ToStretched(w);
			Assert.AreEqual(s1.X, sp.X);
			Assert.AreEqual(s1.Y, sp.Y);
		}
	}
}
