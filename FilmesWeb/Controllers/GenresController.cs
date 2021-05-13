using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Entidades.Model;
using Persistencia.Repositorio;

namespace FilmesWeb.Controllers
{
    public class GenresController : Controller
    {
        private readonly MovieContext _context;

        public GenresController(MovieContext context)
        {
            _context = context;
        }

        // GET: Genres
        public async Task<IActionResult> Index()
        {
            return View(await _context.Genres.ToListAsync());
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GenreId,Name,Description,RowVersion")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                await _context.AddAsync(genre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        // POST: Genres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,byte[] RowVersion, [Bind("GenreId,Name,Description")] Genre genre)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genreToUpdate = await _context.Genres.FirstOrDefaultAsync(m => m.GenreId == id);

            if (genreToUpdate == null)
            {
                Genre deletedGenre = new();
                await TryUpdateModelAsync(deletedGenre);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The Genre was deleted by another user.");
                return View(deletedGenre);
            }

            _context.Entry(genreToUpdate).Property("RowVersion").OriginalValue =  genre.RowVersion;

            if (await TryUpdateModelAsync<Genre>(
                genreToUpdate,
                "",
                s => s.GenreId, s => s.Name, s => s.Description))
            {
                try
                {
                    await TryUpdateModelAsync<Genre>(genreToUpdate);
                    //_context.Update(genreToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Genre)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The Genre was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Genre)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Name", $"Current value: {databaseValues.Name}");
                        }
                        if (databaseValues.Description != clientValues.Description)
                        {
                            ModelState.AddModelError("Description", $"Current value: {databaseValues.Description}");
                        }

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you got the original value. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to edit this record, click "
                                + "the Save button again. Otherwise click the Back to List hyperlink.");
                        genreToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            return View(genreToUpdate);
        }

        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres.FindAsync(id);

            if (genre == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }
            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Genre genre)
        {
            try
            {
                if (await _context.Genres.AnyAsync(m => m.GenreId == genre.GenreId))
                {
                    _context.Genres.Remove(genre);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = genre.GenreId });
            }
        }

        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.GenreId == id);
        }
    }
}
