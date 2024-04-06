using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using TicketProject.Models;
using TicketProject.Utility;

namespace TicketProject.Controllers
{
    public class UserController : Controller
    {
        private UserManager<AppUser> userManager;
        private IPasswordHasher<AppUser> passwordHasher;
        private IWebHostEnvironment webHostEnvironment;
        public UserController(UserManager<AppUser> userManager,IPasswordHasher<AppUser> passwordHasher,
            IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.userManager = userManager;
            this.passwordHasher = passwordHasher;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid) 
            {
                string webRootPath = webHostEnvironment.WebRootPath;
                var i = Image.FromFile(webRootPath + PathManager.TicketImagePath);
                Bitmap bitmap = new Bitmap(i);
                Graphics graphics = Graphics.FromImage(bitmap);
                Brush brush = new SolidBrush(Color.Black);

                // Define text font
                Font arial = new Font("Arial", 30, FontStyle.Regular);

                // Text to display
                string text = model.FirstName+" " + model.LastName;

                // Define rectangle
                Rectangle rectangle = new Rectangle(420, 150, 1600, 150);

                // Specify rectangle border
                Pen pen = new Pen(Color.White, 2);

                // Draw rectangle
                graphics.DrawRectangle(pen, rectangle);


                // Draw text on image
                graphics.DrawString(text, arial, brush, rectangle);
                string fileName = Guid.NewGuid().ToString();
                string extension = ".png";
                
                string path = Path.Combine(webRootPath + PathManager.TicketUserImagePath, fileName) + extension;
                // Save the output file
                bitmap.Save(path,ImageFormat.Png);
                AppUser appUser = new AppUser() 
                {
                    UserName=model.Name,FirstName=model.FirstName,LastName=model.LastName,Email=model.Email,Image= fileName
                };

                IdentityResult result = await userManager.CreateAsync(appUser, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("TicketPage",appUser);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                }
            }
            return View(model);
        }
        public IActionResult TicketPage(AppUser model) 
        {
            
            
           
            return View(model);
        }
    }
}
