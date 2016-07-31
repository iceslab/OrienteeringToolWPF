using OrienteeringToolWPF.DAO.Implementation;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System.Collections.Generic;
using System.Windows;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    public partial class RouteStepForm : Window, IForm
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
            var errors = FormToObject();
            if (errors.HasErrors() == false)
            {
                if(!noSave)
                {
                    var db = MainWindow.GetDatabase();
                    db.RouteSteps.Upsert(routeStep);
                }
                
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
            OrderCB.SelectedIndex = (int)(routeStep.Order - 1);
            CodeTB.Text = routeStep.Code.ToString();
        }

        public ErrorList FormToObject()
        {
            var errors = ValidateForm();
            if (errors.HasErrors() == false)
            {
                routeStep.Code = long.Parse(CodeTB.Text);
            }
            return errors;
        }

        public ErrorList ValidateForm()
        {
            var errors = new ErrorList();
            if (OrderCB.SelectedItem == null)
                errors.Add(Properties.Resources.RouteStepOrder, Properties.Resources.InvalidOrderError);
            long n;
            if (!long.TryParse(CodeTB.Text, out n))
                errors.Add(Properties.Resources.RouteStepCode, Properties.Resources.NotANumberError);
            return errors;
        }

        private void PopulateOrderCB(long routeId, bool insertMode)
        {
            var db = MainWindow.GetDatabase();
            // TODO: Take into account records added but not yet inserted into database
            var routeStepsCount = db.RouteSteps.GetCount(db.RouteSteps.RouteId == routeId);
            
                //FindAllByRouteId(routeId).Count;

            //var r = new RouteStepDAO();
            //var routeStepsCount = r.findAllByRouteID(routeId).Count;

            // When inserting there should be one place more for new item
            if (insertMode)
                ++routeStepsCount;

            for(int i = 1; i <= routeStepsCount; ++i)
                OrderCB.Items.Add(i);
        }

    }
}
