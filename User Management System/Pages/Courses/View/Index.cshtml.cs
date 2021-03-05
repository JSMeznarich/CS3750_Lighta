using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightaplusplus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lightaplusplus.Pages.Courses.View
{
    public class IndexModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public IndexModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public int SectionId { get; set; }

        public Users Users { get; set; }

        public Sections Section { get; set; }

        public List<Assignments> Assignments { get; set; }

        public async Task<IActionResult> OnGetAsync(int sectionId, int? id)
        {
            SectionId = sectionId;

            if (id == null)
            {
                return RedirectToPage("/Index");
            }

            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            if (Users == null)
            {
                return RedirectToPage("/Index");
            }

            if (Users.usertype == 'I')
            {
                Section = await _context.Sections.Include(s => s.Course).Where(s => s.InstructorId == Users.ID).FirstOrDefaultAsync(s => s.SectionId == SectionId);
            }
            else if (Users.usertype == 'S')
            {
                var StudentSections = await _context.SectionStudents.Where(ss => ss.StudentId == Users.ID).FirstOrDefaultAsync(ss => ss.SectionId == SectionId);
                if (StudentSections == null)
                {
                    return RedirectToPage("/Welcome", new { id = id });
                }

                Section = await _context.Sections.Include(s => s.Course).FirstOrDefaultAsync(s => s.SectionId == SectionId);
            }


            if (Section == null)
            {
                if(Users.usertype == 'S')
                {
                    return RedirectToPage("/Welcome", new { id = id });
                }
                else
                {
                    return RedirectToPage("/Courses/Index", new { id = id });
                }
            }

            Assignments = await _context.Assignments.Where(a => a.SectionId == SectionId).ToListAsync();

            return Page();
        }
    }
}