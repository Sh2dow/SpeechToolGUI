using System;
using System.Linq;
using System.Windows;

namespace SpeechToolGUI
{
    public partial class ParamsSelectionWindow : Window
    {
        // These properties will hold the selected enum values.
        public TargetConsoleEnum SelectedConsole { get; private set; }
        public TargetGameEnum SelectedGame { get; private set; }

        public ParamsSelectionWindow()
        {
            InitializeComponent();

            // Populate the comboboxes with the enum values.
            ConsoleComboBox.ItemsSource = Enum.GetValues(typeof(TargetConsoleEnum)).Cast<TargetConsoleEnum>();
            GameComboBox.ItemsSource = Enum.GetValues(typeof(TargetGameEnum)).Cast<TargetGameEnum>();

            // Optionally, set defaults or current settings.
            ConsoleComboBox.SelectedItem = TargetConsoleEnum.PC;
            GameComboBox.SelectedItem = TargetGameEnum.MostWanted;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // Validate selections.
            if (ConsoleComboBox.SelectedItem == null || GameComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select both a target console and a target game.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedConsole = (TargetConsoleEnum)ConsoleComboBox.SelectedItem;
            SelectedGame = (TargetGameEnum)GameComboBox.SelectedItem;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}