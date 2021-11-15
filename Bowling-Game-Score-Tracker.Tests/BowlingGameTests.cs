using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bowling_Game_Score_Tracker.ViewModels;
using Xunit;

namespace Bowling_Game_Score_Tracker.Tests
{
    public class BowlingGameTests
    {
        [Fact]
        public void Should_BeAbleToMakeInputsByDefault () {
            // Arrange
            bool expected = true;

            // Act
            bool actual = BowlingScoreTrackerViewModel.CanInputBeMade;

            // Assert
            Assert.Equal (expected, actual);
        }
    }
}
