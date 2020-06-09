using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vectio.eventmanagement.api.db.entities;

namespace vectio.eventmanagement.api.controllers
{
    public class ExtendedController : ControllerBase
    {
        protected IActionResult Json(JsonQuery data)
        {
          ;
            if (data == null ||data.JsonData==null)
                data.JsonData = "[]";
            
            return Content(data.JsonData, "application/json");
        }
        protected async Task<IActionResult> JsonAsync(IQueryable<JsonQuery> query)
        {
            var data = await query.ToListAsync();
            if (data==null || !data.Any() || data[0] == null)
                data = new List<JsonQuery>();
            var result = string.Join("", (data).Select(x => x.JsonData)) ?? "[]";
            return Content(string.IsNullOrWhiteSpace(result)?"[]":result, "application/json");
        }
    }
}
