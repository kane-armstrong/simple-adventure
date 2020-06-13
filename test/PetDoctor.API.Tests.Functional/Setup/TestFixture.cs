using System.Net.Http;

namespace PetDoctor.API.Tests.Functional.Setup
{
    public class TestFixture
    {
        private readonly TestApiFactory _webApplicationFactory;
        public HttpClient Client { get; }

        public TestFixture()
        {
            _webApplicationFactory = new TestApiFactory();
            Client = _webApplicationFactory.CreateClient();
        }
    }
}
