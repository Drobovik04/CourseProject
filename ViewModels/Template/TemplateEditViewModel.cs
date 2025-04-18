﻿using CourseProject.Attributes;
using CourseProject.Models;
using CourseProject.SupportClasses.Template;
using Microsoft.AspNetCore.Identity;

namespace CourseProject.ViewModels.Template
{
    public class TemplateEditViewModel
    {
        public int Id { get; set; }
        public AppUser Author { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? DescriptionWithMarkdown { get; set; }
        public string? CurrentImageUrl { get; set; }
        public IFormFile? NewImageFile { get; set; }
        public AccessType AccessType { get; set; }
        public List<string>? Tags { get; set; } = new();
        public List<UserNameAndEmail>? AllowedUserNames { get; set; } = new();
        [LocalizedMinLength(1, "MinLengthArray")]
        public List<QuestionUpdate> Questions { get; set; } = new();
        public int? TopicId { get; set; }
        public List<Topic> Topics { get; set; } = new();
    }
}
