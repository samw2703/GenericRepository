using System;
using System.Threading.Tasks;

namespace GenericRepository.TestContext
{
	public class EntityAssertions<T>
	{
		private readonly T _entity;
		private readonly IAssertionRules _assertionRules;

		internal EntityAssertions(T entity, IAssertionRules assertionRules)
		{
			_entity = entity;
			_assertionRules = assertionRules;
		}

		public void True(Func<T, bool> predicate)
			=> _assertionRules.True(predicate(_entity), $"{typeof(T).Name} did not match");

		public void False(Func<T, bool> predicate)
			=> _assertionRules.False(predicate(_entity), $"{typeof(T).Name} matched but was not expected to");
	}

	public static class EntityAssertionsExtensions
	{
		public static async Task True<T>(this Task<EntityAssertions<T>> task, Func<T, bool> predicate)
			=> (await task).True(predicate);

		public static async Task False<T>(this Task<EntityAssertions<T>> task, Func<T, bool> predicate)
			=> (await task).False(predicate);
	}
}