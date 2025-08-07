using Example_POS.Controllers;
using Example_POS.Data;
using Example_POS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example_POS.ViewComponents
{
    public class DropdownViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext _db;
        public DropdownViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(BaseConst.SYS_FLEX_CODE flexCode)
        {
            var data = await _db.SysFlexItem
                .FromSqlRaw("""
                                SELECT i.* 
                                FROM SysFlex f
                                INNER JOIN SysFlexItem i ON f.FlexId = i.FlexId
                                WHERE f.FlexCode = {0}
                            """, flexCode.ToString())
                .ToListAsync();
            return View(data);
        }
    }
}
