using System.Collections.Generic;

namespace Model
{
    public class Employee : Base
    {
        #region Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
        public List<Email> Emails { get; set; }
        #endregion
    }
}
