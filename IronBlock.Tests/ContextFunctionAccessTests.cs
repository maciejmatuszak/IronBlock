// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronBlock.Tests
{
    [TestClass]
    public class ContextFunctionAccessTests
    {
        [TestMethod]
        public void Test_VariableAccess_Empty()
        {
            var ctx = new Context();
            Assert.AreEqual(0, ctx.GetLocalFunctionNames().Count);
        }

        [TestMethod]
        public void Test_Hierarchic_set_overrides()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            parent.SetLocalFunction("f1", "fp");
            child.SetLocalFunction("f1", "fc");
            
            Assert.AreEqual("fc", child.GetFunction("f1"));
            Assert.AreEqual("fp", parent.GetFunction("f1"));
        }
        
        [TestMethod]
        public void Test_Hierarchic_get()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            parent.SetLocalFunction("f1", "fp");
            
            Assert.AreEqual("fp", child.GetFunction("f1"));
            Assert.AreEqual("fp", parent.GetFunction("f1"));
        }
        
        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void Test_Hierarchic_missing()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            parent.SetLocalFunction("f1", "fp");
            child.GetFunction("missing");
        }
        
        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void Test_Hierarchic_generic_get()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            parent.SetLocalFunction("f1", "fp");
            child.GetLocalFunction("f1");
        }
        
        [TestMethod]
        public void Test_Hierarchic_local_missing()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            parent.SetLocalFunction("f1", "fp");
            Assert.AreEqual("fp", child.GetFunction<string>("f1"));
        }
        
        [TestMethod]
        public void Test_Hierarchic_does_exists()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            child.SetLocalFunction("f1", "fp");
            Assert.IsTrue(child.DoesFunctionExists("f1"));
            Assert.IsFalse(parent.DoesFunctionExists("f1"));
        }
    }
}