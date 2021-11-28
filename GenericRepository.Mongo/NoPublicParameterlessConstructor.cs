using System;

namespace GenericRepository.Mongo
{
	internal class NoPublicParameterlessConstructor : Exception
	{
		public Type Type { get; }

		public NoPublicParameterlessConstructor(Type type)
		{
			Type = type;
		}
	}
}