﻿/*
 * Copyright (C) 2023 IKTSolution
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */

using IdeaIncubatorBlazor.Services.Users;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace IdeaIncubator.Tests.Unit.Services.Users
{
    class UserServiceTest
    {
        [TestFixture]
        public class Login
        {
            private IUserService _userService;
            private IdeaIncubatorBlazor.Models.IdeaIncubatorDbContext _dbContext;
            private IRoleService _roleService;

            [SetUp]
            public void SetUp()
            {
                _dbContext = new IdeaIncubatorBlazor.Models.IdeaIncubatorDbContext();
                _roleService = new RoleService(_dbContext, null);
                _userService = new UserService(_dbContext, null, _roleService);
            }

            [Test]
            public void Login_Blanks_Returns_0()
            {
                Assert.AreEqual(_userService.LoginUser("", ""), 0);
            }

            [Test]
            public void Login_Invalid_User_Returns_0()
            {
                Assert.AreEqual(_userService.LoginUser("bob", "9999999999999"), 0);
            }

            [Test]
            public void Login_Is_Case_Sensitive()
            {
                bool isValid;
                isValid = _userService.LoginUser("testaccount", "12345") == 0;
                isValid = isValid && _userService.LoginUser("TestAccount", "12345") > 0;
                Assert.IsTrue(isValid);
            }
        }
    }
}
