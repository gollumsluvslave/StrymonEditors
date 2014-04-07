using System;
using Moq;
namespace RITS.StrymonEditor.Tests
{
    /// <summary>
    /// Variation on AutoMock base class found here 
    /// http://code.google.com/p/moq-contrib/wiki/Automocking
    /// With nice ideas from http://andrewblogs.com/moq-testing-tutorial/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TestContext<T> where T : class
    {
        private MockRepository _repository;
        private T _sut;
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMockingTests"/> class.
        /// </summary>
        /// <remarks>Defaults to <see cref="MockBehavior.Loose"/>.</remarks>
        protected TestContext()
        {
            Globals.Init();
            this._repository = new MockRepository(MockBehavior.Loose);
                this.Container = new AutoMockContainer( this._repository );
        }

        /// <summary>
        /// Changes mocks behavior to <see cref="MockBehavior.Strict"/>.
        /// </summary>
        protected void UseStrict()
        {
            this._repository = new MockRepository(MockBehavior.Strict);
                this.Container = new AutoMockContainer( this._repository );
        }

        /// <summary>
        /// Forces factory verification.
        /// </summary>
        /// <remarks>This is called automatically by xUnit.</remarks>
        public virtual void Dispose()
        {
                if( this.VerifyAll )
                        this._repository.VerifyAll();
                else
                        this._repository.Verify();
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="MockFactory.VerifyAll"/> will be used or <see cref="MockFactory.Verify"/>.
        /// </summary>
        /// <value><c>true</c> to use <see cref="MockFactory.VerifyAll"/>; otherwise, <c>false</c>.</value>
        /// <remarks><c>false</c> be default.</remarks>
        public bool VerifyAll { get; set; }

        /// <summary>
        /// Gets or sets the auto mocking container.
        /// </summary>
        /// <value>The auto mocking container.</value>
        protected AutoMockContainer Container { get; private set; }

        /// <summary>
        /// Creates an object of the specified type.
        /// </summary>
        /// <typeparam name="T">A type to create.</typeparam>
        /// <returns>
        /// Object of the type <typeparamref name="T"/>.
        /// </returns>
        /// <remarks>Usually used to create objects to test. Any non-existing dependencies
        /// are mocked.
        /// <para>Container is used to resolve build dependencies.</para></remarks>
        public T Create()
        {
                return this.Container.Create<T>();
        }

        /// <summary>
        /// Creates an object of the specified type.
        /// </summary>
        /// <typeparam name="T">A type to create.</typeparam>
        /// <param name="activator">The activator used to create object.</param>
        /// <returns>
        /// Object of the type <typeparamref name="T"/>.
        /// </returns>
        /// <remarks>Usually used to create objects to test. Any non-existing dependencies
        /// are mocked.
        /// <para>Container is used to resolve build dependencies.</para></remarks>
        /// <example>Using activator</example>
        /// <code>this.Create( c => new Apple( c.Resolve&lt; ITree> ()) )</code>
        /// <seealso href="http://code.google.com/p/autofac/wiki/ResolveParameters"/>
        public T Create( Func< IResolve, T > activator )
        {
                return this.Container.Create( activator );
        }

        /// <summary>
        /// Resolves an object from the container.
        /// </summary>
        /// <typeparam name="T">Type to resolve for.</typeparam>
        /// <returns>Resolved object.</returns>
        public T Resolve ()
        {
                return this.Container.Resolve<T>();
        }

        /// <summary>
        /// Creates a mock object.
        /// </summary>
        /// <typeparam name="T">A type to mock.</typeparam>
        /// <returns>Mocked object of the type <typeparamref name="T"/>.</returns>
        /// <remarks>Used to set-up expected mock behavior when object created with <see cref="Create{T}()"/>.
        /// calls on the mock.
        /// <para>To set-up expected calls to mock use
        /// <code>Mock&lt; type >().Expect( x => x.Method( params )).Returns( fakeResults )</code></para></remarks>
        public Mock<T> Mock()
        {
                return this.Container.GetMock<T>();
        }

        /// <summary>
        /// Captures which class to use to provide specified service.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void Register < TService, TImplementation >()
        {
                this.Container.Register< TService, TImplementation >();
        }

        /// <summary>
        /// Captures <paramref name="instance"/> as the object to provide <typeparamref name="TService"/> for mocking.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The instance.</param>
        public void Register < TService > ( TService instance )
        {
                this.Container.Register( instance );
        }

        /// <summary>
        /// Registers the given service creation delegate on the container.
        /// </summary>
        /// <typeparam name="TService">Service type.</typeparam>
        public void Register < TService > ( Func< IResolve, TService > activator )
        {
                this.Container.Register( activator );
        }

        // Allow tests to create their own SUT
        protected T Sut
        {
            get
            {
                if(_sut==null) return Container.Create<T>();
                return _sut;
            }
            set
            {
                _sut = value;
            }
        }

    }
}