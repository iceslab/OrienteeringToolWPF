using OrienteeringToolWPF.DAO.Implementation;
using OrienteeringToolWPF.Model;
using System;
using System.Windows;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    /// <summary>
    /// Interaction logic for KCTournamentWindow.xaml
    /// </summary>
    public partial class TournamentForm : Window
    {
        private Tournament tournament;

        public TournamentForm()
        {
            InitializeComponent();
            tournament = new Tournament();
            StartTimeTB.DefaultValue = DateTime.Now;
        }

        public TournamentForm(Tournament t)
        {
            InitializeComponent();
            tournament = t;
            ObjectToForm();
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            if (FormToObject())
            {
                TournamentDAO dao = new TournamentDAO();
                if (tournament.Id == null)
                    dao.insert(tournament);
                else
                    dao.update(tournament);

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show(this, "Nieprawidłowe dane", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool FormToObject()
        {
            if (ValidateForm())
            {
                tournament.StartTime = (DateTime)StartTimeTB.Value;
                tournament.Name = NameTB.Text;
                tournament.CourseType = (CourseEnum)CourseTypeCB.SelectedIndex;
                tournament.Description = DescriptionTB.Text;
                return true;
            }
            return false;
        }

        private void ObjectToForm()
        {
            StartTimeTB.Value = tournament.StartTime;
            NameTB.Text = tournament.Name;
            CourseTypeCB.SelectedIndex = (int)tournament.CourseType;
            DescriptionTB.Text = tournament.Description;
        }

        private bool ValidateForm()
        {
            if (StartTimeTB.Value == null)
                return false;
            if (string.IsNullOrWhiteSpace(NameTB.Text))
                return false;
            switch ((CourseEnum)CourseTypeCB.SelectedIndex)
            {
                case CourseEnum.START_ON_CHIP:
                case CourseEnum.START_CALCULATED:
                    break;
                default:
                    return false;
            }
            return true;
        }

    }
}
