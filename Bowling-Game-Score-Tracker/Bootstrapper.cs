using Bowling_Game_Score_Tracker.ViewModels;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bowling_Game_Score_Tracker
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper () {
            Initialize ();
        }

        protected override void OnStartup(object sender, StartupEventArgs e) {
            // Caliburn library is setting up the relation between the BowlingScoreTracker View and ViewModel for me
            DisplayRootViewFor<BowlingScoreTrackerViewModel>();
        }
    }
}
