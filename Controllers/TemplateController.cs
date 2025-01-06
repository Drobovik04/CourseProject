using CourseProject.Database;
using CourseProject.Models;
using CourseProject.SupportClasses.Template;
using CourseProject.SupportClasses.Template.Statistics;
using CourseProject.Utilities;
using CourseProject.ViewModels;
using CourseProject.ViewModels.Template;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CourseProject.Controllers
{
    public class TemplateController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TemplateController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchQuery = "", string sortColumn = "Title", string sortOrder = "asc", string viewMode = "Table")
        {
            var templates = _context.Templates
                .Include(x => x.Author)
                .Include(x => x.Topic)
                .Include(x => x.Comments)
                .Include(x => x.Likes)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                var searchProcedure = _context.Database.SqlQuery<int>($"EXEC SearchTemplates {"\"" + searchQuery + "\""}").ToList();
                templates = templates.Where(x => searchProcedure.Contains(x.Id));
            }

            templates = sortColumn switch
            {
                "Title" => sortOrder == "asc" ? templates.OrderBy(x => x.Title) : templates.OrderByDescending(x => x.Title),
                "Topic" => sortOrder == "asc" ? templates.OrderBy(x => x.Topic.Name) : templates.OrderByDescending(x => x.Topic.Name),
                "Author" => sortOrder == "asc" ? templates.OrderBy(x => x.Author.UserName) : templates.OrderByDescending(x => x.Author.UserName),
                "Comments" => sortOrder == "asc" ? templates.OrderBy(x => x.Comments.Count) : templates.OrderByDescending(x => x.Comments.Count),
                "Likes" => sortOrder == "asc" ? templates.OrderBy(x => x.Likes.Count) : templates.OrderByDescending(x => x.Likes.Count),
                "CreatedAt" => sortOrder == "asc" ? templates.OrderBy(x => x.CreatedAt) : templates.OrderByDescending(x => x.CreatedAt),

                _ => templates
            };

            var model = new TemplateIndexViewModel
            {
                Templates = await templates.ToListAsync(),
                CurrentViewMode = viewMode,
                SortColumn = sortColumn,
                SortOrder = sortOrder
            };
            return View(model);
        }

        public async Task<IActionResult> Search(string searchQuery = "", string sortColumn = "Title", string sortOrder = "asc", string viewMode = "Table", bool byTagOnly = false)
        {
            var templates = _context.Templates
                .Include(x => x.Author)
                .Include(x => x.Topic)
                .Include(x => x.Comments)
                .Include(x => x.Likes)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                if (byTagOnly)
                {
                    templates = templates.Where(x => x.TemplateTags.Where(y => y.Tag.Name == searchQuery).Count() != 0);
                }
                else
                {
                    var searchProcedure = _context.Database.SqlQuery<int>($"EXEC SearchTemplates {"\"" + searchQuery + "\""}").ToList();
                    templates = templates.Where(x => searchProcedure.Contains(x.Id));
                }
            }

            templates = sortColumn switch
            {
                "Title" => sortOrder == "asc" ? templates.OrderBy(x => x.Title) : templates.OrderByDescending(x => x.Title),
                "Topic" => sortOrder == "asc" ? templates.OrderBy(x => x.Topic.Name) : templates.OrderByDescending(x => x.Topic.Name),
                "Author" => sortOrder == "asc" ? templates.OrderBy(x => x.Author.UserName) : templates.OrderByDescending(x => x.Author.UserName),
                "Comments" => sortOrder == "asc" ? templates.OrderBy(x => x.Comments.Count) : templates.OrderByDescending(x => x.Comments.Count),
                "Likes" => sortOrder == "asc" ? templates.OrderBy(x => x.Likes.Count) : templates.OrderByDescending(x => x.Likes.Count),
                "CreatedAt" => sortOrder == "asc" ? templates.OrderBy(x => x.CreatedAt) : templates.OrderByDescending(x => x.CreatedAt),

                _ => templates
            };

            var model = new TemplateSearchViewModel
            {
                Templates = await templates.ToListAsync(),
                CurrentViewMode = viewMode,
                SortColumn = sortColumn,
                SortOrder = sortOrder,
                ByTagOnly = byTagOnly,
                SearchQuery = searchQuery
            };
            return View(model);
        }

        public async Task<IActionResult> Main(string searchQuery, string sortColumnTab1 = "Title", string sortOrderTab1 = "asc", string sortColumnTab2 = "Title", string sortOrderTab2 = "asc", string viewModeTab1 = "Table", string viewModeTab2 = "Card", string currentTab = "LatestTemplates")
        {
            var latestTemplates = _context.Templates
                .Include(x => x.Author)
                .Include(x => x.Topic)
                .Include(x => x.Comments)
                .Include(x => x.Likes)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                var searchProcedure = _context.Database.SqlQuery<int>($"EXEC SearchTemplates {"\"" + searchQuery + "\""}").ToList();
                latestTemplates = latestTemplates.Where(x => searchProcedure.Contains(x.Id));
            }

            latestTemplates = sortColumnTab1 switch
            {
                "Title" => sortOrderTab1 == "asc" ? latestTemplates.OrderBy(x => x.Title) : latestTemplates.OrderByDescending(x => x.Title),
                "Topic" => sortOrderTab1 == "asc" ? latestTemplates.OrderBy(x => x.Topic.Name) : latestTemplates.OrderByDescending(x => x.Topic.Name),
                "Author" => sortOrderTab1 == "asc" ? latestTemplates.OrderBy(x => x.Author.UserName) : latestTemplates.OrderByDescending(x => x.Author.UserName),
                "Comments" => sortOrderTab1 == "asc" ? latestTemplates.OrderBy(x => x.Comments.Count) : latestTemplates.OrderByDescending(x => x.Comments.Count),
                "Likes" => sortOrderTab1 == "asc" ? latestTemplates.OrderBy(x => x.Likes.Count) : latestTemplates.OrderByDescending(x => x.Likes.Count),
                "CreatedAt" => sortOrderTab1 == "asc" ? latestTemplates.OrderBy(x => x.CreatedAt) : latestTemplates.OrderByDescending(x => x.CreatedAt),

                _ => latestTemplates
            };

            var fiveMostPopularTemplates = _context.Templates
                .Include(x => x.Author)
                .Include(x => x.Topic)
                .Include(x => x.Comments)
                .Include(x => x.Likes)
                .Include(x => x.Forms)
                .OrderByDescending(x => x.Forms.Count)
                .Take(5)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                var searchProcedure = _context.Database.SqlQuery<int>($"EXEC SearchTemplates {"\"" + searchQuery + "\""}").ToList();
                fiveMostPopularTemplates = fiveMostPopularTemplates.Where(x => searchProcedure.Contains(x.Id));
            }

            fiveMostPopularTemplates = sortColumnTab2 switch
            {
                "Title" => sortOrderTab2 == "asc" ? fiveMostPopularTemplates.OrderBy(x => x.Title) : fiveMostPopularTemplates.OrderByDescending(x => x.Title),
                "Topic" => sortOrderTab2 == "asc" ? fiveMostPopularTemplates.OrderBy(x => x.Topic.Name) : fiveMostPopularTemplates.OrderByDescending(x => x.Topic.Name),
                "Author" => sortOrderTab2 == "asc" ? fiveMostPopularTemplates.OrderBy(x => x.Author.UserName) : fiveMostPopularTemplates.OrderByDescending(x => x.Author.UserName),
                "Comments" => sortOrderTab2 == "asc" ? fiveMostPopularTemplates.OrderBy(x => x.Comments.Count) : fiveMostPopularTemplates.OrderByDescending(x => x.Comments.Count),
                "Likes" => sortOrderTab2 == "asc" ? fiveMostPopularTemplates.OrderBy(x => x.Likes.Count) : fiveMostPopularTemplates.OrderByDescending(x => x.Likes.Count),
                "CreatedAt" => sortOrderTab2 == "asc" ? fiveMostPopularTemplates.OrderBy(x => x.CreatedAt) : fiveMostPopularTemplates.OrderByDescending(x => x.CreatedAt),

                _ => fiveMostPopularTemplates
            };

            var tagInfo = await _context.Tags
                .Select(x => new TagForCloud()
                {
                    Tag = x,
                    Frequency = x.TemplateTags.Count()
                })
                .Where(x => x.Frequency != 0)
                .ToListAsync();

            var model = new TemplateMainViewModel
            {
                LatestTemplates = await latestTemplates.ToListAsync(),
                FiveMostPopularTemplates = await fiveMostPopularTemplates.ToListAsync(),
                TagCloud = tagInfo,
                SortColumnTab1 = sortColumnTab1,
                SortColumnTab2 = sortColumnTab2,
                SortOrderTab1 = sortOrderTab1,
                SortOrderTab2 = sortOrderTab2,
                CurrentTab = currentTab,
                ViewModeTab1 = viewModeTab1,
                ViewModeTab2 = viewModeTab2
            };
            return View(model);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new TemplateCreateViewModel
            {
                Topics = await GetTopicsAsync(),
                AllowedUserNames = null,
                Tags = null
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(TemplateCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var currentUser = await _userManager.GetUserAsync(User);

            string? uploadedImageUrl = null;

            if (model.ImageFile != null)
            {
                uploadedImageUrl = await UploadImageToImgBB(model.ImageFile);
            }

            var template = new Template
            {
                Title = model.Title,
                Description = MarkdownHelper.ExtractPlainText(model.Description),
                DescriptionWithMarkdown = model.Description,
                ImageUrl = uploadedImageUrl,
                AccessType = model.AccessType,
                Author = currentUser,
                CreatedAt = DateTime.UtcNow,
                TopicId = model.TopicId
            };

            model.Tags = model.Tags?.FirstOrDefault()?.Split(',').Select(x => x.Trim()).Where(x => x.Length != 0).ToList();
            model.AllowedUserNames = model.AllowedUserNames?.FirstOrDefault()?.Split(',').Select(x => x.Trim()).Where(x => x.Length != 0).ToList();

            if (model.Tags != null)
            {
                foreach (var tagName in model.Tags)
                {
                    if (!template.TemplateTags.Any(x => x.Tag.Name == tagName))
                    {
                        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName) ?? new Tag { Name = tagName };
                        template.TemplateTags.Add(new TemplateTag { Template = template, Tag = tag });
                    }
                }
            }

            if (model.AllowedUserNames != null)
            {
                foreach (var userName in model.AllowedUserNames)
                {
                    var user = await _userManager.FindByNameAsync(userName);
                    if (user != null)
                    {
                        template.AllowedUsers.Add(new TemplateAccess { Template = template, User = user });
                    }
                }
            }

            foreach (var questionModel in model.Questions.Select((q, index) => new { q, index }))
            {
                template.Questions.Add(new Question
                {
                    Title = questionModel.q.Title,
                    Description = questionModel.q.Description,
                    Type = questionModel.q.Type,
                    ShowInResults = questionModel.q.ShowInResults,
                    Order = questionModel.index
                });
            }

            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var template = await _context.Templates
                .Include(x => x.Questions.OrderBy(x => x.Order))
                .Include(x => x.Author)
                .Include(x => x.AllowedUsers)
                    .ThenInclude(x => x.User)
                .Include(x => x.TemplateTags)
                    .ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            if (template.Author.Id != currentUser.Id && !isAdmin)
            {
                return Forbid();
            }

            var model = new TemplateEditViewModel
            {
                Id = template.Id,
                Author = template.Author,
                Title = template.Title,
                Description = template.Description,
                DescriptionWithMarkdown = template.DescriptionWithMarkdown,
                CurrentImageUrl = template.ImageUrl,
                AccessType = template.AccessType,
                Questions = template.Questions.Select(q => new QuestionUpdate
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    Type = q.Type,
                    ShowInResults = q.ShowInResults,
                    Order = q.Order
                }).ToList(),

                Tags = template.TemplateTags.Select(x => x.Tag.Name).ToList(),
                AllowedUserNames = template.AllowedUsers.Select(x => new UserNameAndEmail() { UserName = x.User.UserName, Email = x.User.Email }).ToList()!,
                TopicId = template.TopicId,
                Topics = await GetTopicsAsync()
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(TemplateEditViewModel model)
        {

            var template = await _context.Templates
                .Include(x => x.Questions.OrderBy(x => x.Order))
                .Include(x => x.Author)
                .Include(x => x.AllowedUsers)
                    .ThenInclude(x => x.User)
                .Include(x => x.TemplateTags)
                    .ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync(f => f.Id == model.Id);

            if (template == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                //model.Questions = template.Questions.Select(q => new QuestionUpdate
                //{
                //    Id = q.Id,
                //    Title = q.Title,
                //    Description = q.Description,
                //    Type = q.Type,
                //    ShowInResults = q.ShowInResults,
                //    Order = q.Order
                //}).ToList();
                return View(model);
            }

            model.Tags = model.Tags?.Where(tag => !string.IsNullOrWhiteSpace(tag)).ToList() ?? new List<string>();
            var allowedUsers = model.AllowedUserNames?.Select(x => x.UserName).ToList() ?? new List<string>();

            var currentUser = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            if (template.Author.Id != currentUser.Id && !isAdmin)
            {
                return Forbid();
            }

            template.Title = model.Title;
            template.Description = MarkdownHelper.ExtractPlainText(model.DescriptionWithMarkdown);
            template.DescriptionWithMarkdown = model.DescriptionWithMarkdown;
            template.AccessType = model.AccessType;
            template.TopicId = model.TopicId;

            model.Tags = model.Tags?.FirstOrDefault()?.Split(',').Select(x => x.Trim()).Where(x => x.Length != 0).ToList();

            if (model.NewImageFile != null)
            {
                var newImageUrl = await UploadImageToImgBB(model.NewImageFile);
                if (!string.IsNullOrEmpty(newImageUrl))
                {
                    template.ImageUrl = newImageUrl;
                }
            }

            if (model.Tags != null)
            {
                var tagsToRemove = template.TemplateTags
                    .Where(x => !model.Tags.Contains(x.Tag.Name))
                    .ToList();

                foreach (var tagToRemove in tagsToRemove)
                {
                    template.TemplateTags.Remove(tagToRemove);
                }

                foreach (var tagName in model.Tags)
                {
                    if (!template.TemplateTags.Any(x => x.Tag.Name == tagName))
                    {
                        var tag = await _context.Tags.FirstOrDefaultAsync(x => x.Name == tagName) ?? new Tag { Name = tagName };
                        template.TemplateTags.Add(new TemplateTag { Template = template, Tag = tag });
                    }
                }
            }
            else
            {
                template.TemplateTags.Clear();
            }

            if (allowedUsers != null)
            {
                var usersToRemove = template.AllowedUsers
                    .Where(x => !allowedUsers.Contains(x.User.UserName))
                    .ToList();

                foreach (var userToRemove in usersToRemove)
                {
                    template.AllowedUsers.Remove(userToRemove);
                }

                foreach (var userName in allowedUsers)
                {
                    var user = await _userManager.FindByNameAsync(userName);
                    if (user != null && !template.AllowedUsers.Any(x => x.User.Id == user.Id))
                    {
                        template.AllowedUsers.Add(new TemplateAccess { Template = template, User = user });
                    }
                }
            }
            else
            {
                template.AllowedUsers.Clear();
            }

            var existingQuestions = template.Questions.ToDictionary(x => x.Id);
            foreach (var questionModel in model.Questions)
            {
                if (existingQuestions.TryGetValue(questionModel.Id, out var existingQuestion))
                {
                    if (existingQuestion.Type != questionModel.Type)
                    {
                        var relatedAnswers = await _context.Answers
                            .Where(a => a.QuestionId == existingQuestion.Id)
                            .ToListAsync();

                        foreach (var answer in relatedAnswers)
                        {
                            _context.Answers.Remove(answer);

                            switch (questionModel.Type)
                            {
                                case QuestionType.SingleLineText:
                                    _context.Answers.Add(new TextAnswer
                                    {
                                        QuestionId = answer.QuestionId,
                                        FormId = answer.FormId
                                    });
                                    break;

                                case QuestionType.Integer:
                                    _context.Answers.Add(new IntegerAnswer
                                    {
                                        QuestionId = answer.QuestionId,
                                        FormId = answer.FormId
                                    });
                                    break;

                                case QuestionType.Checkbox:
                                    _context.Answers.Add(new CheckboxAnswer
                                    {
                                        QuestionId = answer.QuestionId,
                                        FormId = answer.FormId
                                    });
                                    break;
                            }
                        }
                    }
                    existingQuestion.Title = questionModel.Title;
                    existingQuestion.Description = questionModel.Description;
                    existingQuestion.Type = questionModel.Type;
                    existingQuestion.Order = questionModel.Order;
                    existingQuestion.ShowInResults = questionModel.ShowInResults;
                }
                else
                {
                    template.Questions.Add(new Question
                    {
                        Title = questionModel.Title,
                        Description = questionModel.Description,
                        Type = questionModel.Type,
                        Order = questionModel.Order,
                        TemplateId = template.Id,
                        ShowInResults = questionModel.ShowInResults
                    });
                }
            }

            var questionIdsToKeep = model.Questions.Select(x => x.Id).ToHashSet();
            var questionsToRemove = template.Questions.Where(x => !questionIdsToKeep.Contains(x.Id)).ToList();
            _context.Questions.RemoveRange(questionsToRemove);

            await _context.SaveChangesAsync();

            return RedirectToAction("Info", new { id = template.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Info(int id, string sortOrderAnswers, string sortColumnAnswers, string currentTab = "Details")
        {
            var template = await _context.Templates
                .Include(x => x.Questions.OrderBy(x => x.Order))
                .Include(x => x.Author)
                .Include(x => x.Likes)
                .Include(x => x.Topic)
                .Include(x => x.Comments)
                    .ThenInclude(x => x.User)
                .Include(x => x.Forms)
                    .ThenInclude(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            bool isOwner = false;
            bool isAdmin = false;
            var isAnswerExists = false;

            if (currentUser != null)
            {
                isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
                isOwner = template.Author.Id == currentUser.Id;

                isAnswerExists = template.Forms.FirstOrDefault(x => x.TemplateId == template.Id && x.User == currentUser) != null;
            }

            var model = new TemplateInfoViewModel
            {
                Details = await Details(id),
                Answers = await Answers(id, sortColumnAnswers, sortOrderAnswers),
                Statistics = await Statistics(id),
                CurrentTab = currentTab
            };

            return View(model);
        }
        
        [Authorize]
        private async Task<TemplateStatisticsViewModel> Statistics(int id)
        {
            var template = await _context.Templates
                .Include(x => x.Questions)
                .Include(x => x.Author)
                .Include(x => x.Forms)
                    .ThenInclude(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == id);

            var statistics = template.Questions.Select(question =>
            {
                var answers = template.Forms
                    .SelectMany(fa => fa.Answers)
                    .Where(a => a.QuestionId == question.Id)
                    .ToList();

                var result = new QuestionStats
                {
                    QuestionTitle = question.Title,
                    QuestionDescription = question.Description,
                    QuestionType = question.Type
                };

                switch (question.Type)
                {
                    case QuestionType.Integer:
                        var integerAnswers = answers.OfType<IntegerAnswer>().Select(a => (long)a.IntegerAnswerValue).ToList();
                        result.IntegerStats = integerAnswers.Any() ? new IntegerStatistics
                        {
                            Min = integerAnswers.Min(),
                            Max = integerAnswers.Max(),
                            Mean = CalculateMean(integerAnswers),
                            Median = CalculateMedian(integerAnswers),
                            StdDev = CalculateStandardDeviation(integerAnswers)
                        } : null;
                        break;

                    case QuestionType.SingleLineText:
                    case QuestionType.MultiLineText:
                        var textAnswers = answers.OfType<TextAnswer>().Select(a => a.TextAnswerValue).ToList();
                        result.TextStats = textAnswers.Any() ? new TextStatistics
                        {
                            MostFrequent = textAnswers.GroupBy(x => x)
                                .OrderByDescending(g => g.Count())
                                .FirstOrDefault()?.Key
                        } : null;
                        break;

                    case QuestionType.Checkbox:
                        var checkboxAnswers = answers.OfType<CheckboxAnswer>().Select(a => (bool)a.CheckboxAnswerValue).ToList();
                        result.CheckboxStats = checkboxAnswers.Any() ? new CheckboxStatistics
                        {
                            SelectionPercentage = (checkboxAnswers.Count(x => x) / (double)checkboxAnswers.Count()) * 100
                        } : null;
                        break;
                }

                return result;
            }).ToList();

            var model = new TemplateStatisticsViewModel
            {
                QuestionsStatistic = statistics
            };

            return model;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id, string? returnUrl = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var template = await _context.Templates
                .Include(x => x.Author)
                .Include(x => x.Forms)
                    .ThenInclude(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == id);

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            if (!isAdmin && currentUser != template.Author)
            {
                return Forbid();
            }

            if (template == null)
            {
                return NotFound();
            }

            _context.Answers.RemoveRange(template.Forms.SelectMany(x => x.Answers));
            _context.Templates.Remove(template);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Main", "Template");
        }

        private async Task<TemplateDetailsViewModel> Details(int id)
        {
            var template = await _context.Templates
                .Include(x => x.Questions.OrderBy(x => x.Order))
                .Include(x => x.Author)
                .Include(x => x.Likes)
                .Include(x => x.Topic)
                .Include(x => x.Comments)
                    .ThenInclude(x => x.User)
                .Include(x => x.Forms)
                    .ThenInclude(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == id);

            var currentUser = await _userManager.GetUserAsync(User);
            bool isOwner = false;
            bool isAdmin = false;
            var isAnswerExists = false;

            if (currentUser != null)
            {
                isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
                isOwner = template.Author.Id == currentUser.Id;

                isAnswerExists = template.Forms.FirstOrDefault(x => x.TemplateId == template.Id && x.User == currentUser) != null;
            }

            var model = new TemplateDetailsViewModel
            {
                Template = template,
                IsEditable = isOwner || isAdmin,
                UserAnswers = template.Forms.Where(x => x.UserId == currentUser?.Id).ToList(),
                Questions = template.Questions.ToList(),
                UserHasLiked = template.Likes.Any(x => x.UserId == currentUser?.Id),
                IsAnswerExists = isAnswerExists
            };

            return model;
        }
        [Authorize]
        private async Task<TemplateAnswersViewModel> Answers(int id, string sortColumn = "UserName", string sortOrder = "asc")
        {

            var forms = _context.Forms
                .Include(x => x.User)
                .Where(x => x.TemplateId == id)
                .AsQueryable();

            forms = sortColumn switch
            {
                "UserName" => sortOrder == "asc" ? forms.OrderBy(x => x.User.UserName) : forms.OrderByDescending(x => x.User.UserName),
                "SubmittedAt" => sortOrder == "asc" ? forms.OrderBy(x => x.SubmittedAt) : forms.OrderByDescending(x => x.SubmittedAt),
                _ => forms
            };

            var model = new TemplateAnswersViewModel
            {
                TemplateId = id,
                Forms = await forms.ToListAsync(),
                SortColumn = sortColumn,
                SortOrder = sortOrder
            };

            return model;
        }
        [HttpGet]
        public IActionResult GetTags(string begin)
        {
            var tags = _context.Tags
                .Where(x => x.Name.StartsWith(begin))
                .Select(x => x.Name)
                .OrderBy(x => x)
                .Take(10)
                .ToList();

            return Json(tags);
        }
        [HttpGet]
        public IActionResult GetUsers(string begin)
        {
            var users = _context.Users
                .Where(x => (x.UserName.StartsWith(begin) || x.Email.StartsWith(begin)))
                .Select(x => new { x.UserName, x.Email })
                .Take(10)
                .ToList();

            return Json(users);
        }
      
        private async Task<List<Topic>> GetTopicsAsync()
        {
            return await _context.Topics.ToListAsync();
        }

        private double CalculateMean(List<long> numbers)
        {
            Dictionary<long, double> xAndFreq = new Dictionary<long, double>();
            for (int i = 0; i < numbers.Count; i++)
            {
                if (!xAndFreq.ContainsKey(numbers[i]))
                {
                    xAndFreq.Add(numbers[i], 1);
                }
                else
                {
                    xAndFreq[numbers[i]]++;
                }
            }

            double mean = 0;
            int count = numbers.Count;

            foreach (var item in xAndFreq)
            {
                mean += item.Key * (item.Value / count);
            }
            return mean;
        }
        private double CalculateMedian(List<long> numbers)
        {
            var sorted = numbers.OrderBy(n => n).ToList();
            int count = sorted.Count;

            if (count % 2 == 0)
            {
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
            }
            return sorted[count / 2];
        }

        private double CalculateStandardDeviation(List<long> numbers)
        {
            var mean = CalculateMean(numbers);
            var dispersion = CalculateMean(numbers.Select(x => x * x).ToList()) - (mean * mean);
            return Math.Sqrt(dispersion);
        }
        private async Task<string?> UploadImageToImgBB(IFormFile imageFile)
        {
            const string apiKey = "870bece3125067607ba26de2f9349f40";
            const string uploadUrl = "https://api.imgbb.com/1/upload";

            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();
            var base64Image = Convert.ToBase64String(imageBytes);

            using var client = new HttpClient();
            var content = new MultipartFormDataContent
            {
                { new StringContent(apiKey), "key" },
                { new StringContent(base64Image), "image" }
            };

            var response = await client.PostAsync(uploadUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<JsonElement>(responseBody);
                return json.GetProperty("data").GetProperty("url").GetString();
            }

            return null;
        }
    }
}
