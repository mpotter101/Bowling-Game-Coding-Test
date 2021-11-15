using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Bowling_Game_Score_Tracker.Models;
using Caliburn.Micro;

// ---
// This block of comments only exist as part of expectation of this app being a test. If this were going to a client, such notes would be taken elsewhere, but I figured it would grant some transparency to my line of thinking when reviewing the object structure

// Missing Features:
//      Auto-focusing cells in a frame once a score is entered or the reset button is hit

// Questions:
//      How to change focus of an element with the MVVM structure? I saw a few posts using the Code-Behind class, but that seems to be a poor practice for MVVM...
//      How to best go about generating templated objects in an environment where the the view should not be directly linked to?
//      ...More generically, what are best practices for manipulating XAML? Web-Dev has a concept of the DOM and allows the freedom of generating structure in code without tying it to the look thanks to CSS. Structure can be manipulated without locking an app into a specific appearance, certainly there is a concept in WPF+MVVM akin to that?
// ---

namespace Bowling_Game_Score_Tracker.ViewModels
{
    public class BowlingScoreTrackerViewModel : Screen
    {
        private string _validNumbers = "0123456789";
        private const string STRIKE_CHAR = "x";
        private const string SPARE_CHAR = "/";

        private BowlingFrameModel[] _bowlingFrames = new BowlingFrameModel[10] {
            new BowlingFrameModel (),
            new BowlingFrameModel (),
            new BowlingFrameModel (),
            new BowlingFrameModel (),
            new BowlingFrameModel (),
            new BowlingFrameModel (),
            new BowlingFrameModel (),
            new BowlingFrameModel (),
            new BowlingFrameModel (),
            new BowlingFrameModel ()
        };
        private int _furthestFrameModified = 0;

        private bool _processingInput = false;

        public bool CanInputBeMade { get { return !_processingInput; } }

        public BowlingFrameModel[] BowlingFrames {
            get { return _bowlingFrames; }
        }

        public void HandleUserInput(int frameIndex, int scoreIndex, string value) { 
            // Prevent more than input from being handled at a time
            if (_processingInput) { return; }
            _processingInput = true;
            NotifyOfPropertyChange ( () => CanInputBeMade );

            // check user input. It should follow this criteria:
            //  VALID INPUT VALUES
            //    * an "x" or an "X" for a value for a STRIKE
            //    * an "/" for a spare
            //    * the numbers 0-9 for each shot if it is not a strike or a spare
            //    * the cursor should auto-advance to the next shot (or next frame in case of a strike)

            bool validInput = (
                // The string is less than 2 characters long
                value.Length < 2 && 
                    // and The string contains any of the white listed numbers OR is the strike or spare character
                    ( value.IndexOfAny (_validNumbers.ToCharArray ()) != -1 || 
                        (value.ToLower () == STRIKE_CHAR || value == SPARE_CHAR) 
                    )
            );


            // Make sure user isn't trying to skip frames
            if (frameIndex > _furthestFrameModified + 1) { validInput = false; }
            
            // Checks for frames that aren't the last...
            if (frameIndex < BowlingFrames.Length - 1) {
                // check to see if the user is trying to put a strike in the second cell
                if (value.ToLower () == STRIKE_CHAR && scoreIndex == 1) { validInput = false; }
            }
            
            // Make sure the user isn't trying to put a spare in the first cell
            if (value == SPARE_CHAR && scoreIndex == 0) { validInput = false; }

            if (validInput) {
                // Users input was valid!

                // Try to convert the string into an integer
                try {
                    // If this fails, the user put in one of the whitelisted special characters
                    int scoreVal = int.Parse (value);

                    // Make sure the user isn't trying to input a combined value greater than 10
                    bool validScore = true;
                    if (scoreIndex == 1) {
                        validScore = (BowlingFrames [frameIndex].Scores [0] + scoreVal) < 10;
                    }

                    if (validScore) {
                        SetScoreOfFrame (frameIndex, scoreIndex, scoreVal, BowlingFrameModel.OPEN, value);
                    }
                }

                // If that fails it means the user is trying to input a strike or spare character
                catch (FormatException invalidCast) {

                    // Assume its a strike...
                    string scoreType = BowlingFrameModel.STRIKE;

                    // ... unless it isn't
                    if (value == SPARE_CHAR) { scoreType = BowlingFrameModel.SPARE; }

                    // Set the value of the modified frame
                    SetScoreOfFrame (frameIndex, scoreIndex, BowlingFrameModel.SPECIAL_SCORE_VALUE, scoreType, value);
                }

                // Figure out the new totals of the game
                FigureOutScoreOfGame ();
            }

            // Update UI based on state of bowling frames
            NotifyOfPropertyChange (() => BowlingFrames);

            _processingInput = false;
            NotifyOfPropertyChange ( () => CanInputBeMade );
        }
        
