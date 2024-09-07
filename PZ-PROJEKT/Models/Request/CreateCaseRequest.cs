using Microsoft.AspNetCore.Mvc;
using PZ_PROJEKT.Models.Request;

namespace PZ_PROJEKT.Models
{
    public class CreateCaseRequest
    {
        public string Name { get; set; }
        public List<CreateCaseRequestItem> Items { get; set; }
    }
}
