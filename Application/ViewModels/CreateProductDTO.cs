﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class CreateProductDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name can't be longer than 100 characters")]
        public string NameProduct { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string DescriptionProduct { get; set; }
        public string? Key { get; set; }
        [Required(ErrorMessage = "Price is required")]
        [ValidateType(typeof(double), ErrorMessage = "Price must be a valid number")]
        [NonNegative(ErrorMessage = "Price must be a non-negative number")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [NonNegative(ErrorMessage = "Quantity must be a non-negative integer")]
        [ValidateType(typeof(int), ErrorMessage = "Quantity must be a valid number")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Color ID is required")]
        [ValidateType(typeof(int), ErrorMessage = "Color ID must be an integer")]
        public int ColorId { get; set; }

        [Required(ErrorMessage = "Material ID is required")]
        [ValidateType(typeof(int), ErrorMessage = "Material ID must be an integer")]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "TypeProduct ID is required")]
        [ValidateType(typeof(int), ErrorMessage = "TypeProduct ID must be an integer")]
        public int TypeProductId { get; set; }
    }
}