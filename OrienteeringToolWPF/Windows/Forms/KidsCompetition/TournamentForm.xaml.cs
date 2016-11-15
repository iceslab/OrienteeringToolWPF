using OrienteeringToolWPF.Enumerations;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System;
using System.Windows;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    /// <summary>
    /// Interaction logic for KCTournamentWindow.xaml
    /// </summary>
    public partial class TournamentForm : Window, IForm
    {
        public Tournament tournament { get; private set; }
        private bool noSave;

        public TournamentForm(bool noSave = false)
        {
            InitializeComponent();
            tournament = new Tournament();
            StartTimeDP.DefaultValue = DateTime.Now;
            this.noSave = noSave;
        }

        public TournamentForm(Tournament t, bool noSave = false) : this(noSave)
        {
            tournament = t;
            ObjectToForm();
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            if (errors.HasErrors() == false)
            {
                if (noSave == false)
                    Save();
                DialogResult = true;
                Close();
            }
            else
            {
                MessageUtils.ShowValidatorErrors(this, errors);
            }
        }

        public void Save()
        {
            if (tournament != null)
            {
                var db = MainWindow.GetDatabase();
                db.Tournament.Upsert(tournament);
            }
        }

        public void ObjectToForm()
        {
            StartTimeDP.Value = tournament.StartTime;
            NameTB.Text = tournament.Name;
            CourseTypeCB.SelectedIndex = (int)tournament.CourseType;
            DescriptionTB.Text = tournament.Description;
        }

        public ErrorList FormToObject()
        {
            var errors = ValidateForm();
            if (errors.HasErrors() == false)
            {
                tournament.StartTime = (DateTime)StartTimeDP.Value;
                tournament.Name = NameTB.Text;
                tournament.CourseType = (Course)CourseTypeCB.SelectedIndex;
                tournament.Description = DescriptionTB.Text;
            }
            return errors;
        }

        public ErrorList ValidateForm()
        {
            var errors = new ErrorList();
            if (StartTimeDP.Value == null)
                errors.Add(Properties.Resources.TournamentStartTime, Properties.Resources.InvalidDateError);
            if (string.IsNullOrWhiteSpace(NameTB.Text))
                errors.Add(Properties.Resources.TournamentName, Properties.Resources.TournamentName);
            switch ((Course)CourseTypeCB.SelectedIndex)
            {
                case Course.START_ON_CHIP:
                case Course.START_CALCULATED:
                    break;
                default:
                    errors.Add(Properties.Resources.TournamentCourseType, Properties.Resources.InvalidCourseTypeError);
                    break;
            }
            return errors;
        }

    }
}
