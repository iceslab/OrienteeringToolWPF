/// <summary>
/// Models relay info
/// </summary>
namespace OrienteeringToolWPF.Model
{
    public class Relay : BaseModel
    {
        public long? Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if(obj is Relay)
            {
                Relay r = (Relay)obj;
                if(Id == r.Id)
                {
                    if (Name.Equals(r.Name))
                        return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Id.GetHashCode();
        }
    }
}
