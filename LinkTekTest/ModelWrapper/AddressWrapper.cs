using Model;

namespace LinkTekTest.ModelWrapper
{
    public class AddressWrapper : ModelWrapper<Address>
    {
        public AddressWrapper(Address address) : base(address) { }

        #region SimpleProperties
        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string Number
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string Street
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string City
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        #endregion

        #region ChangeTrackingProperties
        public string NumberOriginalValue => GetOriginalValue<string>(nameof(Number));
        public bool NumberIsChanged => GetIsChanged(nameof(Number));
        public string StreetOriginalValue => GetOriginalValue<string>(nameof(Street));
        public bool StreetIsChanged => GetIsChanged(nameof(Street));
        public string CityOriginalValue => GetOriginalValue<string>(nameof(City));
        public bool CityIsChanged => GetIsChanged(nameof(City));
        #endregion
    }
}
