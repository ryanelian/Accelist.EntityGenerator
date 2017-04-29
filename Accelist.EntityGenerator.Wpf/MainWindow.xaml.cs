using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;
using Accelist.EntityGenerator.Wpf.Models;
using LiteDB;

namespace Accelist.EntityGenerator.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly LiteDatabase DB;
        private readonly LiteCollection<SavedConfiguration> SavedConfigurations;

        public MainWindow()
        {
            InitializeComponent();

            this.DB = new LiteDatabase(@"EntityGeneratorConfiguration.db");
            this.SavedConfigurations = DB.GetCollection<SavedConfiguration>();
            this.SavedConfigurations.EnsureIndex(Q => Q.Name);
        }

        protected override void OnClosed(EventArgs e)
        {
            DB.Dispose();
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs() == false)
            {
                return;
            }

            var ns = NamespaceInput.Text.Trim();
            var folder = FolderInput.Text.Trim();
            var dbContext = DbContextInput.Text.Trim();

            try
            {
                this.IsEnabled = false;
                using (var sql = new SqlConnection(ConnectionStringInput.Text.Trim()))
                {
                    var gen = new EntityGenerator(sql);
                    var entities = (await gen.Scan()).ToEntities();

                    if (ValidateEntities(entities) == false)
                    {
                        return;
                    }

                    await Task.Run(() =>
                    {
                        entities.ToVirtualFiles(ns, dbContext).WriteToFolder(folder);
                    });
                }

                MessageBox.Show($"Successfully generated entity models!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();

            this.FolderInput.Text = dialog.SelectedPath;
        }

        private void ConfigurationSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ConfigurationNameInput.Text))
            {
                MessageBox.Show("Configuration name input must not be empty!", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var data = SavedConfigurations.FindOne(Q => Q.Name == this.ConfigurationNameInput.Text);

            if (data != null)
            {
                data.ConnectionString = ConnectionStringInput.Text;
                data.DbContextName = DbContextInput.Text;
                data.ProjectNamespace = NamespaceInput.Text;
                data.ExportToFolder = FolderInput.Text;
                SavedConfigurations.Update(data);
                return;
            }

            SavedConfigurations.Insert(new SavedConfiguration
            {
                Name = ConfigurationNameInput.Text.Trim(),
                ConnectionString = ConnectionStringInput.Text.Trim(),
                DbContextName = DbContextInput.Text.Trim(),
                ProjectNamespace = NamespaceInput.Text.Trim(),
                ExportToFolder = FolderInput.Text.Trim(),
            });
            RefreshConfigurationList();
        }

        public bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(ConnectionStringInput.Text))
            {
                MessageBox.Show("Connection string input must not be empty!", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(NamespaceInput.Text))
            {
                MessageBox.Show("Project namespace input must not be empty!", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (NamespaceInput.Text.Contains(" "))
            {
                MessageBox.Show("Project namespace input must not contains space!", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(DbContextInput.Text))
            {
                MessageBox.Show("Database context name input must not be empty!", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (DbContextInput.Text.Contains(" "))
            {
                MessageBox.Show("Database context name input must not contains space!", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public bool ValidateEntities(List<Entity> entities)
        {
            var validator = new List<string>();
            foreach (var entity in entities)
            {
                if (entity.Validate() == false)
                {
                    validator.Add(entity.Name);
                }
            }

            if (validator.Any())
            {
                var msg = "The following table(s) are missing Primary Key(s): " + string.Join(", ", validator);
                MessageBox.Show(msg, "Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void MainWindowControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.ConfigurationList.DisplayMemberPath = nameof(SavedConfiguration.Name);
            this.ConfigurationList.SelectedValuePath = nameof(SavedConfiguration.Id);
            // Apparently if you pre-set values in XAML, it'll run before all form controls are ready! BAD!
            this.FolderInput.Text = "Entities";
            FolderCheck();
            RefreshConfigurationList();
        }

        private void ConfigurationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConfigurationList == null)
            {
                return;
            }

            EraseConfigurationButton.IsEnabled = ConfigurationList.SelectedItem != null;

            if (ConfigurationList.SelectedItem == null)
            {
                return;
            }

            var item = (SavedConfiguration)ConfigurationList.SelectedItem;

            ConfigurationNameInput.Text = item.Name;
            ConnectionStringInput.Text = item.ConnectionString;
            DbContextInput.Text = item.DbContextName;
            NamespaceInput.Text = item.ProjectNamespace;
            FolderInput.Text = item.ExportToFolder;
        }

        private void RefreshConfigurationList()
        {
            this.ConfigurationList.ItemsSource = SavedConfigurations.FindAll();
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var path = System.IO.Path.GetFullPath(FolderInput.Text);
            Process.Start("explorer.exe", path);
        }

        private void CopyFileButton_Click(object sender, RoutedEventArgs e)
        {
            var path = System.IO.Path.GetFullPath(FolderInput.Text);
            var files = Directory.EnumerateFiles(path);
            var c = new StringCollection();
            foreach (var file in files)
            {
                c.Add(file);
            }
            Clipboard.SetFileDropList(c);
        }

        private void EraseConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (SavedConfiguration)ConfigurationList.SelectedItem;

            SavedConfigurations.Delete(Q => Q.Id == item.Id);
            RefreshConfigurationList();
        }

        private void FolderInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            FolderCheck();
        }

        private bool FolderCheck()
        {
            var exist = Directory.Exists(FolderInput.Text);
            OpenFolderButton.IsEnabled = exist;
            CopyFileButton.IsEnabled = exist;
            return exist;
        }
    }
}
