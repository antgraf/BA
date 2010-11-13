using System;

namespace WindowEntity
{
	public class InitializationException: ApplicationException
	{
		public InitializationException()
		{ }

		public InitializationException(string message)
			: base(message)
		{ }
	}

	public class CoordinatesOutOfRangeException: ApplicationException
	{
		public CoordinatesOutOfRangeException()
		{ }

		public CoordinatesOutOfRangeException(string message)
			: base(message)
		{ }
	}
}
