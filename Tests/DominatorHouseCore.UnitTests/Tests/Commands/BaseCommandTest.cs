using DominatorHouseCore.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Input;
using FluentAssertions;
using System;

namespace DominatorHouseCore.UnitTests.Tests.Commands
{
    [TestClass]
    public class BaseCommandTest
    {
        ICommand _cmd;
        bool? isExecuted;
        string output = string.Empty;
        [TestMethod]
        public void output_Should_be_Executed_if_isExecuted_is_true()
        {
            isExecuted = true;
            _cmd = new BaseCommand<bool>(CanExecutedCmd, ExecuteCmd);
            if (_cmd.CanExecute(null))
                _cmd.Execute(null);
            output.Should().Be("Executed");
        }
        [TestMethod]
        public void output_Should_be_Empty_string_if_isExecuted_is_false()
        {
            isExecuted = false;
            _cmd = new BaseCommand<bool>(CanExecutedCmd, ExecuteCmd);
            if (_cmd.CanExecute(null))
                _cmd.Execute(null);
            output.Should().Be(string.Empty);
        }
        [TestMethod]
        public void Should_throw_InvalidOperationException_if_isExecuted_is_null()
        {
            isExecuted = null;
            _cmd = new BaseCommand<bool>(CanExecutedCmd, ExecuteCmd);
            Assert.ThrowsException<InvalidOperationException>(() => _cmd.CanExecute(null));
        }
        private void ExecuteCmd(object obj)
        {
            output = "Executed";
        }

        private bool CanExecutedCmd(object arg)
        {
            return (bool)isExecuted;
        }
    }
}
