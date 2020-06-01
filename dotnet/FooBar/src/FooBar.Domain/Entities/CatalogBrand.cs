﻿using System.ComponentModel.DataAnnotations.Schema;
 using FooBar.Domain.Interfaces;

 namespace FooBar.Domain.Entities
{
    [Table("CatalogBrand")]
    public class CatalogBrand : BaseEntity, IAggregateRoot
    {
        public string Brand { get; private set; }
        public CatalogBrand(string brand)
        {
            Brand = brand;
        }
    }
}
