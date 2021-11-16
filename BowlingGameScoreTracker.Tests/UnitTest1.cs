using System;
using Xunit;
using Bowling_Game_Score_Tracker.ViewModels;

namespace BowlingGameScoreTracker.Tests
{
    public class UnitTest1
    {
        BowlingScoreTrackerViewModel bowlingVM = new BowlingScoreTrackerViewModel();

        [Fact]
        public void CanInputsBeMade_ShouldBeTrueByDefault() {
            // Arrange
            bool expected = true;

            // Act
            bool actual = bowlingVM.CanInputBeMade;

            // Test
            Assert.Equal (expected, actual);
        }
    }
}