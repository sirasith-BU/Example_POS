using Example_POS.Data;
using Example_POS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example_POS.Controllers
{
    public class FlexItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        public FlexItemController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<SysFlexItem>> Color()
        {
            var data = await _db.SysFlex.Include(i => i.SysFlexItem).Where(f => f.FlexCode == BaseConst.SYS_FLEX_CODE.COLOR.ToString()).SelectMany(s => s.SysFlexItem).ToListAsync();
            return data;
        }
    }


}
