using System;
using Xunit;
using Bowling_Game_Score_Tracker.ViewModels;
using Bowling_Game_Score_Tracker.Models;

namespace BowlingGameScoreTracker.Tests
{
    public class BowlingGameUnitTests
    {

        //User should be able to make inputs in app
        [Fact]
        public void CanInputsBeMade_ShouldBeTrueByDefault() {
            BowlingScoreTrackerViewModel bowlingVM = new BowlingScoreTrackerViewModel();
            
            // Arrange
            bool expected = true;

            // Act
            bool actual = bowlingVM.CanInputBeMade;

            // Test
            Assert.Equal (expected, actual);
        }

        // Game should only have ten frames
        [Fact]
        public void BowlingFrames_ShouldAlwaysHaveALengthOfTen () {
            BowlingScoreTrackerViewModel bowlingVM = new BowlingScoreTrackerViewModel();

            // Arrange
            int expected = 10;

            // Act
            int actual = bowlingVM.BowlingFrames.Length;

            // Test
            Assert.Equal (expected, actual);
        }

        // There should be no null members in bowling frames array
        [Fact]
        public void BowlingFrames_ShouldNotHaveAnNullOrUndefinedEntries () {
            BowlingScoreTrackerViewModel bowlingVM = new BowlingScoreTrackerViewModel();

            // Arrange
            bool expected = false;

            // Act
            bool foundEmpty = false;
            foreach (BowlingFrameModel frame in bowlingVM.BowlingFrames) {
                System.Type frameType = frame.GetType ();
                if (frameType != typeof (BowlingFrameModel)) { foundEmpty = true; }
            }
            bool actual = foundEmpty;

            // Test
            Assert.Equal (expected, actual);
        }

        // user should be able add a score to the game
        [Fact]
        public void HandleUserInput_ShouldTrackUsersScore () {
            BowlingScoreTrackerViewModel bowlingVM = new BowlingScoreTrackerViewModel();
            
            // Arrange
            int expected = 5;

            // Act
                // Sending a score to the first cell of the first frame
            bowlingVM.HandleUserInput (0, 0, "5");
            int actual = bowlingVM.BowlingFrames [0].Scores [0];

            // Test
            Assert.Equal (expected, actual);
        }

        // App should track which frame they are in
        [Fact]
        public void HandleUserInput_ShouldTrackFrameProgress () {
            BowlingScoreTrackerViewModel bowlingVM = new BowlingScoreTrackerViewModel();

            // Arrange
            int expected = 1;

            // Act
                // Sending a few scores to the game so it ups where it thinks the user is
            bowlingVM.HandleUserInput (0, 0, "x");
            bowlingVM.HandleUserInput (1, 0, "x");
            int actual = bowlingVM.CurrentlyPlayingFrameIndex;

            // Test
            Assert.Equal (expected, actual);
        }

        // App should prevent the user from skipping a frame
        [Fact]
        public void HandleUserInput_ShouldPreventUserFromSkippingAFrame () {
            BowlingScoreTrackerViewModel bowlingVM = new BowlingScoreTrackerViewModel();

            // Arrange
            int expected = 0;

            // Act
                // Sending a score too far ahead in the game
            bowlingVM.HandleUserInput (2, 0, "x");
            int actual = bowlingVM.CurrentlyPlayingFrameIndex;

            // Test
            Assert.Equal(expected, actual);
        }

        // A strike is worth 10 plus the next two shots
        [Fact]
        public void HandleUserInput_StrikeShouldBeWorthTenPlusTheNextTwoShots () {
            BowlingScoreTrackerViewModel bowlingVM = new BowlingScoreTrackerViewModel();

            // Arrange
            int expected = 14;

            // Act
            bowlingVM.HandleUserInput (0, 0, "x");
            bowlingVM.HandleUserInput (1, 0, "3");
            bowlingVM.HandleUserInput (1, 1, "1");
            int actual = bowlingVM.BowlingFrames [0].ProcessedScore;

            // Test
            Assert.Equal (expected, actual);
        }
        
        // Three strikes in a row should cause the earliest frame of a set of 3 to be worth 30 points
        [Fact]
        public void HandleUserInput_ThreeStrikesInARowShouldBeWorthThirtyPoints () {
            BowlingScoreTrackerViewModel bowlingVM = new BowlingScoreTrackerViewModel();

            // Arrange
            int expected = 30;

            // Act
            bowlingVM.HandleUserInput (0, 0, "x");
            bowlingVM.HandleUserInput (1, 0, "x");
            bowlingVM.HandleUserInput (2, 0, "x");
            int actual = bowlingVM.BowlingFrames [0].ProcessedScore;

            // Test
            Assert.Equal (expected, actual);
        }

        // A Spare should be worth 10 plus the next shot
        [Fact]
        public void HandleUserInput_ASpareShouldBeWorthTenPlusTheNextShot () {
            BowlingScoreTrackerViewModel bowlingVM = new BowlingScoreTrackerViewModel();

            // Arrange
            int expected = 20;

            // Act
            bowlingVM.HandleUserInput (0, 0, "5");
            bowlingVM.HandleUserInput (0, 1, "/");
            bowlingVM.HandleUserInput (1, 0, "5");
            int actual = bowlingVM.BowlingFrames [0].ProcessedScore;

            // Test
            Assert.Equal (expected, actual);
        }

        // Reseting the game should clear the scores
        [Fact]
        public void Reset_ShouldClearOutAllScores () {
            BowlingScoreTrackerViewModel bowlingVM = new BowlingScoreTrackerViewModel();

            // Arrange
            int expected = 0;

            // Act
            bowlingVM.HandleUserInput (0, 0, "x");
            bowlingVM.HandleUserInput (1, 0, "x");
            bowlingVM.HandleUserInput (2, 0, "x");
            bowlingVM.HandleUserInput (3, 0, "x");
            bowlingVM.HandleUserInput (4, 0, "x");
            bowlingVM.HandleUserInput (5, 0, "x");
            bowlingVM.HandleUserInput (6, 0, "x");
            bowlingVM.HandleUserInput (7, 0, "x");
            bowlingVM.HandleUserInput (8, 0, "x");
            bowlingVM.HandleUserInput (9, 0, "x");
            bowlingVM.HandleUserInput (9, 1, "x");
            bowlingVM.HandleUserInput (9, 2, "x");
            bowlingVM.Reset ();
            int actual = bowlingVM.BowlingFrames [9].CombinedScore;

            // Test
            Assert.Equal (expected, actual);
        }
    }
}