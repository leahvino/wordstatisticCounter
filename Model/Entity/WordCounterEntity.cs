using System;
using System.Collections.Generic;
using System.Text;


// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable
namespace Model.Entity
{
    public partial class WordCounter
    {
        public int Id { get; set; }
        public string WordName { get; set; }
        public int? Counter { get; set; }
    }
}