        public void Reset () {
            // Pressing the RESET button should clear all scores and set the cursor to the very first input field in Frame 1.

            // TODO: Figure out how to set the focus of an element from here
            foreach (BowlingFrameModel frame in BowlingFrames) {
                frame.DisplayedCombiendScore = "";

                for (int index = 0; index < frame.Scores.Length; index++) {
                    frame.Scores [index] = 0;
                    frame.DisplayedValues [index] = "";
                }
            }
            _furthestFrameModified = 0;
            NotifyOfPropertyChange (() => BowlingFrames);
        }

        private void SetScoreOfFrame (int frameIndex, int scoreIndex, int scoreValue, string scoreType, string displayText) {
            // Update frame with all the data we need to calculate its score
            BowlingFrameModel targetFrame = BowlingFrames [frameIndex];
            targetFrame.Scores [scoreIndex] = scoreValue;
            targetFrame.ScoreTypes [scoreIndex] = scoreType;
            targetFrame.DisplayedValues [scoreIndex] = displayText;

            // Track user's progress
            if (frameIndex > _furthestFrameModified) { _furthestFrameModified = frameIndex; }
        }

        public int FigureOutScoreOfGame ()  {
            //* Strike - if you knock down all 10 pins in the first shot of a frame, this is a strike.  A strike is worth 10 points PLUS THE SUM OF YOUR NEXT TWO SHOTS.
            //* Spare - If you knock down all 10 pins using both shots of a frame, this is a spare.  A spare is worth 10 points PLUS THE SUM OF YOUR NEXT ONE SHOT.
            //* Open frame - If you do not knock down all 10 pins using both shots of your frame (9 or fewer pins knocked down), this is an open frame and is worth the number of pins knocked down.

            BowlingFrameModel curFrame, nextFrame, theFrameAfterNext;
            int end = _furthestFrameModified;
            for (int index = 0; index <= end; index++) {
                curFrame = BowlingFrames [index];
                nextFrame = null;
                theFrameAfterNext = null;

                // Determine the score of this frame as an individual. This also handles OPEN frames
                curFrame.ProcessedScore = curFrame.Scores [0] + curFrame.Scores [1] + curFrame.Scores [2];

                // Check to see if there are any frames ahead of this one
                    //  The last frame will never pass this conditional
                if (index < _furthestFrameModified) {
                    // check to see if there is a strike or spare in this frame
                    if (curFrame.ScoreTypes [0] != BowlingFrameModel.OPEN || curFrame.ScoreTypes [1] != BowlingFrameModel.OPEN) {
                        nextFrame = BowlingFrames [index + 1];

                        if (index + 2 <= _furthestFrameModified) { theFrameAfterNext = BowlingFrames [index + 2]; } 

                        // Check for and Handle a strike
                        //      data validation stops strikes from being put into second cell of frames that aren't the last
                        if (curFrame.ScoreTypes [0] == BowlingFrameModel.STRIKE) {
                            // check the next frame's first score for a strike, if it is, try and grab the first score of the frame after that
                            bool nextFrameHasStrike = nextFrame.ScoreTypes [0] == BowlingFrameModel.STRIKE;

                            // If no strike is detected, or we can't reach the frame after next, just calcuate what is there
                            if (nextFrameHasStrike == false || theFrameAfterNext == null) {
                                curFrame.ProcessedScore += nextFrame.Scores [0] + nextFrame.Scores [1];
                            }
                            // If a strike is detected and we can reach the frame after next, grab those values
                            else if (nextFrameHasStrike == true && theFrameAfterNext != null) {
                                curFrame.ProcessedScore += nextFrame.Scores [0] + theFrameAfterNext.Scores [0];
                            }
                        }
                        // Check for a spare
                        //      data validation stops spares from being put in the first cell of frames
                        else if (curFrame.ScoreTypes [1] == BowlingFrameModel.SPARE) {
                            // we found a spare, add the next two scores of the next frame. Frames always at least have a score of zero even if the player hasn't touched them yet
                            curFrame.ProcessedScore += nextFrame.Scores [0] + nextFrame.Scores[1];
                        }
                    }
                }
                // Alright, we've hit the last frame of the game. Congrats warrior.
                else if (index == BowlingFrames.Length - 1) {
                    int finalValue = curFrame.Scores [0] + curFrame.Scores [1];

                    // Only add the the third value if neither of the first two cells are OPEN
                    if (curFrame.ScoreTypes [0] != BowlingFrameModel.OPEN && curFrame.ScoreTypes [1] != BowlingFrameModel.OPEN) {
                        finalValue += curFrame.Scores [2];
                    }

                    curFrame.ProcessedScore = finalValue;
                }

                // Look at the previous frame to determine the combined score so far
                if (index > 0) {
                    curFrame.CombinedScore = curFrame.ProcessedScore + BowlingFrames [index - 1].CombinedScore;
                }
                else {
                    curFrame.CombinedScore = curFrame.ProcessedScore;
                }

                // Display that score
                curFrame.DisplayedCombiendScore = curFrame.CombinedScore.ToString ();
            }

            // Return the combined score so Unit Tests can be confirmed
            return BowlingFrames [_furthestFrameModified].CombinedScore;
        }
    }
}
