using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace Practice
{
    /// <summary>
    ///     Логика взаимодействия для AddEditEngineering.xaml
    /// </summary>
    public partial class AddEditEngineering : Window
    {
        public AddEditEngineering(engineering en, practiceEntities db)
        {
            InitializeComponent();
            db.engineeringtype.Load();
            db.grindingmethod.Load();
            En = en;
            Grindingmethods = db.grindingmethod.Local.ToList();
            Engineeringtypes = db.engineeringtype.Local.ToList();
            DataContext = this;
        }


        public List<grindingmethod> Grindingmethods { get; }
        public List<engineeringtype> Engineeringtypes { get; }

        public engineering En { get; }


        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}