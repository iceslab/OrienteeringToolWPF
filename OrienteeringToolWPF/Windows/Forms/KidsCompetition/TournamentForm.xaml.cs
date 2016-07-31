using OrienteeringToolWPF.DAO.Implementation;
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
        private Tournament tournament;

        public TournamentForm()
        {
            InitializeComponent();
            tournament = new Tournament();
            StartTimeDP.DefaultValue = DateTime.Now;
        }

        public TournamentForm(Tournament t)
        {
            InitializeComponent();
            tournament = t;
            ObjectToForm();
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            if (errors.HasErrors() == false)
            {
                var db = MainWindow.GetDatabase();
                db.Tournament.Upsert(tournament);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageUtils.ShowValidatorErrors(this, errors);
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
                tournament.CourseType = (CourseEnum)CourseTypeCB.SelectedIndex;
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
            switch ((CourseEnum)CourseTypeCB.SelectedIndex)
            {
                case CourseEnum.START_ON_CHIP:
                case CourseEnum.START_CALCULATED:
                    break;
                default:
                    errors.Add(Properties.Resources.TournamentCourseType, Properties.Resources.InvalidCourseTypeError);
                    break;
            }
            return errors;
        }

    }
}
