using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Website.Models;

namespace Website.ViewModels
{
    public class FilterViewModel
    {
        public FilterViewModel(List<Company> companies, int? company, string name)
        {
            companies.Insert(0, new Company { Name = "Все", Id = 0 });
            Companies = new SelectList(companies, "Id", "Name", company);
            SelectedCompany = company;
            SelectedName = name;
        }
        public SelectList Companies { get; private set; }
        public int? SelectedCompany { get; private set; }
        public string SelectedName { get; private set; }
    }
}