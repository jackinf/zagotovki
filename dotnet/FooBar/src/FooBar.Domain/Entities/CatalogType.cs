﻿using System.ComponentModel.DataAnnotations.Schema;
 using FooBar.Domain.Interfaces;

 namespace FooBar.Domain.Entities
{
    [Table("CatalogType")]
    public class CatalogType : BaseEntity, IAggregateRoot
    {
        public string Type { get; private set; }
        public CatalogType(string type)
        {
            Type = type;
        }
    }
}
