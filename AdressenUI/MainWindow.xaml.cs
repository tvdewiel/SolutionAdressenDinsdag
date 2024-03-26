﻿using AdressenBL.Exceptions;
using AdressenBL.Managers;
using AdressenDL;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdressenUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenFileDialog fileDialog=new OpenFileDialog();
        private FileManager fileManager=new FileManager(new FileProcessor());

        public MainWindow()
        {
            InitializeComponent();
            fileDialog.DefaultExt = ".zip";
            fileDialog.Filter = "Zip files (.zip)|*.zip";
            fileDialog.InitialDirectory = @"c:\data\adresdata";
            fileDialog.Multiselect = false;
        }

        private void SourceFileButton_Click(object sender, RoutedEventArgs e)
        {
            bool? result=fileDialog.ShowDialog();
            if ((result == true) && !string.IsNullOrWhiteSpace(fileDialog.FileName))
            {
                SourceFileTextBox.Text = fileDialog.FileName;
                try
                {
                    List<string> fileNames = fileManager.GetFilesFromZip(SourceFileTextBox.Text);
                    ZipListBox.ItemsSource = fileNames;
                    fileManager.CheckZipFile(SourceFileTextBox.Text,fileNames);                    
                }
                catch(ZipFileManagerException ex) 
                {
                    List<string> errors = new();
                    foreach(var key in ex.Data.Keys) { errors.Add($"'{key}' - error : {ex.Data[key]}"); }
                    ZipListBox.ItemsSource = errors;
                    MessageBox.Show(ex.Message,"FileManager",MessageBoxButton.OK,MessageBoxImage.Error);
                    SourceFileTextBox.Text = null;
                        
                }
                catch(FileManagerException ex)
                {
                    ZipListBox.ItemsSource = null;
                    SourceFileTextBox.Text = null;
                    MessageBox.Show(ex.Message, "FileManager", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DestinationFolderButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}