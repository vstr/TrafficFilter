using System;
using System.Threading;

using FluentAssertions;

using TrafficFilter.Core;

using Xunit;

namespace TrafficFilter.Tests
{
    public class RequestBufferTests
    {
        [Fact]
        public void SameRequestWithinAllowedTime()
        {
            //Arrange
            var buff = new RequestBuffer(3, TimeSpan.FromSeconds(1));

            //Act
            //Assert
            buff.IsFull("req1").Should().BeFalse();
            buff.IsFull("req1").Should().BeFalse();
            buff.IsFull("req1").Should().BeFalse();
            buff.IsFull("req1").Should().BeTrue();
        }

        [Fact]
        public void DifferentRequestWithinAllowedTime()
        {
            //Arrange
            var buff = new RequestBuffer(2, TimeSpan.FromMilliseconds(10));

            //Act
            //Assert
            buff.IsFull("req1").Should().BeFalse();
            buff.IsFull("req2").Should().BeFalse();
            buff.IsFull("req3").Should().BeFalse();

            buff.IsFull("req1").Should().BeFalse();
            buff.IsFull("req2").Should().BeFalse();

            buff.IsFull("req1").Should().BeTrue();
            buff.IsFull("req1").Should().BeTrue();

            Thread.Sleep(50);

            buff.IsFull("req1").Should().BeFalse();
            buff.IsFull("req2").Should().BeFalse();
        }
    }
}