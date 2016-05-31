﻿using UnityEngine;

using NUnit.Framework;
using Rhino.Mocks;
using System;

namespace ironsource {
    namespace test {        
        [TestFixture()]
        public class ResponseTest {
            [Test()]
            public void TestResponseProperties() {
                string expectedError = "test error";
                string expectedData = "test data";
                int expectedStatus = 200;

                Response response = new Response(expectedError, expectedData, expectedStatus);

                Assert.AreEqual(expectedError, response.error);
                Assert.AreEqual(expectedData, response.data);
                Assert.AreEqual(expectedStatus, response.status);
            }           
        }
    }
}

