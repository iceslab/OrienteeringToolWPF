﻿using OrienteeringToolWPF.Interfaces;
using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using OrienteeringToolWPF.Windows;
using OrienteeringToolWPF.Windows.Forms.KidsCompetition;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OrienteeringToolWPF.Views.Lists
{
    /// <summary>
    /// Interaction logic for RelaysListView.xaml
    /// </summary>
    public partial class RelaysListView : UserControl, IRefreshable, IButtonsManageable
    {
        public List<Relay> RelaysList { get; private set; }

        public RelaysListView()
        {
            InitializeComponent();
            ManageButtons(null);
            Refresh();
        }

        public void Refresh()
        {
            var db = MainWindow.GetDatabase();
            RelaysList = db.Relays.All();
            relaysLV.ItemsSource = RelaysList;
        }

        private void addB_Click(object sender, RoutedEventArgs e)
        {
            Window window = new RelayForm();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
            Refresh();
        }

        private void editB_Click(object sender, RoutedEventArgs e)
        {
            Window window = new RelayForm((Relay)relaysLV.SelectedItem);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
            Refresh();
        }

        private void deleteB_Click(object sender, RoutedEventArgs e)
        {
            if (MessageUtils.ShowDeleteWarning(this) == true)
            {
                var db = MainWindow.GetDatabase();
                foreach (Relay r in relaysLV.SelectedItems)
                    db.Relays.DeleteById(r.Id);
                Refresh();
            }
        }

        private void ManageButtons(ListView lv)
        {
            if (lv?.SelectedItem != null)
            {
                editB.IsEnabled = true;
                deleteB.IsEnabled = true;
            }
            else
            {
                editB.IsEnabled = false;
                deleteB.IsEnabled = false;
            }
        }

        private void relaysLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is ListView)
                ManageButtons((ListView)e.Source);
            else
                Console.WriteLine("Not ListView: " + e.Source);
        }

        public void SetButtonsVisibility(Visibility all)
        {
            addB.Visibility = all;
            editB.Visibility = all;
            deleteB.Visibility = all;
        }

        public void SetButtonsVisibility(Visibility add, Visibility edit, Visibility delete)
        {
            addB.Visibility = add;
            editB.Visibility = edit;
            deleteB.Visibility = delete;
        }
    }
}