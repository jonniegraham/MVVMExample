using Model;

namespace LinkTekTest.ModelWrapper
{
    public class EmailWrapper : ModelWrapper<Email>
    {
        public EmailWrapper(Email email) : base(email) { }

        #region SimpleProperties
        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string EmailAddress
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string Notes
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        #endregion

        #region ChangeTrackingProperties
        public string EmailAddressOriginalValue => GetOriginalValue<string>(nameof(EmailAddress));
        public bool EmailAddressIsChanged => GetIsChanged(nameof(EmailAddress));
        public string NotesOriginalValue => GetOriginalValue<string>(nameof(Notes));
        public bool NotesIsChanged => GetIsChanged(nameof(Notes));
        #endregion
    }
}