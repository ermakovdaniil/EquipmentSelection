using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Practice
{
    internal class MainVM : INotifyPropertyChanged
    {
        public MainVM()
        {
            db = new practiceEntities();
            db.engineeringtype.Load();
            db.material.Load();
            db.grindingmethod.Load();
            db.engineering.Load();
            db.characteristics.Load();

            HighestCost = db.engineering.Local.Max(t => t.Cost);
            HighestInputSize = db.characteristics.Local.Max(t => t.InputSize);
            HighestOutputSize = db.characteristics.Local.Max(t => t.OutputSize);
            HighestPerfomance = db.characteristics.Local.Max(t => t.Perfomance);
            HighestPower = db.characteristics.Local.Max(t => t.EnginePower);
            HighestWeight = db.characteristics.Local.Max(t => t.Weight);

            MaxCost = HighestCost;
            MaxInputSize = HighestInputSize;
            MaxOutputSize = HighestOutputSize;
            MaxPerfomance = HighestPerfomance;
            MaxPower = HighestPower;
            MaxWeight = HighestWeight;

            MinCost = 0;
            MinInputSize = 0;
            MinOutputSize = 0;
            MinPerfomance = 0;
            MinPower = 0;
            MinWeight = 0;


            Engineerings = db.engineering.Local;
            GrindingMethods = db.grindingmethod.Local;
            Materials = db.material.Local;
            EngineeringTypes = db.engineeringtype.Local;


            SelectedMaterials = new ObservableCollection<material>();
            SelectedEngineerings = new ObservableCollection<engineering>();
            SelectedEngineeringTypes = new ObservableCollection<engineeringtype>(EngineeringTypes);
            SelectedGrindingMethods = new ObservableCollection<grindingmethod>(GrindingMethods);

            SelectedEngineerings = db.engineering.Local;
            SelectEngeerings();
        }


        private void SelectEngeerings()
        {
            db.engineering.Load();
            Engineerings = db.engineering.Local;
            var engineerings = from t in db.engineering.Local.ToList()
                where (t.Cost <= MaxCost) && (t.Cost >= MinCost) &&
                      (t.characteristics.Perfomance <= MaxPerfomance) &&
                      (t.characteristics.Perfomance >= MinPerfomance) &&
                      (t.characteristics.EnginePower <= MaxPower) &&
                      (t.characteristics.EnginePower >= MinPower) &&
                      (t.characteristics.InputSize <= MaxInputSize) &&
                      (t.characteristics.InputSize >= MinInputSize) &&
                      (t.characteristics.OutputSize <= MaxOutputSize) &&
                      (t.characteristics.OutputSize >= MinOutputSize) &&
                      (t.characteristics.Weight <= MaxWeight) &&
                      (t.characteristics.Weight >= MinWeight) &&
                      SelectedEngineeringTypes.Contains(t.characteristics.engineeringtype) &&
                      SelectedGrindingMethods.Contains(t.characteristics.grindingmethod)
                select t;
            var answer = new List<engineering>();
            foreach (var engineeringg in engineerings)
            {
                foreach (var material in SelectedMaterials)
                {
                    var temp = engineeringg.process.Where(t => SelectedMaterials.Contains(t.material));
                    temp = temp.Where(t => t.Abiity == true);
                    if ((temp.Count() == SelectedMaterials.Count) && !answer.Contains(engineeringg))
                    {
                        answer.Add(engineeringg);
                    }
                }
            }

            db.process.Load();
            SelectedEngineerings = new ObservableCollection<engineering>(answer);
        }


        #region Fields

        private readonly practiceEntities db;
        private ObservableCollection<engineering> _engineerings;
        private ObservableCollection<material> _materials;
        private decimal? _maxCost;
        private double? _maxInputSize;
        private double? _maxOutputSize;
        private double? _maxPerfomance;
        private double? _maxPower;
        private double? _maxWeight;
        private decimal? _minCost;
        private double? _minInputSize;
        private double? _minOutputSize;
        private double? _minPerfomance;
        private double? _minPower;
        private double? _minWeight;
        private ObservableCollection<engineering> _selectedEngineerings;
        private ObservableCollection<material> _selectedMaterials;

        private RelayCommand filterCommand;
        private RelayCommand addEngineering;
        private RelayCommand addMaterial;
        private RelayCommand deleteEngineering;
        private RelayCommand deleteMaterial;
        private RelayCommand editEngineering;
        private RelayCommand editMaterial;

        #endregion


        #region Commands

        #region Material

        public RelayCommand AddMaterial
        {
            get
            {
                return addMaterial ??
                       (addMaterial = new RelayCommand(o =>
                       {
                           var materialEdit =
                               new AddEditMaterial(new material());
                           if (materialEdit.ShowDialog() == true)
                           {
                               var material = materialEdit.Mat;
                               db.material.Add(material);
                               db.SaveChanges();
                               db.engineering.Load();
                               Materials = new ObservableCollection<material>(db.material.Local);
                           }
                       }));
            }
        }

        public RelayCommand EditMaterial
        {
            get
            {
                return editMaterial ??
                       (editMaterial = new RelayCommand(selectedItem =>
                       {
                           if (selectedItem == null)
                           {
                               return;
                           }

                           // получаем выделенный объект
                           var material = selectedItem as material;

                           var vm = new material
                           {
                               BreakingPointMin = material.BreakingPointMin,
                               BreakingPointMax = material.BreakingPointMax,
                               MatID = material.MatID,
                               Name = material.Name
                           };
                           var addEditMaterial = new AddEditMaterial(vm);


                           if (addEditMaterial.ShowDialog() == true)
                           {
                               // получаем измененный объект
                               material = db.material.Find(addEditMaterial.Mat.MatID);
                               if (material != null)
                               {
                                   material.Name = addEditMaterial.Mat.Name;
                                   material.BreakingPointMax = addEditMaterial.Mat.BreakingPointMax;
                                   material.BreakingPointMin = addEditMaterial.Mat.BreakingPointMin;
                                   db.Entry(material).State = EntityState.Modified;
                                   db.SaveChanges();
                                   db.material.Load();
                                   Materials = new ObservableCollection<material>(db.material.Local);
                               }
                           }
                       }));
            }
        }

        public RelayCommand DeleteMaterial
        {
            get
            {
                return deleteMaterial ??
                       (deleteMaterial = new RelayCommand(selectedItem =>
                       {
                           if (selectedItem == null)
                           {
                               return;
                           }

                           var result = MessageBox.Show("Вы действительно хотите удалить запись?", "",
                               MessageBoxButton.OKCancel, MessageBoxImage.Question);
                           if (result == MessageBoxResult.OK)
                           {
                               var material = selectedItem as material;
                               db.material.Remove(material);
                               db.SaveChanges();
                               db.material.Load();
                               Materials = new ObservableCollection<material>(db.material.Local);
                           }
                       }));
            }
        }

        #endregion


        #region Engineering

        public RelayCommand AddEngineering
        {
            get
            {
                return addEngineering ??
                       (addEngineering = new RelayCommand(o =>
                       {
                           var engineeringEdit =
                               new AddEditEngineering(new engineering {characteristics = new characteristics()}, db);
                           if (engineeringEdit.ShowDialog() == true)
                           {
                               var engineering = engineeringEdit.En;
                               db.engineering.Add(engineering);
                               db.SaveChanges();
                               db.engineering.Load();
                               Engineerings = new ObservableCollection<engineering>(db.engineering.Local);
                           }
                       }));
            }
        }

        public RelayCommand EditEngineering
        {
            get
            {
                return editEngineering ??
                       (editEngineering = new RelayCommand(selectedItem =>
                       {
                           if (selectedItem == null)
                           {
                               return;
                           }

                           // получаем выделенный объект
                           var engineering = selectedItem as engineering;

                           var vm = new engineering
                           {
                               EnID = engineering.EnID,
                               Name = engineering.Name,
                               Cost = engineering.Cost,
                               characteristics = new characteristics
                               {
                                   Perfomance = engineering.characteristics.Perfomance,
                                   EnginePower = engineering.characteristics.EnginePower,
                                   InputSize = engineering.characteristics.InputSize,
                                   OutputSize = engineering.characteristics.OutputSize,
                                   Weight = engineering.characteristics.Weight,
                                   grindingmethod = engineering.characteristics.grindingmethod,
                                   engineeringtype = engineering.characteristics.engineeringtype
                               }
                           };
                           var addEditEngineering = new AddEditEngineering(vm, db);


                           if (addEditEngineering.ShowDialog() == true)
                           {
                               // получаем измененный объект
                               engineering = db.engineering.Find(addEditEngineering.En.EnID);
                               if (engineering != null)
                               {
                                   engineering.Name = addEditEngineering.En.Name;
                                   engineering.Cost = addEditEngineering.En.Cost;
                                   engineering.characteristics.Perfomance =
                                       addEditEngineering.En.characteristics.Perfomance;
                                   engineering.characteristics.EnginePower =
                                       addEditEngineering.En.characteristics.EnginePower;
                                   engineering.characteristics.InputSize =
                                       addEditEngineering.En.characteristics.InputSize;
                                   engineering.characteristics.OutputSize =
                                       addEditEngineering.En.characteristics.OutputSize;
                                   engineering.characteristics.grindingmethod =
                                       addEditEngineering.En.characteristics.grindingmethod;
								engineering.characteristics.Weight =
									   addEditEngineering.En.characteristics.Weight;  
								engineering.characteristics.engineeringtype =
                                       addEditEngineering.En.characteristics.engineeringtype;

                                   //db.Entry(engineering).State = EntityState.Modified;
                                   db.SaveChanges();
                                   db.engineering.Load();
                                   Engineerings = new ObservableCollection<engineering>(db.engineering.Local);
                               }
                           }
                       }));
            }
        }

        public RelayCommand DeleteEngineering
        {
            get
            {
                return deleteEngineering ??
                       (deleteEngineering = new RelayCommand(selectedItem =>
                       {
                           if (selectedItem == null)
                           {
                               return;
                           }

                           var result = MessageBox.Show("Вы действительно хотите удалить запись?", "",
                               MessageBoxButton.OKCancel, MessageBoxImage.Question);
                           if (result == MessageBoxResult.OK)
                           {
                               var engineering = selectedItem as engineering;
                               db.engineering.Remove(engineering);
                               db.SaveChanges();
                               db.engineering.Load();
                               Engineerings = new ObservableCollection<engineering>(db.engineering.Local);
                           }
                       }));
            }
        }

        #endregion


        public RelayCommand FilterCommand
        {
            get
            {
                return filterCommand ??
                       (filterCommand = new RelayCommand(o => { SelectEngeerings(); }));
            }
        }

        #endregion


        #region Collections

        public ObservableCollection<engineering> Engineerings
        {
            get { return _engineerings; }
            set
            {
                _engineerings = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<grindingmethod> GrindingMethods { get; set; }

        public ObservableCollection<material> Materials
        {
            get { return _materials; }
            set
            {
                _materials = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<engineeringtype> EngineeringTypes { get; set; }


        #region SelectedCollections

        public ObservableCollection<engineering> SelectedEngineerings
        {
            get { return _selectedEngineerings; }
            set
            {
                _selectedEngineerings = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<grindingmethod> SelectedGrindingMethods { get; set; }

        public ObservableCollection<material> SelectedMaterials { get; set; }

        public ObservableCollection<engineeringtype> SelectedEngineeringTypes { get; set; }

        #endregion

        #endregion


        #region Properties

        #region HighestValues

        public decimal? HighestCost { get; set; }
        public double? HighestPower { get; set; }
        public double? HighestPerfomance { get; set; }
        public double? HighestInputSize { get; set; }
        public double? HighestOutputSize { get; set; }
        public double? HighestWeight { get; set; }

        #endregion


        public decimal? MinCost
        {
            get { return _minCost; }

            set
            {
                _minCost = value;
                OnPropertyChanged();
            }
        }

        public decimal? MaxCost
        {
            get { return _maxCost; }
            set
            {
                _maxCost = value;
                OnPropertyChanged();
            }
        }

        public double? MinPerfomance
        {
            get { return _minPerfomance; }
            set
            {
                _minPerfomance = value;
                OnPropertyChanged();
            }
        }

        public double? MaxPerfomance
        {
            get { return _maxPerfomance; }
            set
            {
                _maxPerfomance = value;
                OnPropertyChanged();
            }
        }

        public double? MinPower
        {
            get { return _minPower; }
            set
            {
                _minPower = value;
                OnPropertyChanged();
            }
        }

        public double? MaxPower
        {
            get { return _maxPower; }
            set
            {
                _maxPower = value;
                OnPropertyChanged();
            }
        }

        public double? MinInputSize
        {
            get { return _minInputSize; }
            set
            {
                _minInputSize = value;
                OnPropertyChanged();
            }
        }

        public double? MaxInputSize
        {
            get { return _maxInputSize; }
            set
            {
                _maxInputSize = value;
                OnPropertyChanged();
            }
        }

        public double? MinOutputSize
        {
            get { return _minOutputSize; }
            set
            {
                _minOutputSize = value;
                OnPropertyChanged();
            }
        }

        public double? MaxOutputSize
        {
            get { return _maxOutputSize; }
            set
            {
                _maxOutputSize = value;
                OnPropertyChanged();
            }
        }

        public double? MinWeight
        {
            get { return _minWeight; }
            set
            {
                _minWeight = value;
                OnPropertyChanged();
            }
        }

        public double? MaxWeight
        {
            get { return _maxWeight; }
            set
            {
                _maxWeight = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;


        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion
    }
}