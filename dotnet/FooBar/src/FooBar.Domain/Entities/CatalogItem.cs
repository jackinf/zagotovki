﻿using System;
using FooBar.Domain.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace FooBar.Domain.Entities
{
    public class CatalogItem : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public string PictureUri { get; private set; }
        public int CatalogTypeId { get; private set; }
        public CatalogType CatalogType { get; private set; }
        public int CatalogBrandId { get; private set; }
        public CatalogBrand CatalogBrand { get; private set; }

        public CatalogItem(int catalogTypeId, int catalogBrandId, string description, string name, decimal price, string pictureUri)
        {
            CatalogTypeId = catalogTypeId;
            CatalogBrandId = catalogBrandId;
            Description = description;
            Name = name;
            Price = price;
            PictureUri = pictureUri;
        }

        public void Update(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"Required input {nameof(name)} was empty.", nameof(name));
            }
            Name = name;
            Price = price;
        }
    }
}