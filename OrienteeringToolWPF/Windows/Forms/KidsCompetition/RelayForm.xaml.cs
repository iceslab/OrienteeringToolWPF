using OrienteeringToolWPF.DAO.Implementation;
using OrienteeringToolWPF.Model;
using System.Windows;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    /// <summary>
    /// Interaction logic for KCRelayForm.xaml
    /// </summary>
    public partial class RelayForm : Window
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
            if (FormToObject())
            {
                var db = MainWindow.GetDatabase();
                db.Relays.Upsert(relay);
                //RelayDAO dao = new RelayDAO();
                //if (relay.Id == null)
                //    dao.insert(relay);
                //else
                //    dao.update(relay);

                Close();
            }
            else
            {
                MessageBox.Show(this, "Nieprawidłowe dane", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAndNextB_Click(object sender, RoutedEventArgs e)
        {
            if (FormToObject())
            {
                var db = MainWindow.GetDatabase();
                db.Relays.Upsert(relay);
                //RelayDAO dao = new RelayDAO();
                //if (relay.Id == null)
                //    dao.insert(relay);
                //else
                //    dao.update(relay);

                relay = new Relay();

                NameTB.Text = "";
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
                relay.Name = NameTB.Text;
                return true;
            }
            return false;
        }

        private void ObjectToForm()
        {
            NameTB.Text = relay.Name;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(NameTB.Text))
                return false;
            return true;
        }
    }
}
