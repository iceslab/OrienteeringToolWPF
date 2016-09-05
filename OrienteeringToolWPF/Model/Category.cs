namespace OrienteeringToolWPF.Model
{
    public class Category
    {
        public long? Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
