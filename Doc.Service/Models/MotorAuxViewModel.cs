using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doc.Service.Models
{
    public class MotorAuxViewModel : BaseViewModel
    {
        public List<AcaoViewModel> Acoes { get; set; }
        public string Url { get; set; }
    }
}
