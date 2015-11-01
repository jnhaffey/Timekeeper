using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Timekeeper.Crm;

namespace Timekeeper.VsExtension
{
    /// <summary>
    /// Interaction logic for CategorySelection.xaml
    /// </summary>
    public partial class CategorySelection : Window
    {
        public CategorySelection()
        {
            InitializeComponent();
        }

        public CategorySelection(IEnumerable<Crm.Category> categories) : this()
        {
            DataContext = categories;
        }

        public Category Value
        {
            get
            {
                return CategoryCombo.SelectedItem as Category;
            }
        }
    }
}
