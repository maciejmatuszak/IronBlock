using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronBlock.Tests
{
    [TestClass]
    public class ContextVariableAccessTests
    {
        [TestMethod]
        public void Test_VariableAccess_Empty()
        {
            var ctx = new Context();
            Assert.AreEqual(0, ctx.GetLocalVariableNames().Count);
        }

        [TestMethod]
        public void Test_VariableAccess_Hierarchic()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            parent.SetVariable("v", 1);
            child.SetVariable("v", 2);

            Assert.AreEqual(2, parent.GetLocalVariable("v"));

            Assert.AreEqual(2, parent.GetVariable("v"));
            Assert.AreEqual(2, child.GetVariable("v"));


            Assert.AreEqual(0, child.GetLocalVariableNames().Count);
            Assert.AreEqual(1, parent.GetLocalVariableNames().Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_VariableAccess_Hierarchic_GetLocalVariable_notExists()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            parent.SetVariable("v", 1);
            child.SetVariable("v", 2);

            child.GetLocalVariable("v");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_VariableAccess_Hierarchic_notExists()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            parent.SetVariable("v", 1);
            child.SetVariable("v", 2);

            child.GetVariable("vv");
        }

        [TestMethod]
        public void Test_VariableAccess_Hierarchic_Local_Override()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            parent.SetLocalVariable("v", 1);
            child.SetLocalVariable("v", 2);

            Assert.AreEqual(1, parent.GetLocalVariable("v"));
            Assert.AreEqual(2, child.GetLocalVariable("v"));

            Assert.AreEqual(1, parent.GetVariable("v"));
            Assert.AreEqual(2, child.GetVariable("v"));


            Assert.AreEqual(1, child.GetLocalVariableNames().Count);
            Assert.AreEqual(1, parent.GetLocalVariableNames().Count);
        }

        [TestMethod]
        public void Test_VariableAccess_parentIsolation()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            child.SetVariable("v", 1);

            Assert.AreEqual(1, child.GetLocalVariable("v"));
            Assert.AreEqual(1, child.GetVariable("v"));


            Assert.AreEqual(1, child.GetLocalVariableNames().Count);
            Assert.AreEqual(0, parent.GetLocalVariableNames().Count);
        }

        [TestMethod]
        public void Test_VariableAccess_GetVariableWithDefault()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            parent.SetVariable("v", 1);

            Assert.AreEqual(1, parent.GetVariable("v", 5));
            Assert.AreEqual(1, child.GetVariable("v", 5));
            
            Assert.AreEqual(5, parent.GetVariable("vv", 5));
            Assert.AreEqual(5, child.GetVariable("vv", 5));

        }

        [TestMethod]
        public void Test_VariableAccess_OverrideVariables()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            parent.SetVariable("p", "p");
            child.SetVariable("c", "c");

            Assert.AreEqual("p", parent.GetLocalVariable("p"));
            Assert.AreEqual("c", child.GetLocalVariable("c"));
            parent.OverrideVariables(new Dictionary<string, object>() {{"pp", "pp"}});

            Assert.AreEqual("pp", parent.GetVariable("pp"));
            Assert.AreEqual("c", child.GetVariable("c"));
        }

        [TestMethod]
        public void Test_VariableAccess_DoesExists()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            parent.SetVariable("p", "p");
            child.SetVariable("c", "c");

            Assert.IsTrue(parent.DoesVariableExists("p"));
            Assert.IsTrue(child.DoesVariableExists("c"));
            
            Assert.IsFalse(parent.DoesVariableExists("c"));
            Assert.IsFalse(parent.DoesVariableExists("pp"));
            Assert.IsFalse(child.DoesVariableExists("cc"));
            
            Assert.IsTrue(child.DoesVariableExists("p"));
            Assert.IsTrue(child.DoesVariableExists("c"));
        }
        
        [TestMethod]
        public void Test_VariableAccess_GenericAccess()
        {
            var parent = new Context();
            var child = parent.CreateChildContext();
            parent.SetVariable("p", 2);
            child.SetVariable("c", "3");

            Assert.AreEqual(2, parent.GetVariable("p"));
            Assert.AreEqual(2, parent.GetVariable<int>("p"));
           
            Assert.AreEqual("3", child.GetVariable("c"));
            Assert.AreEqual("3", child.GetVariable<string>("c"));
            
            Assert.AreEqual("33", child.GetVariable<string>("cc", "33"));
        }
    }
}
