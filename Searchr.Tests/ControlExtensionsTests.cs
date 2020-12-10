using NUnit.Framework;
using System;
using Searchr.UI;

namespace Searchr.Tests
{
    [TestFixture]
    public class ControlExtensionsTests
    {
        [TestCase("too short to truncate", 100, "too short to truncate")]
        [TestCase("12345", 0, "…")]
        [TestCase("12345", 3, "123…")]
        [TestCase("12345", 5, "12345")]
        [TestCase("  trims text   ", 10, "trims text")]
        [TestCase("  trims text   ", 9, "trims tex…")]
        [TestCase(null, 5, "")]
        [TestCase("", 0, "")]
        [TestCase(" ", 0, "")] // space
        [TestCase("     ", 1, "")] // 5 spaces
        [TestCase(" ", 0, "")] // tab
        public void Truncate_TruncatesStrings(string input, int size, string expected)
        {
            var actual = ControlExtensions.Truncate(input, size);
            Assert.AreEqual(expected, actual);
        }

    }
}
