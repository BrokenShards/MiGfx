// TestModule.cs //

using SFML.Graphics;

namespace SharpGfxTest
{
	public abstract class TestModule
	{
		/// <summary>
		///   Run module tests.
		/// </summary>
		/// <returns>
		///   True if tests succeeded, otherwise false.
		/// </returns>
		public virtual bool RunTest( RenderWindow window = null )
		{
			return OnTest();
		}

		/// <summary>
		///   Override this to run your module tests.
		/// </summary>
		/// <returns>
		///   True if tests succeeded, otherwise false.
		/// </returns>
		protected abstract bool OnTest();
	}
	public abstract class VisualTestModule : TestModule
	{
		/// <summary>
		///   Run module tests.
		/// </summary>
		/// <returns>
		///   True if tests succeeded, otherwise false.
		/// </returns>
		public override bool RunTest( RenderWindow window )
		{
			return OnTest() && OnVisualTest( window );
		}

		/// <summary>
		///   Run visual module tests.
		/// </summary>
		/// <param name="window">
		///   The render window.
		/// </param>
		/// <returns>
		///   True if tests succeeded, otherwise false.
		/// </returns>
		protected abstract bool OnVisualTest( RenderWindow window );
	}

	public static class Testing
	{
		public static bool Test<T>( RenderWindow window = null ) where T : TestModule, new()
		{
			return new T().RunTest( window );
		}
	}

}
	