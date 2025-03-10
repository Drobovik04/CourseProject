﻿using CourseProject.Database;
using CourseProject.Models;
using CourseProject.SupportClasses.Form;
using CourseProject.Utilities;
using CourseProject.ViewModels.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CourseProject.Controllers
{
    public class FormController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly MailSender _mailSender;

        public FormController(AppDbContext context, UserManager<AppUser> userManager, MailSender mailSender)
        {
            _context = context;
            _userManager = userManager;
            _mailSender = mailSender;
        }

        [Authorize]
        public async Task<IActionResult> ViewAnswer(int id, string? returnUrl = null)
        {
            var form = await _context.Forms
                .Include(x => x.User)
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
            return View(new FormViewAnswerViewModel
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

            var currentUser = await _userManager.GetUserAsync(User);

            if (template.AccessType == AccessType.Restricted)
            {
                var isUserCanFillForm = template.AllowedUsers.Any(x => x.Template.Id == template.Id && x.User.Id == currentUser.Id);

                if (!isUserCanFillForm)
                {
                    return Forbid();
                }
            }

            var model = new FormSubmitFormViewModel
            {
                TemplateId = template.Id,
                Questions = template.Questions.Select(x => new KeyValuePair<int, QuestionAnswer>(x.Id, new QuestionAnswer
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
        public async Task<IActionResult> SubmitForm(FormSubmitFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var template = await _context.Templates
                .Include(x => x.Questions.OrderBy(x => x.Order))
                .Include(x => x.Author)
                .Include(x => x.Likes)
                .Include(x => x.Topic)
                .Include(x => x.Comments)
                    .ThenInclude(x => x.User)
                .Include(x => x.Forms)
                    .ThenInclude(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == model.TemplateId);

            if (template == null)
            {
                return NotFound();
            }

            var isAnswerExists = template.Forms.FirstOrDefault(x => x.TemplateId == template.Id && x.User == currentUser) != null;

            if (isAnswerExists)
            {
                return Forbid();
            }

            var form = new Form
            {
                TemplateId = model.TemplateId,
                UserId = currentUser.Id,
                SubmittedAt = DateTime.UtcNow,
            };

            foreach (var (questionId, userAnswer) in model.Answers)
            {
                KeyValuePair<int, QuestionAnswer>? question = model.Questions.First(x => x.Key == questionId);

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
                    .Include(x => x.User)
                    .Include(x => x.Answers)
                        .ThenInclude(x => x.Question)
                    .FirstOrDefault(x => x.Id == form.Id);

                var htmlContent = await HtmlGenerator.HtmlPage("_UserAnswersPartial", formToSend, HttpContext, RouteData, ControllerContext);
                var htmlBytes = Encoding.UTF8.GetBytes(htmlContent);
                var pdfBytes = new NReco.PdfGenerator.HtmlToPdfConverter().GeneratePdf(htmlContent);
                await _mailSender.SendMessage(formToSend.User.Email, pdfBytes, htmlBytes);
            }

            return RedirectToAction("Info", "Template", new { id = model.TemplateId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateAnswer([FromBody] FormUpdateAnswerViewModel model)
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

            return RedirectToAction("Info", "Template", new { id = form.TemplateId });
        }
    }

}
