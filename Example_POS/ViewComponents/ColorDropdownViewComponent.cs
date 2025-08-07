using Example_POS.Controllers;
using Example_POS.Data;
using Example_POS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example_POS.ViewComponents
{
    public class ColorDropdownViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext _db;
        public ColorDropdownViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await _db.SysFlex.Include(i => i.SysFlexItem).Where(f => f.FlexCode == BaseConst.SYS_FLEX_CODE.COLOR.ToString()).SelectMany(s => s.SysFlexItem).ToListAsync();
            return View(data);
        }

    }
}
