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
    
    public partial class Azione
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Azione()
        {
            this.Opportunita = new HashSet<Opportunita>();
        }
    
        public int id { get; set; }
        public string tipologia { get; set; }
        public Nullable<int> ordinale { get; set; }
        public bool adesione { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Opportunita> Opportunita { get; set; }
    }
}
