//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mapper.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Bacteria
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bacteria()
        {
            this.Opportunita = new HashSet<Opportunita>();
        }
    
        public string code { get; set; }
        public string description_EN { get; set; }
        public string description_IT { get; set; }
        public Nullable<int> ordinale { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Opportunita> Opportunita { get; set; }
    }
}
