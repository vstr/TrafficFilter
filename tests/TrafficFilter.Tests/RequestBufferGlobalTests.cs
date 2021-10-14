using System;
using System.Threading;

using FluentAssertions;

using TrafficFilter.Core;

using Xunit;

namespace TrafficFilter.Tests
{
    public class RequestBufferGlobalTests
    {
        [Fact]
        public void SameRequestWithinAllowedTime()
        {
            //Arrange
            var buff = new RequestBufferGlobal(3, TimeSpan.FromSeconds(1));

            //Act
            //Assert
            buff.IsFull().Should().BeFalse();
            buff.IsFull().Should().BeFalse();
            buff.IsFull().Should().BeFalse();
            buff.IsFull().Should().BeTrue();
        }

        [Fact]
        public void RequestsWithinAllowedTimeWithTimeout()
        {
            //Arrange
            var buff = new RequestBufferGlobal(3, TimeSpan.FromMilliseconds(20));

            //Act
            //Assert
            buff.IsFull().Should().BeFalse();
            buff.IsFull().Should().BeFalse();
            buff.IsFull().Should().BeFalse();
            buff.IsFull().Should().BeTrue();

            Thread.Sleep(50);

            buff.IsFull().Should().BeFalse();
            buff.IsFull().Should().BeFalse();
            buff.IsFull().Should().BeFalse();
            buff.IsFull().Should().BeTrue();
        }
    }
}