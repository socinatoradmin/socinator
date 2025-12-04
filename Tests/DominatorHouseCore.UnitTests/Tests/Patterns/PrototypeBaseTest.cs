using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DominatorHouseCore.Patterns;

namespace DominatorHouseCore.UnitTests.Tests.Patterns
{
    [TestClass]
    public class PrototypeBaseTest
    {
        [TestMethod]
        public void Should_not_equal_SerializableStudent_object_and_deepcloned_object_if_object()
        {
            var student = new SerializableStudent("Harsh", 100);
            var clonedStudent = student.DeepClone();
            student.Name = "Kumar";
            student.Marks = 200;
            clonedStudent.Should().NotBe(student);
            clonedStudent.Name.Should().Be("Harsh");
            clonedStudent.Marks.Should().Be(100);
        }
        [TestMethod]
        public void Should_return_null_after_deepclone_of_null_object_if_object_is_serilizable()
        {
            SerializableStudent student = null;
            var clonedStudent = PrototypeBase.DeepClone(student);
            clonedStudent.Should().BeNull();
        }
        [TestMethod]
        public void Should_return_null_after_deepcloned_of_NonSerializableStudent_object()
        {
            var student = new NonSerializableStudent("Harsh", 100);
            var clonedStudent = student.DeepClone();
            clonedStudent.Should().BeNull();
        }
    }
    [Serializable]
    class SerializableStudent
    {
        public string Name;
        public int Marks;
        public SerializableStudent(string Name, int Marks)
        {
            this.Name = Name;
            this.Marks = Marks;
        }
    }
    class NonSerializableStudent
    {
        public string Name;
        public int Marks;
        public NonSerializableStudent(string Name, int Marks)
        {
            this.Name = Name;
            this.Marks = Marks;
        }
    }
}
