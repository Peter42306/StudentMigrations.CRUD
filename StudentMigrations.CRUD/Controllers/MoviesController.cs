using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMigrations.CRUD.Data;
using StudentMigrations.CRUD.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace StudentMigrations.CRUD.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MoviesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Director,ProductionYear,Description,PosterUrl")] Movie movie,IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                movie.PosterUrl=await UploadPicture(uploadedFile);

                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Director,ProductionYear,Description,PosterUrl")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }

        /// <summary>
        /// Вспомогательный метод для загрузки картинок, с уменьшением размера фото
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        private async Task<string> UploadPicture(IFormFile uploadedFile)
        {
            if (uploadedFile==null || uploadedFile.Length==0)
            {
                throw new ArgumentException("File was not loaded or empty");
            }

            // Путь к папке, где будут храниться изображения
            string uploadedFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "image");

            // Создаем папку, если она не существует
            if (!Directory.Exists(uploadedFilePath))
            {
                Directory.CreateDirectory(uploadedFilePath);
            }

            // Генерируем новое уникальное имя файла для изображения
            string newFileNameGenerated = Guid.NewGuid().ToString()+"_"+uploadedFile.FileName;

            // Полный путь к файлу на сервере
            string fileFullPath=Path.Combine(uploadedFilePath, newFileNameGenerated);

            // Сохраняем файл на сервере
            try
            {
                using (var fileStream = new FileStream(fileFullPath, FileMode.Create))
                {
                    // Загрузка изображения
                    using (var image = await SixLabors.ImageSharp.Image.LoadAsync(uploadedFile.OpenReadStream()))
                    {
                        // Resize the image to a maximum width and height of 800px
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Mode=ResizeMode.Max,
                            Size=new SixLabors.ImageSharp.Size(800, 600) //Size=new Size(image.Width, image.Height)
                        }));

                        await image.SaveAsJpegAsync(fileStream);
                    }
                }
            }
            catch (Exception)
            {

                throw new Exception("Error during loading of image");
            }

            // Устанавливаем путь к изображению в объекте фильма
            return Path.Combine("image", newFileNameGenerated);            
        }
    }
}
