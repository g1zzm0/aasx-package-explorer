// Copyright (C) 2020 Robin Krahl, RD/ESR, SICK AG <robin.krahl@sick.de>
// This software is licensed under the Apache License 2.0 (Apache-2.0).

#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AasxImport
{
    /// <summary>
    /// Query a data provider and a query string from the user that will be uesd to retrieve data from the network
    /// (see <see cref="Model.IDataProvider.Fetch"/>).
    /// </summary>
    internal partial class FetchOnlineDialog : Window
    {
        public Model.IDataProvider? DataProvider => ComboBoxProvider.SelectedItem as Model.IDataProvider;
        public string Query { get; set; } = string.Empty;

        public FetchOnlineDialog(ICollection<Model.IDataProvider> providers)
        {
            DataContext = this;

            InitializeComponent();

            foreach (var provider in providers)
                ComboBoxProvider.Items.Add(provider);
            if (providers.Count > 0)
                ComboBoxProvider.SelectedItem = providers.First();
        }

        private void ComboBoxProvider_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (!(ComboBoxProvider.SelectedItem is Model.IDataProvider provider))
                return;
            if (!provider.IsFetchSupported)
                return;

            LabelQuery.Content = provider.FetchPrompt;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
