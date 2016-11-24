using OrienteeringToolWPF.Model;
using OrienteeringToolWPF.Utils;
using System.Collections.Generic;

namespace OrienteeringToolWPF.DAO
{
    public static class CategoryHelper
    {
        public static List<Category> Categories()
        {
            var db = DatabaseUtils.GetDatabase();
            var categories = (List<Category>)db.Categories.All();
            return categories;            
        }
    }
}
