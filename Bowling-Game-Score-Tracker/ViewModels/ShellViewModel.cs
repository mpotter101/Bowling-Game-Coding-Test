using Bowling_Game_Score_Tracker.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bowling_Game_Score_Tracker.ViewModels
{
    public class ShellViewModel : Screen
    {
        private string _firstName = "Tim";
        private string _lastName = "The Wizard";
        private BindableCollection<PersonModel> _people = new BindableCollection<PersonModel>();
        private PersonModel _selectedPerson;

        public ShellViewModel() {
            People.Add (new PersonModel { FirstName = "Tim", LastName = "the Wizard" });
            People.Add (new PersonModel { FirstName = "King", LastName = "Arthur" });
            People.Add (new PersonModel { FirstName = "White", LastName = "Rabbit" });

            _selectedPerson = People [0];
        }

        public string FirstName {
            get { return _firstName; }
            set { 
                _firstName = value; 
                NotifyOfPropertyChange( () => FirstName );
                NotifyOfPropertyChange( () => FullName );
            }
        }

        public string LastName {
            get { return _lastName; }
            set { 
                _lastName = value;
                NotifyOfPropertyChange( () => LastName );
                NotifyOfPropertyChange( () => FullName );
            }
        }

        public string FullName {
            get { return $"{FirstName} {LastName}"; }
        }

        
        public BindableCollection<PersonModel> People {
            get { return _people; }
            set { _people = value; }
        }

        
        public PersonModel SelectedPerson {
            get { return _selectedPerson; }
            set { 
                _selectedPerson = value; 
                NotifyOfPropertyChange ( () => SelectedPerson );
            }
        }

        public bool CanClearText (string firstName, string lastName) {
            return (!String.IsNullOrWhiteSpace(firstName) || !String.IsNullOrWhiteSpace(lastName));
        }

        public void ClearText (string firstName, string lastName) {

            if (CanClearText(firstName, lastName)) {
                FirstName = "";
                LastName = "";
            }
        }

    }
}
