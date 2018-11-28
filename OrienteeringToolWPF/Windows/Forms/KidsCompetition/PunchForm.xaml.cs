using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System;
using System.Windows;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    enum SpecialPunchE
    {
        NONE,
        START,
        CHECK,
        FINISH
    }

    public partial class PunchForm : Window, IForm
    {
        public Punch punch { get; private set; }
        public Result result { get; private set; }
        private SpecialPunchE specialPunchType = SpecialPunchE.NONE;
        private bool noSave;

        public PunchForm(bool noSave = false)
        {
            InitializeComponent();
            punch = new Punch();
            result = new Result();
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
            result = r;
            ObjectToForm();
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            if (errors.HasErrors() == false)
            {
                if (!noSave)
                {
                    var db = DatabaseUtils.GetDatabase();
                    switch (specialPunchType)
                    {
                        case SpecialPunchE.NONE:
                            db.Punches.Upsert(punch);
                            break;
                        case SpecialPunchE.START:
                        case SpecialPunchE.CHECK:
                        case SpecialPunchE.FINISH:
                            db.Results.Upsert(result);
                            break;
                        default:
                            break;
                    }
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
            if (punch.Chip != 0)
            {
                ChipTB.Text = punch.Chip.ToString();
            }

            if (punch.Code != 0)
            {
                CodeTB.Text = punch.Code.ToString();
            }

            if (punch.Timestamp != 0)
            {
                TimestampDP.Value = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(punch.Timestamp);
            }
        }

        public ErrorList FormToObject()
        {
            var errors = ValidateForm();
            if (errors.HasErrors() == false)
            {
                long timestamp = Convert.ToInt64(((DateTime)TimestampDP.Value).TimeOfDay.TotalMilliseconds);
                switch (specialPunchType)
                {
                    case SpecialPunchE.NONE:
                        punch.Chip = long.Parse(ChipTB.Text);
                        punch.Code = long.Parse(CodeTB.Text);
                        punch.Timestamp = timestamp;
                        break;
                    case SpecialPunchE.START:
                        result.StartTime = timestamp;
                        break;
                    case SpecialPunchE.CHECK:
                        result.CheckTime = timestamp;
                        break;
                    case SpecialPunchE.FINISH:
                        result.FinishTime = timestamp;
                        break;
                    default:
                        break;
                }
            }
            return errors;
        }

        public ErrorList ValidateForm()
        {
            specialPunchType = SpecialPunchE.NONE;
            var errors = new ErrorList();
            long n;
            if (!long.TryParse(ChipTB.Text, out n))
                errors.Add(Properties.Resources.PunchChip, Properties.Resources.NotANumberError);

            if (TimestampDP.Value == null)
                errors.Add(Properties.Resources.PunchTimestamp, Properties.Resources.InvalidDateError);
            if (!long.TryParse(CodeTB.Text, out n))
            {
                switch (CodeTB.Text)
                {
                    case "START":
                        specialPunchType = SpecialPunchE.START;
                        break;
                    case "CHECK":
                        specialPunchType = SpecialPunchE.CHECK;
                        break;
                    case "FINISH":
                        specialPunchType = SpecialPunchE.FINISH;
                        break;
                    default:
                        errors.Add(Properties.Resources.PunchCode, Properties.Resources.NotANumberError);
                        break;
                }
            }
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
