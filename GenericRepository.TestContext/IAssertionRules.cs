namespace GenericRepository.TestContext
{
	public interface IAssertionRules
	{
		void True(bool result, string failMessage);
		void False(bool result, string failMessage);
		void NotNull(object? obj, string failMessage);
		void Null(object? obj, string failMessage);
		void Fail(string message);
	}
}