using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExecutionActors
{
	public static class ActorsMan
	{
		private static List<Actor> pActors = new List<Actor>();

		public static Actor NewActor(Type type, object data)
		{
			Actor actor = (Actor)Activator.CreateInstance(type);
			actor.Init(data);
			pActors.Add(actor);
			actor.Run();
			return actor;
		}

		public static Actor[] Actors
		{
			get { return pActors.ToArray(); }
		}
	}
}
