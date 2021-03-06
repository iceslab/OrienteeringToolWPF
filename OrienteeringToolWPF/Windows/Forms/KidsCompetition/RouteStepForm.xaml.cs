﻿using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System;
using System.Collections.Generic;
using System.Windows;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    public partial class RouteStepForm : Window, IForm
    {
        public RouteStep routeStep { get; private set; }
        private int itemsCount;
        private bool noSave;

        public RouteStepForm(int itemsCount, bool noSave = false)
        {
            InitializeComponent();
            routeStep = new RouteStep();
            this.itemsCount = itemsCount;
            this.noSave = noSave;
        }

        public RouteStepForm(RouteStep r, int itemsCount, bool noSave = false) : this(itemsCount, noSave)
        {
            routeStep = r;
            PopulateOrderCB(false);
            ObjectToForm();
        }

        public RouteStepForm(Route r, int itemsCount, bool noSave = false) : this(itemsCount, noSave)
        { 
            if(noSave == true)
                routeStep.RouteId = r.Id ?? 0;
            else
                routeStep.RouteId = (long)r.Id;
            PopulateOrderCB(true);
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            if (errors.HasErrors() == false)
            {
                if(!noSave)
                {
                    var db = DatabaseUtils.GetDatabase();
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
                routeStep.Order = Convert.ToInt64(OrderCB.SelectedItem);
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

        private void PopulateOrderCB(bool insertMode)
        {
            var routeStepsCount = itemsCount;
            // When inserting there should be one place more for new item
            if (insertMode)
                ++routeStepsCount;

            for(int i = 1; i <= routeStepsCount; ++i)
                OrderCB.Items.Add(i);
        }

    }
}
