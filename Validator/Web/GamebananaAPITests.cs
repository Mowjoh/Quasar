using DataModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.Web;
using Xunit;

namespace Testing_Web
{
    /// <summary>
    /// Aimed at validating interactions with Gamebanana's API
    /// </summary>
    public class APITests
    {
        /// <summary>
        /// Asserts that Gamebanana responds
        /// </summary>
        [Fact]
        public async void ValidateBasicResponse()
        {
            APIMod Response = await APIRequest.GetModInformation("324701");
            Assert.True(Response != null);
        }

        /// <summary>
        /// Asserts that all the data from a specific mod are properly parsed and deserialized
        /// </summary>
        [Fact]
        public async void ValidateRequestDetails()
        {
            APIMod Response = await APIRequest.GetModInformation("322837");

            Assert.True((Response.Authors.Authors != null) || (Response.Authors.KeyAuthors != null));
            Assert.Equal(322837, Response.ID);
            Assert.Equal(6498, Response.Game.ID);
            Assert.Equal(1480922, Response.Submitter.ID);
            Assert.Equal("Devory", Response.Submitter.Name);
            Assert.Equal("Super Smash Bros. Ultimate", Response.Game.Name);
            Assert.Equal("Violet Sunset CSS", Response.Name);
            Assert.Equal("UI", Response.SuperCategory.Name);
            Assert.Equal("CSS Background", Response.SubCategory.Name);
            Assert.Equal("Mod", Response.GamebananaRootCategoryName);
            
            
        }

        /// <summary>
        /// Validates necessary data for multiple mods
        /// </summary>
        /// <param name="ModID"></param>
        [Theory]
        [InlineData("175268")]
        [InlineData("324106")]
        [InlineData("324202")]
        [InlineData("318522")]
        [InlineData("324676")]
        [InlineData("310949")]
        [InlineData("318734")]
        [InlineData("321199")]
        [InlineData("38828")]
        public async void ValidateMultipleRequest(string ModID)
        {
            APIMod Response = await APIRequest.GetModInformation(ModID);

            Assert.True((Response.Authors.Authors != null) || (Response.Authors.KeyAuthors != null) || (Response.Submitter != null));
            Assert.True(Response.ID > 0);
            Assert.Equal(6498, Response.Game.ID);
            Assert.Equal("Super Smash Bros. Ultimate", Response.Game.Name);
            Assert.True(Response.Name != null);
            Assert.True(Response.SubCategory != null);
            Assert.Equal("Mod", Response.GamebananaRootCategoryName);

        }
    }
}
