using OrienteeringToolWPF.DAO.Implementation;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System.Windows;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    /// <summary>
    /// Interaction logic for KCRelayForm.xaml
    /// </summary>
    public partial class RelayForm : Window, IForm
    {
        public Relay relay { get; private set; }

        public RelayForm()
        {
            InitializeComponent();
            relay = new Relay();
        }

        public RelayForm(Relay r)
        {
            InitializeComponent();
            relay = r;
            ObjectToForm();
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            if (errors.HasErrors() == false)
            {
                var db = MainWindow.GetDatabase();
                db.Relays.Upsert(relay);

                Close();
            }
            else
            {
                MessageUtils.ShowValidatorErrors(this, errors);
            }
        }

        private void SaveAndNextB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            if (errors.HasErrors() == false)
            {
                var db = MainWindow.GetDatabase();
                db.Relays.Upsert(relay);
                relay = new Relay();

                NameTB.Text = "";
            }
            else
            {
                MessageUtils.ShowValidatorErrors(this, errors);
            }
        }

        public void ObjectToForm()
        {
            NameTB.Text = relay.Name;
        }

        public ErrorList FormToObject()
        {
            var errors = ValidateForm();
            if (errors.HasErrors() == false)
            {
                relay.Name = NameTB.Text;
            }
            return errors;
        }

        public ErrorList ValidateForm()
        {
            var errors = new ErrorList();
            if (string.IsNullOrWhiteSpace(NameTB.Text))
                errors.Add(Properties.Resources.RelayName, Properties.Resources.NullOrEmptyError);
            return errors;
        }
    }
}
