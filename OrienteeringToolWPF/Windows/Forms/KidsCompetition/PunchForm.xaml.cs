using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System;
using System.Windows;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    public partial class PunchForm : Window, IForm
    {
        public Punch punch { get; private set; }
        private bool noSave;

        public PunchForm(bool noSave = false)
        {
            InitializeComponent();
            punch = new Punch();
            this.noSave = noSave;
        }

        public PunchForm(Punch p, bool noSave = false) : this(noSave)
        {
            punch = p;
            ObjectToForm();
        }

        public PunchForm(Result r, bool noSave = false) : this(noSave)
        {
            punch.Chip = r.Chip;
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            if (errors.HasErrors() == false)
            {
                if (!noSave)
                {
                    var db = MainWindow.GetDatabase();
                    db.Punches.Upsert(punch);
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
            ChipTB.Text = punch.Chip.ToString();
            CodeTB.Text = punch.Code.ToString();
            TimestampDP.Value = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(punch.Timestamp);
        }

        public ErrorList FormToObject()
        {
            var errors = ValidateForm();
            if (errors.HasErrors() == false)
            {
                punch.Chip = long.Parse(ChipTB.Text);
                punch.Code = long.Parse(CodeTB.Text);

                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan span = ((DateTime)TimestampDP.Value - epoch);
                punch.Timestamp = Convert.ToInt64(span.TotalSeconds);
            }
            return errors;
        }

        public ErrorList ValidateForm()
        {
            var errors = new ErrorList();
            long n;
            if (!long.TryParse(ChipTB.Text, out n))
                errors.Add(Properties.Resources.PunchChip, Properties.Resources.NotANumberError);
            if (!long.TryParse(CodeTB.Text, out n))
                errors.Add(Properties.Resources.PunchCode, Properties.Resources.NotANumberError);
            if (TimestampDP.Value == null)
                errors.Add(Properties.Resources.PunchTimestamp, Properties.Resources.InvalidDateError);
            return errors;
        }

        //private void PopulateOrderCB(long chip, bool insertMode)
        //{
        //    var db = MainWindow.GetDatabase();
        //    // TODO: Take into account records added but not yet inserted into database
        //    var punchesCount = db.RouteSteps.GetCount(db.Results.Chip == chip);

        //    //FindAllByRouteId(routeId).Count;

        //    //var r = new RouteStepDAO();
        //    //var routeStepsCount = r.findAllByRouteID(routeId).Count;

        //    // When inserting there should be one place more for new item
        //    if (insertMode)
        //        ++punchesCount;

        //    for (int i = 1; i <= punchesCount; ++i)
        //        OrderCB.Items.Add(i);
        //}

    }
}
