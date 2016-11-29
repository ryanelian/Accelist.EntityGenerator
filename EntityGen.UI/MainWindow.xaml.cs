using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.IO;
using EntityGen.Core;

namespace EntityGen.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.ConnectionStringTextBox.Text = @"Data Source=DOCTORLOVE\SQL2016;Initial Catalog=TANGO;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            this.DbContextNameTextBox.Text = "TangoDb";
            this.NamespaceTextBox.Text = "TAM.TANGO.Entities";
            this.ExportPathTextBox.Text = "Entities";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="namespaceText"></param>
        /// <param name="exportPath"></param>
        /// <param name="dbContextName"></param>
        /// <returns></returns>
        public bool ValidateInputs(string connectionString, string namespaceText, string exportPath, string dbContextName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Connection field input must not be empty!", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(namespaceText))
            {
                MessageBox.Show("Namespace field input must not be empty!", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(dbContextName))
            {
                MessageBox.Show("Database Context Name field input must not be empty!", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(exportPath))
            {
                MessageBox.Show("Export Folder Path field input must not be empty!", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                var connectionString = ConnectionStringTextBox.Text;
                var space = NamespaceTextBox.Text;
                var folder = ExportPathTextBox.Text;
                var dbContextName = DbContextNameTextBox.Text;

                if (ValidateInputs(connectionString, space, folder, dbContextName) == false)
                {
                    return;
                }

                var db = new SqlConnection(ConnectionStringTextBox.Text);
                var gen = new GeneratorService(db);

                var scans = await gen.QueryScanModels();
                var entities = gen.ConvertScanToEntities(scans);
                var files = await gen.GenerateEntityVirtualFiles(entities, space, dbContextName);
                gen.WriteVirtualFilesToOutputFolder(files, folder, true);

                MessageBox.Show($"Successfully generated entity models at folder: {folder}!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                this.IsEnabled = true;
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
