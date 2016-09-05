namespace OrienteeringToolWPF.Model
{
    public class Category
    {
        public long? Id { get; set; }
        public string Name { get; set; }

        #region Object overrides
        public override string ToString()
        {
            return Name;
        }

        // Equality method (returns true when all fields matches)
        public override bool Equals(object obj)
        {
            if (obj is Category)
            {
                Category c = (Category)obj;
                if (Id != c.Id)
                    return false;
                if (!Name.Equals(c.Name))
                    return false;
                return true;
            }

            return false;
        }

        // Generates hashcode from Name and Id if present
        public override int GetHashCode()
        {
            return Name.GetHashCode() + Id.GetHashCode();
        }
        #endregion
    }
}
