using Azure.Core;
using CourseProject.Database;
using CourseProject.Models;
using CourseProject.ViewModels;
using CourseProject.ViewModels.Form;
using CourseProject.ViewModels.Template;
using iText.Html2pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace CourseProject.Controllers
{
    public class FormController:Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FormController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> ViewAnswer(int id, string? returnUrl = null)
        {
            var form = await _context.Forms
                .Include(x => x.Template)
                    .ThenInclude(x => x.Questions.OrderBy(x => x.Order))
                .Include(x => x.Answers.OrderBy(x => x.Question.Order))
                .FirstOrDefaultAsync(x => x.Id == id);

            if (form == null)
            {
                return NotFound();
            }
            var questionAnswerMap = form.Template.Questions
                .Select(q => new QuestionAnswerPair
                {
                    Question = q,
                    Answer = form.Answers.FirstOrDefault(a => a.QuestionId == q.Id)
                })
                .ToList();

            ViewData["returnUrl"] = returnUrl;
            return View(new ViewAnswerViewModel
            {
                Template = form.Template,
                Form = form,
                SubmittedAt = form.SubmittedAt,
                UserId = form.UserId,
                QuestionAnswerMap = questionAnswerMap
            });
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SubmitForm(int id)
        {
            var template = await _context.Templates
                .Include(x => x.Questions)
                .Include(x => x.AllowedUsers)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User); ;

            if (template.AccessType == AccessType.Restricted)
            {
                var isUserCanFillForm = template.AllowedUsers.Any(x => x.Template.Id == template.Id && x.User.Id == currentUser.Id);

                if (!isUserCanFillForm)
                {
                    return Forbid();
                }
            }

            var model = new SubmitFormViewModel
            {
                TemplateId = template.Id,
                Questions = template.Questions.Select(x => new KeyValuePair<int, QuestionAnswerViewModel>(x.Id, new QuestionAnswerViewModel
                {
                    QuestionId = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Type = x.Type
                })).ToDictionary()
            };

            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SubmitForm(SubmitFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var form = new Form
            {
                TemplateId = model.TemplateId,
                UserId = currentUser.Id,
                SubmittedAt = DateTime.UtcNow,
            };

            foreach (var (questionId, userAnswer) in model.Answers)
            {
                KeyValuePair<int, QuestionAnswerViewModel>? question = model.Questions.First(x => x.Key == questionId);

                if (question == null)
                {
                    continue;
                }

                Answer answer = question.Value.Value.Type switch
                {
                    QuestionType.SingleLineText => new TextAnswer
                    {
                        QuestionId = question.Value.Value.QuestionId,
                        TextAnswerValue = userAnswer.Answer
                    },
                    QuestionType.MultiLineText => new TextAnswer
                    {
                        QuestionId = question.Value.Value.QuestionId,
                        TextAnswerValue = userAnswer.Answer
                    },
                    QuestionType.Integer => new IntegerAnswer
                    {
                        QuestionId = question.Value.Value.QuestionId,
                        IntegerAnswerValue = int.TryParse(userAnswer.Answer, out var intValue) ? intValue : 0
                    },
                    QuestionType.Checkbox => new CheckboxAnswer
                    {
                        QuestionId = question.Value.Value.QuestionId,
                        CheckboxAnswerValue = bool.TryParse(userAnswer.Answer, out var boolValue) && boolValue
                    },
                    _ => null!
                };

                if (answer != null)
                {
                    form.Answers.Add(answer);
                }
            }

            _context.Forms.Add(form);
            await _context.SaveChangesAsync();

            if (model.SendCopyOnEmail)
            {
                var formToSend = _context.Forms
                    .Include(x => x.Template)
                    .Include(x => x.Answers)
                        .ThenInclude(x => x.Question)
                    .FirstOrDefault(x => x.Id == form.Id);
                var htmlContext = await RenderViewToStringAsync("_UserAnswersPartial", formToSend);
                htmlContext = await InlineStylesAsync(htmlContext);
                System.IO.File.WriteAllText("testhtml.html", htmlContext);
                //var pdfDinkToPDF = _pdfService.GeneratePdf(htmlContext);
                var pdfPDFSharp = PdfGenerator.GeneratePdf(htmlContext, PdfSharp.PageSize.A4);
                pdfPDFSharp.Save("testPDFSharp.pdf");
                HtmlConverter.ConvertToPdf(htmlContext, new FileStream("textIText.pdf", FileMode.Create));

                //System.IO.File.WriteAllBytes("testpdf.pdf", pdf);
                //await new BrowserFetcher().DownloadAsync();
                //var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
                //var page = await browser.NewPageAsync();
                //await page.SetContentAsync(htmlContext);
                //await page.PdfAsync("pdfPuppetSharp.pdf");
            }

            return RedirectToAction("Details", "Template", new { id = model.TemplateId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateAnswer([FromBody] UpdateAnswerViewModel model)
        {
            var form = await _context.Forms
                .Include(x => x.Answers)
                .Include(x => x.Template.Questions)
                .FirstOrDefaultAsync(x => x.Id == model.FormId);

            var currentUser = await _userManager.GetUserAsync(User);

            if (form == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin") && form.UserId != currentUser.Id)
            {
                return Forbid();
            }

            foreach (var updatedAnswer in model.Answers)
            {
                var answer = form.Answers.FirstOrDefault(a => a.Id == updatedAnswer.Id);
                if (answer != null)
                {
                    switch (updatedAnswer.Type)
                    {
                        case "TextAnswer":
                            if (answer is TextAnswer textAnswer)
                            {
                                textAnswer.TextAnswerValue = updatedAnswer.Value.ToString();

                            }
                            break;
                        case "Integer":
                            if (answer is IntegerAnswer integerAnswer)
                            {
                                integerAnswer.IntegerAnswerValue = int.Parse(updatedAnswer.Value.ToString());

                            }
                            break;
                        case "Checkbox":
                            if (answer is CheckboxAnswer checkboxAnswer)
                            {
                                checkboxAnswer.CheckboxAnswerValue = bool.Parse(updatedAnswer.Value.ToString());

                            }
                            break;
                    }
                }
            }

            foreach (var newAnswer in model.NewAnswers)
            {
                var question = form.Template.Questions.FirstOrDefault(x => x.Id == newAnswer.QuestionId);
                if (question != null)
                {
                    Answer answer = newAnswer.Type switch
                    {
                        "TextAnswer" => new TextAnswer { Question = question, TextAnswerValue = newAnswer.Value.ToString() },
                        "Integer" => new IntegerAnswer { Question = question, IntegerAnswerValue = int.Parse(newAnswer.Value.ToString()) },
                        "Checkbox" => new CheckboxAnswer { Question = question, CheckboxAnswerValue = bool.Parse(newAnswer.Value.ToString()) },
                        _ => null!
                    };
                    form.Answers.Add(answer);
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteAnswer(int id, string? returnUrl = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var form = await _context.Forms
                .Include(x => x.User)
                .Include(x => x.Template)
                .Include(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == id);

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            if (!isAdmin && currentUser != form.User)
            {
                return Forbid();
            }

            if (form == null)
            {
                return NotFound();
            }

            _context.Answers.RemoveRange(form.Answers);
            _context.Forms.Remove(form);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Details", "Template", new { id = form.TemplateId });
        }

        private void SendCopyForm(string email, Form form)
        {
            
        }
        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            var viewEngine = HttpContext.RequestServices.GetService(typeof(IRazorViewEngine)) as IRazorViewEngine;
            var tempDataProvider = HttpContext.RequestServices.GetService(typeof(ITempDataProvider)) as ITempDataProvider;
            var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor);

            using var sw = new StringWriter();
            var viewResult = viewEngine.FindView(actionContext, viewName, false);

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var tempData = new TempDataDictionary(actionContext.HttpContext, tempDataProvider);
            var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary, tempData, sw, new HtmlHelperOptions());
            await viewResult.View.RenderAsync(viewContext);

            return sw.ToString();
        }

        public static async Task<string> InlineStylesAsync(string htmlContent)
        {
            var httpClient = new HttpClient();

            // Найти все <link rel="stylesheet" href="..."> в HTML
            var linkRegex = new Regex("<link[^>]*rel=\"stylesheet\"[^>]*href=\"([^\"]+)\"[^>]*>", RegexOptions.IgnoreCase);
            var matches = linkRegex.Matches(htmlContent);

            var inlinedStyles = new StringBuilder();

            foreach (Match match in matches)
            {
                var href = match.Groups[1].Value;

                try
                {
                    string cssContent;
                    if (href.StartsWith("http"))
                    {
                        cssContent = await httpClient.GetStringAsync(href);
                    }
                    else
                    {
                        cssContent = System.IO.File.ReadAllText(Path.Combine(AppContext.BaseDirectory, href));
                    }

                    inlinedStyles.AppendLine(cssContent);

                    htmlContent = htmlContent.Replace(match.Value, string.Empty);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка загрузки стиля: {href}. {ex.Message}");
                }
            }

            // Вставить стили в тег <style> в <head>
            var styleTag = $"<style>{inlinedStyles}</style>";
            htmlContent = htmlContent.Replace("</head>", $"{styleTag}</head>");

            return htmlContent;
        }
    }

}
