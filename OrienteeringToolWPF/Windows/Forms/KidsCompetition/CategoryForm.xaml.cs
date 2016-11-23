using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System.Windows;

namespace OrienteeringToolWPF.Windows.Forms.KidsCompetition
{
    /// <summary>
    /// Interaction logic for KCRelayForm.xaml
    /// </summary>
    public partial class CategoryForm : Window, IForm
    {
        public Category category { get; private set; }

        public CategoryForm()
        {
            InitializeComponent();
            category = new Category();
        }

        public CategoryForm(Category c)
        {
            InitializeComponent();
            category = c;
            ObjectToForm();
        }

        private void SaveB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            if (errors.HasErrors() == false)
            {
                var db = DatabaseUtils.GetDatabase();
                db.Categories.Upsert(category);

                Close();
            }
            else
            {
                MessageUtils.ShowValidatorErrors(this, errors);
            }
        }

        private void SaveAndNextB_Click(object sender, RoutedEventArgs e)
        {
            var errors = FormToObject();
            if (errors.HasErrors() == false)
            {
                var db = DatabaseUtils.GetDatabase();
                db.Categories.Upsert(category);
                category = new Category();

                NameTB.Text = "";
            }
            else
            {
                MessageUtils.ShowValidatorErrors(this, errors);
            }
        }

        public void ObjectToForm()
        {
            NameTB.Text = category.Name;
        }

        public ErrorList FormToObject()
        {
            var errors = ValidateForm();
            if (errors.HasErrors() == false)
            {
                category.Name = NameTB.Text;
            }
            return errors;
        }

        public ErrorList ValidateForm()
        {
            var errors = new ErrorList();
            if (string.IsNullOrWhiteSpace(NameTB.Text))
                errors.Add(Properties.Resources.CategoryName, Properties.Resources.NullOrEmptyError);
            return errors;
        }
    }
}
