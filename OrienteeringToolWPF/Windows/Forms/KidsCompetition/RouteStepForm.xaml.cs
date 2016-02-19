using OrienteeringToolWPF.DAO.Implementation;
using OrienteeringToolWPF.Model;
using System.Collections.Generic;
using System.Windows;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    /// <summary>
    /// Interaction logic for KCRelayForm.xaml
    /// </summary>
    public partial class RouteStepForm : Window
    {
        public RouteStep routeStep { get; private set; }
        private List<Route> routesList;
        private bool noSave;

        public RouteStepForm(bool noSave = false)
        {
            InitializeComponent();
            routeStep = new RouteStep();
            this.noSave = noSave;
        }

        public RouteStepForm(RouteStep r, bool noSave = false)
        {
            InitializeComponent();
            routeStep = r;
            ObjectToForm();
            this.noSave = noSave;
        }

        public RouteStepForm(Route r, bool noSave = false) : this(noSave)
        {
            routeStep.RouteId = (long)r.Id;
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            if (FormToObject())
            {
                if(!noSave)
                {
                    var dao = new RouteStepDAO();
                    if (routeStep.Id == null)
                        dao.insert(routeStep);
                    else
                        dao.update(routeStep);
                }
                
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
                routeStep.Code = long.Parse(CodeTB.Text);
                return true;
            }
            return false;
        }

        private void ObjectToForm()
        {
            PopulateRouteIdCB();
            CodeTB.Text = routeStep.Code.ToString();
        }

        private bool ValidateForm()
        {
            long n;
            if (!long.TryParse(CodeTB.Text, out n))
                return false;
            return true;
        }

        private void PopulateRouteIdCB()
        {
            var r = new RouteDAO();
            routesList = r.findAll();
        }

    }
}
