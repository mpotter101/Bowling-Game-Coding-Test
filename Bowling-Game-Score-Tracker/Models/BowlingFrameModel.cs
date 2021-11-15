using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bowling_Game_Score_Tracker.Models
{

    public class BowlingFrameModel
    {

        // Scores
        public const string STRIKE = "strike";
        public const string SPARE = "spare";
        public const string OPEN = "open";
        public const int SPECIAL_SCORE_VALUE = 10;
        private int[] _scores = new int[3] { 0, 0, 0 };
        private string[] _scoreTypes = new string[3] { OPEN, OPEN, OPEN };

        // Processed score is the score after taking into consideration future frames when we have special characters
        private int _processedScore = 0;
        // Combined score is the sum of this frame and every frame before it
        private int _combinedScore = 0;
        // Displayed values pair with the score and are injected into the score fields as the user is typing.
        // This allows me to clear the values from the fields when they are reset
        private string[] _displayedValues = new string[3] { "", "", "" };
        private string _displayedCombinedScore = "";

        // Score Get/Set

        public int[] Scores {
            get { return _scores; }
        }

        public string[] ScoreTypes {
            get { return _scoreTypes; }
        }

        public string[] DisplayedValues {
            get { return _displayedValues; }
        }

        public bool ShouldEnableSecondSlot {
            get { return _scoreTypes [0] != STRIKE; }
        }

        public bool ShouldEnableThirdSlot {
            get {
                return (
                    _scoreTypes [0] == STRIKE || 
                    _scoreTypes [1] == STRIKE || 
                    _scoreTypes [1] == SPARE
                    );
            }
        }

        public int ProcessedScore {
            get { return _processedScore; }
            set { _processedScore = value; }
        }

        public int CombinedScore {
            get { return _combinedScore; }
            set { _combinedScore = value; }
        }

        public string DisplayedCombiendScore {
            get { return _displayedCombinedScore; }
            set { _displayedCombinedScore = value; }
        }
    }
}
