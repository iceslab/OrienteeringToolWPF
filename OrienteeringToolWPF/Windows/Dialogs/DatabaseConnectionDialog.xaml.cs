﻿using MySql.Data.MySqlClient;
using OrienteeringToolWPF.Utils;
using OrienteeringToolWPF.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OrienteeringToolWPF.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DatabseConnectionDialog.xaml
    /// </summary>
    public partial class DatabseConnectionDialog : Window
    {
        public DatabaseConnectionData databaseConnectionData { get; private set; }

        public DatabseConnectionDialog()
        {
            InitializeComponent();
            databaseConnectionData = new DatabaseConnectionData();
        }

        public DatabseConnectionDialog(DatabaseConnectionData data) : this()
        {
            databaseConnectionData = data ?? new DatabaseConnectionData();
            ObjectToForm();
        }

        public void ObjectToForm()
        {
#if DEBUG
            serverTB.Text = databaseConnectionData.Server ?? "localhost";
            portTB.Text = databaseConnectionData.Port ?? "3306";
            schemaTB.Text = databaseConnectionData.Schema ?? "orienteering";
            userTB.Text = databaseConnectionData.User ?? "root";
            passwordPB.Password = "";
#else
            serverTB.Text = databaseConnectionData.Server ?? "";
            portTB.Text = databaseConnectionData.Port ?? "3306";
            schemaTB.Text = databaseConnectionData.Schema ?? "";
            userTB.Text = databaseConnectionData.User ?? "";
            passwordPB.Password = "";
#endif
        }

        public ErrorList FormToObject()
        {
            var errors = ValidateForm();
            if (errors.HasErrors() == false)
            {
                databaseConnectionData.Server = serverTB.Text;
                databaseConnectionData.Port = portTB.Text;
                databaseConnectionData.Schema = schemaTB.Text;
                databaseConnectionData.User = userTB.Text;
                databaseConnectionData.Password = passwordPB.Password ?? "";
            }
            return errors;
        }

        public ErrorList ValidateForm()
        {
            var errors = new ErrorList();
            if (string.IsNullOrWhiteSpace(serverTB.Text))
                errors.Add(Properties.Resources.ServerName, Properties.Resources.NullOrEmptyError);

            if (string.IsNullOrWhiteSpace(portTB.Text))
                errors.Add(Properties.Resources.PortName, Properties.Resources.NullOrEmptyError);
            long n;
            if (long.TryParse(portTB.Text, out n) == false)
                errors.Add(Properties.Resources.PortName, Properties.Resources.NotANumberError);

            if (string.IsNullOrWhiteSpace(schemaTB.Text))
                errors.Add(Properties.Resources.SchemaName, Properties.Resources.NullOrEmptyError);
            if (string.IsNullOrWhiteSpace(userTB.Text))
                errors.Add(Properties.Resources.UserName, Properties.Resources.NullOrEmptyError);
            return errors;
        }

        private ErrorList VerifyConnection()
        {
            var errors = new ErrorList();
            connectB.IsEnabled = false;
            using (var l_oConnection = new MySqlConnection(
                string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};SslMode=Preferred;",
                    databaseConnectionData.Server,
                    databaseConnectionData.Port,
                    databaseConnectionData.Schema,
                    databaseConnectionData.User,
                    databaseConnectionData.Password)))
            {
                try
                {
                    l_oConnection.Open();
                }
                // TODO: change body of catch to add errors to error list
                catch (SqlException e)
                {
                    MessageBox.Show(e.Message);
                    errors.Add("Błąd połączenia", "Nie można połączyć się z bazą danych");
                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show(e.Message);
                    errors.Add("Błąd połączenia", "Nieprawidłowa operacja");
                }
                catch (ConfigurationErrorsException e)
                {
                    MessageBox.Show(e.Message);
                    errors.Add("Błąd połączenia", "Nieprawidłowa konfiguracja połączenia");
                }

                connectB.IsEnabled = true;
            }
            return errors;
        }


        private void connectB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            var errorsPresent = errors.HasErrors();
            if (errorsPresent == false)
            {
                errors = VerifyConnection();
                errorsPresent = errors.HasErrors();
                if (errorsPresent == false)
                {
                    // If DialogResult is assigned earlier, clicking this button 
                    // will close this dialog no matter what result really is
                    DialogResult = true;
                    Close();
                }
            }

            if(errorsPresent == true)
            {
                MessageUtils.ShowValidatorErrors(this, errors);
            }
        }
    }
}
