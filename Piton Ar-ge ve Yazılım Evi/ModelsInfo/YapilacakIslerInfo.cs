using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Piton_Ar_ge_ve_Yazılım_Evi.ModelsInfo
{
    public class YapilacakIslerInfo
    {
        public Guid Id { get; set; }
        public string YapilacakIs { get; set; }
        public string BaslangicTarih { get; set; }
        public string BitisTarihi { get; set; }
        public int Deleted { get; set; }
        public int Drum { get; set; }
    }
}