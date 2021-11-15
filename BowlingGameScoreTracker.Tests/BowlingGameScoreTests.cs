using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bowling_Game_Score_Tracker.ViewModels;
using Xunit;

namespace BowlingGameScoreTracker.Tests
{
    public class BowlingGameScoreTests
    {
        BowlingScoreTrackerViewModel bowlingGame = new BowlingScoreTrackerViewModel();

        [Fact]
        public Task CanInputsBeMAde_ShouldBeTrueByDefault () {
            Debug.WriteLine ("Running test?");

            // Arrange
            bool expected = true;

            // Act
            bool actual = bowlingGame.CanInputBeMade;

            // Assert
            return Assert.Equal (expected, actual);
        }
    }
}
