using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Mapper.Candidatura.Models
{
    [Serializable]
    public class Alert
    {
        public Alert() { Messages = new List<string>(); }
        public enum AlertTypeEnum
        {
            Success = 1,
            Warning = 2,
            Error = 3,
            Info = 4
        }

        [NotMapped]
        public string Title { get; set; }
        [NotMapped]
        public List<string> Messages { get; set; }
        [NotMapped]
        public AlertTypeEnum AlertType { get; set; }
        private bool _dismissible = true;
        public bool IsDismissible { get { return _dismissible; } set { _dismissible = value; } }
    }

}