using OrienteeringToolWPF.DAO.Implementation;
using OrienteeringToolWPF.Model;
using System.Collections.Generic;
using System.Windows;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    public partial class RouteStepForm : Window
    {
        public RouteStep routeStep { get; private set; }
        private bool noSave;

        public RouteStepForm(bool noSave = false)
        {
            InitializeComponent();
            routeStep = new RouteStep();
            this.noSave = noSave;
        }

        public RouteStepForm(RouteStep r, bool noSave = false) : this(noSave)
        {
            routeStep = r;
            PopulateOrderCB(routeStep.RouteId, false);
            ObjectToForm();
        }

        public RouteStepForm(Route r, bool noSave = false) : this(noSave)
        {
            routeStep.RouteId = (long)r.Id;
            PopulateOrderCB(routeStep.RouteId, true);
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
            OrderCB.SelectedIndex = (int)(routeStep.Order - 1);
            CodeTB.Text = routeStep.Code.ToString();
        }

        private bool ValidateForm()
        {
            if (OrderCB.SelectedItem == null)
                return false;
            long n;
            if (!long.TryParse(CodeTB.Text, out n))
                return false;
            return true;
        }

        private void PopulateOrderCB(long routeId, bool insertMode)
        {
            var r = new RouteStepDAO();
            var routeStepsCount = r.findAllByRouteID(routeId).Count;

            // When inserting there should be one place more for new item
            if (insertMode)
                ++routeStepsCount;

            for(int i = 1; i <= routeStepsCount; ++i)
                OrderCB.Items.Add(i);
        }

    }
}
