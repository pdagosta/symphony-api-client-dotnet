﻿using apiClientDotNet;
using apiClientDotNet.Authentication;
using apiClientDotNet.Clients;
using apiClientDotNet.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace apiClientDotNetTest
{
    [TestClass]
    public class UserClientIntegrationTest
    {
        private static SymBotClient symBotClient;
        private static IConfigurationRoot config;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            // Load integration test settings
            var integrationConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "integration.parameters.json");
            config = new ConfigurationBuilder().AddJsonFile(integrationConfigPath).Build();

            // Create SymBotClient
            var symConfig = new SymConfig();
            var symConfigLoader = new SymConfigLoader();
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "config.json");
            symConfig = symConfigLoader.loadFromFile(configPath);
            var botAuth = new SymBotRSAAuth(symConfig);
            botAuth.authenticate();
            symBotClient = SymBotClient.initBot(symConfig, botAuth);
        }

        [TestMethod]
        public void GetUsersV3_forGivenListOfEmails_correctlyReturnListOfUsers()
        {
            var emails = config.GetSection(this.GetType().Name).GetSection("test_email_addresses").Value;
            var emailList = emails.Split(",");
            var sut = new UserClient(symBotClient);

            List<UserInfo> listUserInfo = sut.getUsersV3(new List<string>(emailList), null, false);

            Assert.IsNotNull(listUserInfo);
            Assert.AreEqual(3, listUserInfo.Count);
            Assert.IsFalse(String.IsNullOrEmpty(listUserInfo[0].displayName));
            Assert.IsFalse(String.IsNullOrEmpty(listUserInfo[0].username));

            Assert.IsFalse(String.IsNullOrEmpty(listUserInfo[1].displayName));
            Assert.IsFalse(String.IsNullOrEmpty(listUserInfo[1].username));

            Assert.IsFalse(String.IsNullOrEmpty(listUserInfo[2].displayName));
            Assert.IsFalse(String.IsNullOrEmpty(listUserInfo[2].username));
        }

        [TestMethod]
        public void SearchUsers_forGivenSearchQuery_correctlyReturnsListOfUsers()
        {
            var query = config.GetSection(this.GetType().Name).GetSection("test_search_user_query").Value;
            var sut = new UserClient(symBotClient);

            UserSearchResult searchUsers = sut.searchUsers(query, false, 0, 10, null);

            Assert.IsNotNull(searchUsers);
            Assert.IsTrue(searchUsers.users.Count >= 1);
        }
    }
}
