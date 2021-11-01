﻿using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using Popstation.Database;
using PSXPackagerGUI.Pages;

namespace PSXPackagerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SinglePage _singlePage;
        private readonly BatchPage _batchPage;
        private readonly SettingsPage _settings;
        private readonly MainModel _model;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly GameDB _gameDb = new GameDB(Path.Combine(ApplicationInfo.AppPath, "Resources", "gameInfo.db"));

        public MainWindow()
        {

            InitializeComponent();

            _cancellationTokenSource = new CancellationTokenSource();

            _settings = new SettingsPage();

            _singlePage = new SinglePage(_settings.Model, _gameDb, _cancellationTokenSource);
            _batchPage = new BatchPage(_settings.Model, _gameDb, _cancellationTokenSource);
            _model = new MainModel();
            DataContext = _model;
            _model.Mode = AppMode.Single;
            CurrentPage.Content = _singlePage;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (_singlePage.IsBusy || _batchPage.IsBusy)
            {
                var result = MessageBox.Show("An operation is in progress. Are you sure you want to cancel?", "PSXPackager",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            _cancellationTokenSource.Cancel();
        }

        private void OpenFile_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
            openFileDialog.Filter = "PBP Files|*.pbp|All files|*.*";
            openFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                _singlePage.LoadPbp(openFileDialog.FileName);
            }
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            _singlePage.Save();
        }
        
        private void Settings_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentPage.Content = _settings;
        }


        private void SingleMode_OnClick(object sender, RoutedEventArgs e)
        {
            _model.Mode = AppMode.Single;
            CurrentPage.Content = _singlePage;
        }

        private void BatchMode_OnClick(object sender, RoutedEventArgs e)
        {
            _model.Mode = AppMode.Batch;
            CurrentPage.Content = _batchPage;
        }
    }
}