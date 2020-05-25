using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Linq.Expressions;


namespace AssetManager
{

    

    public class ChecklistboxViewModel :  ViewModelBase
    {
        private CheckListBoxList<AssetGroup> _list;

        private bool canExecute = true;
        private ICommand hiButtonCommand;

        public ICommand HiButtonCommand
        {
            get { return hiButtonCommand; }
            set { hiButtonCommand = value; }
        }

        public void ShowAllItems(object obj)
        {
            _list.ShowAll = !_list.ShowAll;
        }
        
        public ChecklistboxViewModel()
        {
      
         //   _list.VisibleCount = 2;
           
        

            //the collection:
            AssetGroups _groups = new AssetGroups(1);
            
            var sortedList = Sort<AssetGroup>(_groups, "Group");

       //     Type type = _groups.GetType().GetProperty("Item").PropertyType;
 


           _list = new CheckListBoxList<AssetGroup>(sortedList);

            HiButtonCommand = new RelayCommand(ShowAllItems, param => this.canExecute);
                   
        }

       


        public static List<T> Sort<T>(IEnumerable<T> list, string sortField)
        {
            var param = System.Linq.Expressions.Expression.Parameter(typeof(T), string.Empty);    
            //normally one would use Expression.Property(param, sortField), but that doesnt work    
            //when working with interfaces where the sortField is defined on a base interface.    
            //so instead we search for the Property through our own GetProperty method and use it to build the     
            //Expression property    
            PropertyInfo propertyInfo = GetProperty(typeof (T), sortField);

       //     Type type = list.GetType().GetProperty("Item").PropertyType;

           

            var property = System.Linq.Expressions.Expression.Property(param, propertyInfo);
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, object>>(System.Linq.Expressions.Expression.Convert(property, typeof(object)), param);    
      
            var    returnList = list.AsQueryable().OrderBy(lambda).ToList();    
            return returnList;
        }
        
        ///// Allows you to get the PropertyInfo for a property defined on the provided type
        // Code allows you to find the property even from base interfaces (Type.GetProperty does not return
        // properties from base interfaces
        // if you wish to look for a specific property defined on a certain interface, then provide the
        // propertyName as 'interface.propertyname'
        ///
        private static PropertyInfo GetProperty(Type type, string propertyName)
        {       
            string typeName = string.Empty;       
            if (propertyName.Contains("."))       
            {              
                //name was specified with typename - so pull out the typename              
                typeName = propertyName.Substring(0, propertyName.IndexOf("."));              
                propertyName = propertyName.Substring(propertyName.IndexOf(".")+1);       
            }             
            PropertyInfo prop = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);       
            if (prop == null)       
            {              
                var baseTypesAndInterfaces = new List<Type>();              
                if (type.BaseType != null) 
                    baseTypesAndInterfaces.Add(type.BaseType);              
                baseTypesAndInterfaces.AddRange(type.GetInterfaces());              
                foreach (Type t in baseTypesAndInterfaces)              
                {                     
                    prop = GetProperty(t, propertyName);                     
                    if (prop != null)                     
                    {                           
                        if (!string.IsNullOrEmpty(typeName) && t.Name != typeName)                                  
                            continue; 
                        //keep looking as the typename was not found                           
                        break;                     
                    }              
                }       
            }       
            return prop;
        }

        public List<T> Sort<T, T1>(IEnumerable<T> list, Func<T, T1> sorter) 
        { 
            List<T> returnList = null; 
          //  if (direction == SortDirection.Ascending)        
            returnList = list.OrderBy(sorter).ToList(); 
         //   else        
           //     returnList = list.OrderByDescending(sorter).ToList(); 
            return returnList; 
        }



        public CheckListBoxList<AssetGroup> Chklist
        {
            get { return _list; }
            set { _list = value; }
        }

        string _toggleTextMore;
        public string ToggleTextMore
        {
            get { return _toggleTextMore; }
            set 
            {               
                SetField(ref _toggleTextMore, value);  
            }
        }

        string _toggleTextLess;
        public string ToggleTextLess

        {
            get { return _toggleTextLess; }
            set
            {
                SetField(ref _toggleTextLess, value);
            }
        }

        Visibility _toggleButtonVisibility;
        public Visibility ToggleButtonVisibility
        {
            get { return _toggleButtonVisibility; }
            set
            {
                SetField(ref _toggleButtonVisibility, value); 
            }
        }
                
       // public event PropertyChangedEventHandler PropertyChanged;

    }



        public class CheckListBoxList<T> : List<SelectionItem<T>>, INotifyPropertyChanged
        {
            #region private fields
            /// <summary>
            /// the number of selected elements
            /// </summary>
            private int _selectionCount;
            private int _visibleCount;
            private bool _showAll;

            #endregion

            #region private methods
            /// <summary>
            /// this events responds to the "IsSelectedEvent" and VisibleState change
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                var item = sender as SelectionItem<T>;
                if ((item != null) && e.PropertyName == "IsSelected")
                {
                    SelectionCount = this.Count(me1 => me1.IsSelected == true);
                }
            }

            #endregion


            public CheckListBoxList()
            { }

            /// <summary>
            /// creates the selection list from an existing simple list
            /// </summary>
            /// <param name="elements"></param>
            public CheckListBoxList(IEnumerable<T> elements)
            {
                foreach (T element in elements)
                    AddItem(element);

                _visibleCount = this.Count(e => e.VisibleState == Visibility.Visible);
            }

            #region public methods
            /// <summary>
            /// adds an element to the element and listens to its "IsSelected" property to update the SelectionCount property
            /// use this method insteand of the "Add" one
            /// </summary>
            /// <param name="element"></param>
            public void AddItem(T element)
            {
                var item = new SelectionItem<T>(element);
                item.PropertyChanged += item_PropertyChanged;
                Add(item);
            }

            /// <summary>
            /// gets the selected elements
            /// </summary>
            /// <returns></returns>
            public IEnumerable<T> GetSelection()
            {
                return this.Where(e => e.IsSelected).Select(e => e.Element);
            }

            /// <summary>
            /// uses linq expression to select a part of an object (for example, only id)
            /// </summary>
            /// <typeparam name="U"></typeparam>
            /// <param name="expression"></param>
            /// <returns></returns>
            public IEnumerable<U> GetSelection<U>(Func<SelectionItem<T>, U> expression)
            {
                return this.Where(e => e.IsSelected).Select(expression);
            }

            #endregion


            #region public properties
            /// <summary>
            /// the selection count property is ui-bindable, returns the number of selected elements
            /// </summary>
            public int SelectionCount
            {
                get { return _selectionCount; }

                private set
                {
                    _selectionCount = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("SelectionCount"));
                }
            }

            public int VisibleCount
            {
                private get { return _visibleCount; }
                set
                {
                    _visibleCount = value;
                    for (var i = this.Count - 1; i >= 0; i--)
                    {
                        if (i < value)
                        {
                            this[i].VisibleState = Visibility.Visible;
                        }
                        else
                        {
                            this[i].VisibleState = Visibility.Collapsed;
                        }
                    }
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("VisibleCount"));
                }
            }

            public bool ShowAll
            {
                get { return _showAll; }
                set
                {
                    _showAll = value;

                    if (value == true)
                    {
                        int temp = _visibleCount;
                        VisibleCount = this.Count;
                        _visibleCount = temp;
                    }
                    else
                    {
                        VisibleCount = _visibleCount;
                    }

                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("ShowAll"));
                }
            }


            #endregion public properties

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion
        }



   






}
