using CourseProject.Database;
using CourseProject.Models;
using CourseProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Index(string searchQuery)
        {
            var forms = _context.Forms.Include(f => f.Author).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                forms = forms.Where(f => f.Title.Contains(searchQuery) || f.Description.Contains(searchQuery));
            }

            var model = await forms.ToListAsync();
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(FormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var currentUser = await _userManager.GetUserAsync(User);

            var form = new Form
            {
                Title = model.Title,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                AccessType = model.AccessType,
                Author = currentUser,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var tagName in model.Tags)
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName) ?? new Tag { Name = tagName };
                form.FormTags.Add(new FormTag { Form = form, Tag = tag });
            }

            foreach (var userName in model.AllowedUserNames)
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    form.AllowedUsers.Add(new FormAccess { Form = form, User = user });
                }
            }

            foreach (var questionModel in model.Questions.Select((q, index) => new {q, index}))
            {
                form.Questions.Add(new Question
                {
                    Title = questionModel.q.Title,
                    Type = questionModel.q.Type,
                    ShowInResults = questionModel.q.ShowInResults,
                    Order = questionModel.index
                });
            }

            _context.Forms.Add(form);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var form = await _context.Forms.Include(f => f.Questions.OrderBy(q => q.Order)).Include(f => f.FormAnswers).ThenInclude(fa => fa.Answers).FirstOrDefaultAsync(f => f.Id == id);

            if (form == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var isOwner = form.Author.Id == currentUser.Id;

            var model = new FormDetailsViewModel
            {
                Form = form,
                IsEditable = isOwner || isAdmin,
                UserAnswers = form.FormAnswers.Where(fa => fa.UserId == currentUser?.Id).ToList()
            };

            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> ViewAnswer(int id, string? returnUrl = null)
        {
            var formAnswer = await _context.FormAnswers.Include(fa => fa.Form).ThenInclude(f => f.Questions.OrderBy(q => q.Order)).Include(fa => fa.Answers).FirstOrDefaultAsync(fa => fa.Id == id);

            if (formAnswer == null)
            {
                return NotFound();
            }
            ViewData["returnUrl"] = returnUrl;
            return View(formAnswer);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SubmitForm(int id)
        {
            var form = await _context.Forms.Include(f => f.Questions).FirstOrDefaultAsync(f => f.Id == id);

            if (form == null)
            {
                return NotFound();
            }

            var model = new SubmitFormViewModel
            {
                FormId = form.Id,
                Questions = form.Questions.Select(q => new KeyValuePair<int, QuestionAnswerViewModel>(q.Id, new QuestionAnswerViewModel
                {
                    QuestionId = q.Id,
                    Title = q.Title,
                    Type = q.Type
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

            var formAnswer = new FormAnswer
            {
                FormId = model.FormId,
                UserId = currentUser.Id,
                SubmittedAt = DateTime.UtcNow,
            };

            foreach (var (questionId, userAnswer) in model.Answers)
            {
                KeyValuePair<int, QuestionAnswerViewModel>? question = model.Questions.First(q => q.Key == questionId);

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
                    _ => null
                };

                if (answer != null)
                {
                    formAnswer.Answers.Add(answer);
                }
            }

            _context.FormAnswers.Add(formAnswer);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = model.FormId });
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditForm(int id)
        {
            var form = await _context.Forms.Include(f => f.Questions.OrderBy(q => q.Order)).FirstOrDefaultAsync(f => f.Id == id);

            if (form == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            if (form.Author.Id != currentUser.Id && !isAdmin)
            {
                return Forbid();
            }

            var model = new FormUpdateViewModel
            {
                Id = form.Id,
                Title = form.Title,
                Description = form.Description,
                ImageUrl = form.ImageUrl,
                AccessType = form.AccessType,
                Questions = form.Questions.Select(q => new QuestionUpdateViewModel
                {
                    Id = q.Id,
                    Title = q.Title,
                    Type = q.Type,
                    ShowInResults = q.ShowInResults,
                    Order = q.Order
                }).ToList()
            };

            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditForm(FormUpdateViewModel model)
        {
            var form = await _context.Forms.Include(f => f.Questions).FirstOrDefaultAsync(f => f.Id == model.Id);

            if (form == null)
            {
                return NotFound();
            }

            model.Tags = model.Tags?.Where(tag => !string.IsNullOrWhiteSpace(tag)).ToList() ?? new List<string>();
            model.AllowedUserNames = model.AllowedUserNames?.Where(user => !string.IsNullOrWhiteSpace(user)).ToList() ?? new List<string>();

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            if (form.Author.Id != currentUser.Id && !isAdmin)
            {
                return Forbid();
            }

            form.Title = model.Title;
            form.Description = model.Description;
            form.ImageUrl = model.ImageUrl;
            form.AccessType = model.AccessType;

            model.Tags = model.Tags?.FirstOrDefault()?.Split(',').ToList();
            model.AllowedUserNames = model.AllowedUserNames?.FirstOrDefault()?.Split(',').ToList();

            if(model.Tags != null)
            {
                foreach (var tagName in model.Tags)
                {
                    if (!form.FormTags.Any(x => x.Tag.Name == tagName))
                    {
                        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName) ?? new Tag { Name = tagName };
                        form.FormTags.Add(new FormTag { Form = form, Tag = tag });
                    }
                }
            }

            if(model.AllowedUserNames != null)
            {
                foreach (var userName in model.AllowedUserNames)
                {
                    var user = await _userManager.FindByNameAsync(userName);
                    if (user != null && !form.AllowedUsers.Any(x => x.User == user))
                    {
                        form.AllowedUsers.Add(new FormAccess { Form = form, User = user });
                    }
                }
            }

            var existingQuestions = form.Questions.ToDictionary(q => q.Id);
            foreach (var questionModel in model.Questions)
            {
                if (existingQuestions.TryGetValue(questionModel.Id, out var existingQuestion))
                {
                    existingQuestion.Title = questionModel.Title;
                    existingQuestion.Type = questionModel.Type;
                    existingQuestion.Order = questionModel.Order;
                }
                else
                {
                    form.Questions.Add(new Question
                    {
                        Title = questionModel.Title,
                        Type = questionModel.Type,
                        Order = questionModel.Order,
                        FormId = form.Id
                    });
                }
            }

            var questionIdsToKeep = model.Questions.Select(q => q.Id).ToHashSet();
            var questionsToRemove = form.Questions.Where(q => !questionIdsToKeep.Contains(q.Id)).ToList();
            _context.Questions.RemoveRange(questionsToRemove);

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = form.Id });
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Statistics(int id)
        {
            var form = await _context.Forms.Include(f => f.Questions).Include(f => f.FormAnswers).ThenInclude(fa => fa.Answers).FirstOrDefaultAsync(f => f.Id == id);
            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            if (!isAdmin && currentUser != form.Author)
            {
                return Forbid();
            }

            if (form == null)
            {
                return NotFound();
            }


            var statistics = form.Questions.Select(q => new QuestionStatisticsViewModel
            {
                Question = q,
                AverageValue = q.Type == QuestionType.Integer ? form.FormAnswers
                    .SelectMany(fa => fa.Answers.OfType<IntegerAnswer>())
                    .Where(a => a.QuestionId == q.Id)
                    .Select(a => (double)a.IntegerAnswerValue)
                    .DefaultIfEmpty()
                    .Average() : (double?)null,
                MostFrequentAnswer = q.Type == QuestionType.SingleLineText ? form.FormAnswers
                    .SelectMany(fa => fa.Answers.OfType<TextAnswer>())
                    .Where(a => a.QuestionId == q.Id)
                    .GroupBy(a => a.TextAnswerValue)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault() : null
            }).ToList();

            return View(statistics);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteAnswer(int id, string? returnUrl = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var formAnswer = await _context.FormAnswers.Include(fa => fa.Form).Include(fa => fa.Answers).FirstOrDefaultAsync(fa => fa.Id == id);

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            if (!isAdmin && currentUser != formAnswer.User)
            {
                return Forbid();
            }

            if (formAnswer == null)
            {
                return NotFound();
            }

            _context.Answers.RemoveRange(formAnswer.Answers);
            _context.FormAnswers.Remove(formAnswer);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Details", "Form", new { id = formAnswer.FormId });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteForm(int id, string? returnUrl = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var formAnswer = await _context.FormAnswers.Include(fa => fa.Form).Include(fa => fa.Answers).FirstOrDefaultAsync(fa => fa.Id == id);

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            if (!isAdmin && currentUser != formAnswer.User)
            {
                return Forbid();
            }

            if (formAnswer == null)
            {
                return NotFound();
            }

            _context.Answers.RemoveRange(formAnswer.Answers);
            //_context.FormAnswers.Remove(formAnswer);
            _context.Forms.Remove(formAnswer.Form);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Account");
        }
    }

}
