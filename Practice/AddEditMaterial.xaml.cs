using System.Windows;

namespace Practice
{
    /// <summary>
    ///     Логика взаимодействия для AddEditMaterial.xaml
    /// </summary>
    public partial class AddEditMaterial : Window
    {
        public AddEditMaterial(material mat)
        {
            InitializeComponent();
            Mat = mat;
            DataContext = this;
        }


        public material Mat { get; }


        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}