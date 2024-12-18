using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseProject.Migrations
{
    /// <inheritdoc />
    public partial class fulltextsearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG CourseCatalog WITH ACCENT_SENSITIVITY = OFF AS DEFAULT", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Answers(TextAnswerValue LANGUAGE 0) KEY INDEX PK_Answers ON (CourseCatalog) WITH (CHANGE_TRACKING AUTO)", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Comments(Text LANGUAGE 0) KEY INDEX PK_Comments ON (CourseCatalog) WITH (CHANGE_TRACKING AUTO)", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Questions(Title, Description LANGUAGE 0) KEY INDEX PK_Questions ON (CourseCatalog) WITH (CHANGE_TRACKING AUTO)", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Tags(Name LANGUAGE 0) KEY INDEX PK_Tags ON (CourseCatalog) WITH (CHANGE_TRACKING AUTO)", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Templates(Title LANGUAGE 0, Description LANGUAGE 0) KEY INDEX PK_Templates ON (CourseCatalog) WITH (CHANGE_TRACKING AUTO)", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Topics(Name LANGUAGE 0) KEY INDEX PK_Topics ON (CourseCatalog) WITH (CHANGE_TRACKING AUTO)", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON AspNetUsers(UserName LANGUAGE 0, Email Language 0) KEY INDEX PK_Topics ON (CourseCatalog) WITH (CHANGE_TRACKING AUTO)", true);
            migrationBuilder.Sql("CREATE PROCEDURE SearchTemplates " +
                "@SearchTerm NVARCHAR(500) " +
                "AS " +
                "BEGIN " +
                "SELECT DISTINCT Templates.Id " +
                "FROM Templates " +
                "LEFT JOIN Topics ON Templates.TopicId = Topics.Id " +
                "LEFT JOIN Questions ON Questions.TemplateId = Templates.Id " +
                "LEFT JOIN Answers ON Answers.QuestionId = Questions.Id " +
                "LEFT JOIN AspNetUsers ON Templates.AuthorId = AspNetUsers.Id " +
                "LEFT JOIN Comments ON Templates.Id = Comments.TemplateId " +
                "LEFT JOIN TemplateTags ON TemplateTags.TemplateId = Templates.Id " +
                "LEFT JOIN Tags ON TemplateTags.TagId = TemplateTags.TagId " +
                "WHERE CONTAINS((Templates.Title, Templates.Description), @SearchTerm) " +
                    "OR CONTAINS(Topics.Name, @SearchTerm) " +
                    "OR CONTAINS((Questions.Title, Questions.Description), @SearchTerm) " +
                    "OR CONTAINS(Answers.TextAnswerValue, @SearchTerm) " +
                    "OR CONTAINS(AspNetUsers.UserName, @SearchTerm) " +
                    "OR CONTAINS(Comments.Text, @SearchTerm) " +
                    "OR CONTAINS(Tags.Name, @SearchTerm); " +
                "END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON Answers", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON Comments", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON Questions", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON Tags", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON Templates", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON Topics", true);
            migrationBuilder.Sql("DROP FULLTEXT INDEX ON AspNetUsers", true);
            migrationBuilder.Sql("DROP FULLTEXT CATALOG CourseCatalog", true);
            migrationBuilder.Sql("DROP PROCEDURE SearchTemplates", true);
        }
    }
}
