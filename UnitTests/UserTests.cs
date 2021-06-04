using System;
using System.Threading.Tasks;
using UnitTests.Auxiliary;
using WebLicense.Shared.Identity;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class UserTests : TestBaseAx<UserInfo>
    {
        #region C-tor | Fields | Properties

        public UserTests(ITestOutputHelper output) : base(output)
        {
        }

        #endregion

        [Theory]
        [InlineData("", "", "", "", "")]
        public async Task Add_Success(string email, string name, string phone, string password, string refId)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);
        }

        #region Methods

        protected override void CompareModels(UserInfo @new, UserInfo info, UserInfo old)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}