using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;

namespace UdemyRealWorldUnitTest.WEB.Models;

public partial class Product
{
    public int Id { get; set; }

    [Required] //Zorunlu tuttuk
    public string? Name { get; set; }

    [Required] //Zorunlu tuttuk
    public decimal? Price { get; set; }

    [Required] //Zorunlu tuttuk
    public int? Stock { get; set; }

    [Required] //Zorunlu tuttuk
    public string? Color { get; set; }
}
